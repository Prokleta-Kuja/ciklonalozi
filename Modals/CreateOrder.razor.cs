using System;
using System.Collections.Generic;
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