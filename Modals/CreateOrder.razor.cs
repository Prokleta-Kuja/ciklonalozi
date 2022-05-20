using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ciklonalozi.Data;
using ciklonalozi.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace ciklonalozi.Modals
{
    public partial class CreateOrder
    {
        [Parameter] public EventCallback OnSaved { get; set; }
        [Inject] public IDbContextFactory<AppDbContext> DbFactory { get; set; } = null!;
        bool Shown;
        CreateOrderModel Model = new();
        private string? PhoneNumber { get => Model.ContactPhone; set { _ = CheckPhoneNumber(value); } }
        private Dictionary<string, string>? Errors;
        public void Show()
        {
            Model.ArrivalOrArrived = DateTime.UtcNow.Date.AddHours(8);
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
            var name = await db.Orders
                .Where(o => o.ContactPhoneNormalized == normalized)
                .OrderByDescending(o => o.Arrival)
                .Select(o => o.ContactName)
                .FirstOrDefaultAsync();

            if (!string.IsNullOrWhiteSpace(name))
                Model.ContactName = name;
        }
        async Task SaveClicked()
        {
            Errors = Model.Validate();
            if (Errors != null)
                return;

            var order = new Order(Model.ContactName!, Model.ContactPhone, Model.Subject!, Model.ArrivalOrArrived!.Value);
            if (!Model.IsArrival)
                order.Arrived = Model.ArrivalOrArrived.Value;

            order.Description = Model.Description;

            using var db = DbFactory.CreateDbContext();
            db.Orders.Add(order);
            await db.SaveChangesAsync();

            if (OnSaved.HasDelegate)
                await OnSaved.InvokeAsync();

            Hide();
        }
    }
}