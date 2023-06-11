using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ciklonalozi.Data;
using ciklonalozi.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;

namespace ciklonalozi.Modals
{
    public partial class CreateOrder
    {
        [Parameter] public EventCallback OnSaved { get; set; }
        [Inject] public IJSRuntime Js { get; set; } = null!;
        [Inject] public IDbContextFactory<AppDbContext> DbFactory { get; set; } = null!;
        bool Shown;
        CreateOrderModel Model = new();
        private string? PhoneNumber { get => Model.ContactPhone; set { _ = CheckPhoneNumber(value); } }
        private Dictionary<string, string>? Errors;
        public void Show()
        {
            Model.ArrivalOrArrived = DateTime.UtcNow.Date.AddHours(17);
            Shown = true;
            StateHasChanged();
        }
        public void Hide()
        {
            Model = new();
            Shown = false;
            StateHasChanged();
        }
        async Task CheckPhoneNumber(string? phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                Model.ContactPhone = null;
                return;
            }

            Model.ContactPhone = phoneNumber;
            var normalized = string.Concat(phoneNumber.Where(char.IsDigit));

            using var db = DbFactory.CreateDbContext();
            var prev = await db.Orders
                .Where(o => o.ContactPhoneNormalized == normalized)
                .OrderByDescending(o => o.Arrival)
                .FirstOrDefaultAsync();

            if (prev != null)
            {
                Model.ContactName = prev.ContactName;
                Model.Subject = prev.Subject;
            }
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
            db.Orders.Add(order);
            await db.SaveChangesAsync();

            if (OnSaved.HasDelegate)
                await OnSaved.InvokeAsync();

            if (order.Arrived.HasValue)
                await Js.InvokeVoidAsync("open", $"/print/{order.OrderId}", "_blank");

            Hide();
        }
    }
}