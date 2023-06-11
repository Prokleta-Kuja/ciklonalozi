using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ciklonalozi.Data;
using ciklonalozi.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace ciklonalozi.Pages;

public partial class Requests
{
    [Inject] public IDbContextFactory<AppDbContext> DbFactory { get; set; } = null!;
    [Parameter] public int Id { get; set; }
    bool Loading = true;
    Request? Item;
    CreateOrderModel Model = new();
    Dictionary<string, string>? Errors;
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
            return;

        using var db = DbFactory.CreateDbContext();
        Item = await db.Requests
            .Where(r => r.RequestId == Id)
            .Include(r => r.Order)
            .SingleOrDefaultAsync();

        Loading = false;

        if (Item == null)
        {
            StateHasChanged();
            return;
        }

        Model.ArrivalOrArrived = TimeZoneInfo.ConvertTimeToUtc(Item.Date.ToDateTime(new(9, 0), DateTimeKind.Unspecified), C.TZ);
        Model.ContactEmail = Item.Email;
        Model.ContactName = Item.Contact;
        Model.ContactPhone = Item.Phone;
        Model.Description = Item.Description;
        Model.Effort = 20;
        Model.IsArrival = true;
        Model.Subject = Item.Subject;
        StateHasChanged();
    }
    async Task SaveClicked()
    {
        Errors = Model.Validate();
        if (Errors != null)
            return;

        var order = new Order(Model.ContactName!, Model.ContactPhone, Model.Subject!, Model.ArrivalOrArrived!.Value);
        if (!Model.IsArrival)
            order.Arrived = DateTime.UtcNow;

        order.ContactEmail = Model.ContactEmail;
        order.Description = Model.Description;
        order.EstimatedPrice = Model.EstimatedPrice;
        order.Effort = Model.Effort!.Value;

        using var db = DbFactory.CreateDbContext();
        if (Item != null)
        {
            db.Requests.Attach(Item);
            order.Request = Item;
        }

        db.Orders.Add(order);
        await db.SaveChangesAsync();

        if (Item != null)
            Item.Order = order;

        RequestPublisher.Raise();
    }
    async Task Delete()
    {
        if (Item == null)
            return;

        using var db = DbFactory.CreateDbContext();
        db.Requests.Remove(Item);
        await db.SaveChangesAsync();

        Item = null;
        RequestPublisher.Raise();
        StateHasChanged();
    }
    string Signature =
    """
    Hvala što koristite Ciklo-Sport.
    www.ciklo-sport.hr
    """;
    string EmailAccepted => $"mailto:{Item?.Email}?subject={Uri.EscapeDataString("Narudžba prihvaćena")}&body={Uri.EscapeDataString(BodyAccepted)}";
    string BodyAccepted =>
    $"""
    Poštovani,

    vaša narudžba je prihvaćena za {C.Display(Item?.Order?.Arrival)}.

    {Signature}
    """;
    string EmailDeclined => $"mailto:{Item?.Email}?subject={Uri.EscapeDataString("Narudžba odbijena")}&body={Uri.EscapeDataString(BodyDeclined)}";
    string BodyDeclined =>
    $"""
    Poštovani,

    Karlo je lijen i ovo neka mu stoji tu na sramotu.

    {Signature}
    """;
}