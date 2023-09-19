using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using ciklonalozi.Data;
using ciklonalozi.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace ciklonalozi.Modals
{
    public partial class EditOrder
    {
        [Parameter] public EventCallback OnSaved { get; set; }
        [Inject] public IDbContextFactory<AppDbContext> DbFactory { get; set; } = null!;
        bool Shown;
        string LocalUrl = string.Empty;
        EditOrderModel Model = new();
        Order? OriginalOrder;
        bool ShowEmail;
        private Dictionary<string, string>? Errors;
        public async Task Show(Order order)
        {
            LocalUrl = C.Hasher.GetQrUrl(order.OrderId);
            Model.ContactName = order.ContactName;
            Model.ContactPhone = order.ContactPhone;
            Model.ContactEmail = order.ContactEmail;
            Model.Subject = order.Subject;
            Model.Description = order.Description;
            Model.Arrival = order.Arrival;
            Model.Arrived = order.Arrived;
            Model.Completed = order.Completed;
            Model.Returned = order.Returned;
            Model.EstimatedPrice = order.EstimatedPrice;
            Model.Effort = order.Effort;
            Model.RealPrice = order.RealPrice;
            Model.Removed = order.Removed;

            // Update original if changed in the meantime
            using var db = DbFactory.CreateDbContext();
            await db.Entry(order).ReloadAsync();
            OriginalOrder = order;

            Shown = true;
            StateHasChanged();
        }
        public void Hide()
        {
            Model = new();
            Shown = false;
            StateHasChanged();
        }
        async Task SaveClicked()
        {
            Errors = Model.Validate();
            if (Errors != null || OriginalOrder == null)
                return;

            using var db = DbFactory.CreateDbContext();
            db.Attach(OriginalOrder);

            OriginalOrder.ContactName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(Model.ContactName!);
            OriginalOrder.ContactNameNormalized = C.Normalize(OriginalOrder.ContactName);
            if (!string.IsNullOrWhiteSpace(Model.ContactPhone))
            {
                OriginalOrder.ContactPhone = Model.ContactPhone;
                OriginalOrder.ContactPhoneNormalized = string.Concat(Model.ContactPhone.Where(char.IsDigit));
            }
            else
            {
                OriginalOrder.ContactPhone = OriginalOrder.ContactPhoneNormalized = null;
            }
            OriginalOrder.ContactEmail = Model.ContactEmail;
            OriginalOrder.Subject = Model.Subject!;
            OriginalOrder.Description = Model.Description;
            if (!string.IsNullOrWhiteSpace(OriginalOrder.Description))
                OriginalOrder.DescriptionNormalized = C.Normalize(OriginalOrder.Description);
            OriginalOrder.Arrival = Model.Arrival!.Value;
            OriginalOrder.Arrived = Model.Arrived;
            OriginalOrder.Completed = Model.Completed;
            OriginalOrder.Returned = Model.Returned;
            OriginalOrder.EstimatedPrice = Model.EstimatedPrice;
            OriginalOrder.Effort = Model.Effort!.Value;
            OriginalOrder.RealPrice = Model.RealPrice;
            OriginalOrder.Removed = Model.Removed;

            Hide();

            await db.SaveChangesAsync();

            if (OnSaved.HasDelegate)
                await OnSaved.InvokeAsync();
        }
        string Signature =
"""
Hvala što koristite Ciklo-Sport.
-- 
www.ciklo-sport.hr
""";
        string EmailFinished => $"mailto:{Model?.ContactEmail}?subject={Uri.EscapeDataString("Servis završen")}&body={Uri.EscapeDataString(BodyFinished)}";
        string BodyFinished =>
$"""
Poštovani,

vaš predmet {Model.Subject} je spreman za preuzimanje.

{Signature}
""";
        string EmailDeclined => $"mailto:{Model?.ContactEmail}?subject={Uri.EscapeDataString("Narudžba odbijena")}&body={Uri.EscapeDataString(BodyDeclined)}";
        string BodyDeclined =>
$"""
Poštovani,

Karlo je lijen i ovo neka mu stoji tu na sramotu.

{Signature}
""";
    }
}