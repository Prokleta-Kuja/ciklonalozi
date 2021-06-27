using System;
using System.Collections.Generic;
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
        EditOrderModel Model = new();
        Order? OriginalOrder;
        private Dictionary<string, string>? Errors;
        public void Show(Order order)
        {
            OriginalOrder = order;

            Model.ContactName = order.ContactName;
            Model.ContactPhone = order.ContactPhone;
            Model.Subject = order.Subject;
            Model.Description = order.Description;
            Model.Arrival = order.Arrival;
            Model.Arrived = order.Arrived;
            Model.Completed = order.Completed;

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

            OriginalOrder.ContactName = Model.ContactName!;
            OriginalOrder.ContactPhone = Model.ContactPhone;
            OriginalOrder.Subject = Model.Subject!;
            OriginalOrder.Description = Model.Description;
            OriginalOrder.Arrival = Model.Arrival!.Value;
            OriginalOrder.Arrived = Model.Arrived;
            OriginalOrder.Completed = Model.Completed;

            await db.SaveChangesAsync();

            if (OnSaved.HasDelegate)
                await OnSaved.InvokeAsync();

            Hide();
        }
    }
}