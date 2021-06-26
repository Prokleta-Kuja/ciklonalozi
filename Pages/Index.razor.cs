using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using ciklonalozi.Data;
using ciklonalozi.Modals;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace ciklonalozi.Pages
{
    public partial class Index
    {
        [Inject] public AppDbContext Db { get; set; } = null!;
        CultureInfo CI = CultureInfo.GetCultureInfo("HR-hr");
        TimeZoneInfo TZ = TimeZoneInfo.FindSystemTimeZoneById("Europe/Zagreb");
        DateTime Today { get; } = DateTime.UtcNow.Date;
        DateTime Yesterday { get; } = DateTime.UtcNow.AddDays(-1).Date;
        List<Order> Orders { get; set; } = new();
        CreateOrder? CreateOrderModal;
        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            return Refresh();
        }
        async Task Refresh()
        {
            Orders = await Db.Orders.ToListAsync();
            StateHasChanged();
        }
        void AddClicked()
        {
            CreateOrderModal?.Show();
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