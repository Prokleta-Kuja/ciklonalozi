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
        string? Query;
        ElementReference QueryElement;
        DateTime? From = DateTime.UtcNow.Date.AddDays(-1);
        DateTime? To = DateTime.UtcNow.Date.AddDays(5);
        CreateOrder? CreateOrderModal;
        EditOrder? EditOrderModal;
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
                await QueryElement.FocusAsync();

            await Refresh();
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
                    EF.Functions.Like(o.ContactPhone, likeStr) ||
                    EF.Functions.Like(o.Description, likeStr));
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
        void AddClicked()
        {
            CreateOrderModal?.Show();
        }
        void EditClicked(Order order)
        {
            EditOrderModal?.Show(order);
        }
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
    }
}