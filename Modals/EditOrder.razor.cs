using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using ciklonalozi.Data;
using ciklonalozi.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using WebPush;

namespace ciklonalozi.Modals
{
    public partial class EditOrder
    {
        [Parameter] public EventCallback OnSaved { get; set; }
        [Inject] public IDbContextFactory<AppDbContext> DbFactory { get; set; } = null!;
        bool Shown;
        EditOrderModel Model = new();
        Order? OriginalOrder;
        bool PushSend;
        string? PushTitle;
        string? PushBody;
        private Dictionary<string, string>? Errors;
        public async Task Show(Order order)
        {

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

            PushTitle = "Servis završen";
            PushBody = "Vaš bicikl je spreman za preuzimanje.";

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
            Shown = PushSend = false;
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
            OriginalOrder.Arrival = Model.Arrival!.Value;
            OriginalOrder.Arrived = Model.Arrived;
            OriginalOrder.Completed = Model.Completed;
            OriginalOrder.Returned = Model.Returned;
            OriginalOrder.EstimatedPrice = Model.EstimatedPrice;
            OriginalOrder.Effort = Model.Effort!.Value;
            OriginalOrder.RealPrice = Model.RealPrice;
            OriginalOrder.Removed = Model.Removed;

            Hide();

            if (PushSend)
            {
                PushSend = false;
                var webPushClient = new WebPushClient();
                var url = C.Hasher.GetQrUrl(OriginalOrder.OrderId);
                var notification = JsonSerializer.Serialize(new { title = PushTitle, message = PushBody, url });
                try
                {
                    await webPushClient.SendNotificationAsync(
                        new(OriginalOrder.Endpoint, OriginalOrder.P256DH, OriginalOrder.Auth),
                        notification,
                        C.Vapid.Current);
                }
                catch (WebPushException ex)
                {
                    if (ex.StatusCode == HttpStatusCode.Gone)
                        OriginalOrder.Endpoint = OriginalOrder.P256DH = OriginalOrder.Auth = null;
                }
                catch (System.Exception ex)
                {
                    System.Console.WriteLine(ex.Message);
                }
            }

            await db.SaveChangesAsync();

            if (OnSaved.HasDelegate)
                await OnSaved.InvokeAsync();
        }
    }
}