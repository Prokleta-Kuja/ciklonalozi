using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ciklonalozi.Data;
using ciklonalozi.Modals;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace ciklonalozi.Pages
{
    public partial class Index
    {
        [Inject] public IDbContextFactory<AppDbContext> DbFactory { get; set; } = null!;
        readonly CultureInfo CI = CultureInfo.GetCultureInfo("HR-hr");
        readonly TimeZoneInfo TZ = TimeZoneInfo.FindSystemTimeZoneById("Europe/Zagreb");
        DateTime Today { get; } = DateTime.UtcNow.Date;
        DateTime Yesterday { get; } = DateTime.UtcNow.AddDays(-1).Date;
        Dictionary<DateTime, List<Order>> Orders { get; set; } = new();
        protected string? Query;
        protected ElementReference QueryElement;
        protected DateTime? From = DateTime.UtcNow.Date.AddDays(-1);
        protected DateTime? To = DateTime.UtcNow.Date.AddDays(5);
        protected CreateOrder? CreateOrderModal;
        protected EditOrder? EditOrderModal;
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await QueryElement.FocusAsync();

                await Refresh();
            }
        }
        async Task Refresh()
        {
            using var db = DbFactory.CreateDbContext();
            var query = db.Orders.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(Query))
            {
                var normalizedString = Query.ToUpperInvariant().Normalize(NormalizationForm.FormD);

                var stringBuilder = new StringBuilder(normalizedString.Length);

                foreach (var c in normalizedString)
                {
                    var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                    if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                        stringBuilder.Append(c);
                }

                var likeStr = $"%{stringBuilder.ToString().Normalize(NormalizationForm.FormC)}%";

                query = query.Where(o =>
                    EF.Functions.Like(o.ContactName, likeStr) ||
                    EF.Functions.Like(o.ContactPhone!, likeStr) ||
                    EF.Functions.Like(o.Description!, likeStr));
            }

            if (From.HasValue)
                query = query.Where(o => o.Arrival > From.Value);

            if (To.HasValue)
                query = query.Where(o => o.Arrival < To.Value);

            query = query.OrderBy(o => o.Arrival);

            var results = await query.ToListAsync();

            Orders.Clear();
            foreach (var order in results)
            {
                if (!Orders.ContainsKey(order.Arrival.Date))
                    Orders.Add(order.Arrival.Date, new());

                Orders[order.Arrival.Date].Add(order);
            }

            StateHasChanged();
        }
        async Task Return(Order order)
        {
            using var db = DbFactory.CreateDbContext();
            db.Attach(order);

            order.Returned = DateTime.UtcNow;
            await db.SaveChangesAsync();
        }
        void AddClicked() => CreateOrderModal?.Show();
        async Task EditClicked(Order order) => await EditOrderModal!.Show(order);
        string Display(DateTime? dt, bool showTime = true, string empty = "-")
        {
            if (!dt.HasValue)
                return empty;

            var printDt = TimeZoneInfo.ConvertTimeFromUtc(dt.Value, TZ);
            var format = CI.DateTimeFormat.ShortDatePattern;

            if (showTime)
                format += $" {CI.DateTimeFormat.ShortTimePattern}";

            return printDt.ToString(format);
        }
        string GetStatusRowClass(Order order)
        {
            if (order.Removed)
                return "text-decoration-line-through";
            if (order.Arrival.Date < Today && !order.Arrived.HasValue)
                return "table-danger";
            if (order.Arrival.Date == Today && !order.Arrived.HasValue)
                return "table-info";
            if (order.Arrived.HasValue && order.Arrived.Value.Date < Yesterday && !order.Completed.HasValue)
                return "table-danger";
            if (order.Arrived.HasValue && !order.Completed.HasValue)
                return "table-warning";
            if (order.Completed.HasValue)
                return "table-success";

            return string.Empty;
        }
        static string GetStatusIconClass(Order order)
        {
            if (order.Removed)
                return "bi bi-eraser-fill";
            if (!order.Arrived.HasValue)
                return "bi bi-clock";
            if (order.Returned.HasValue)
                return "bi bi-check-all";

            return "bi bi-tools";
        }
    }
}