using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ciklonalozi.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;

namespace ciklonalozi.Pages;

public partial class Anal
{
    [Inject] public IDbContextFactory<AppDbContext> DbFactory { get; set; } = null!;
    [Inject] public IJSRuntime JS { get; set; } = null!;
    public Dictionary<int, decimal> YearCount { get; set; } = new();
    public Dictionary<int, Dictionary<int, decimal>> MonthCount { get; set; } = new();
    public Dictionary<int, decimal> YearPrice { get; set; } = new();
    public Dictionary<int, Dictionary<int, decimal>> MonthPrice { get; set; } = new();
    public Dictionary<int, decimal> YearRemoved { get; set; } = new();
    public Dictionary<int, Dictionary<int, decimal>> MonthRemoved { get; set; } = new();
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
            await LoadDataAsync();
    }

    async Task LoadDataAsync()
    {
        using var db = DbFactory.CreateDbContext();
        var orders = await db.Orders.AsNoTracking().OrderBy(o => o.Arrival).ToListAsync();

        foreach (var order in orders)
        {
            var year = order.Arrival.Year;
            var month = order.Arrival.Month;

            if (order.Removed)
            {
                if (YearRemoved.TryAdd(year, 1))
                    MonthRemoved.Add(year, new() { { 1, 0 }, { 2, 0 }, { 3, 0 }, { 4, 0 }, { 5, 0 }, { 6, 0 }, { 7, 0 }, { 8, 0 }, { 9, 0 }, { 10, 0 }, { 11, 0 }, { 12, 0 }, });
                else
                    YearRemoved[year]++;
                MonthRemoved[year][month]++;
            }
            else
            {
                if (YearCount.TryAdd(year, 1))
                    MonthCount.Add(year, new() { { 1, 0 }, { 2, 0 }, { 3, 0 }, { 4, 0 }, { 5, 0 }, { 6, 0 }, { 7, 0 }, { 8, 0 }, { 9, 0 }, { 10, 0 }, { 11, 0 }, { 12, 0 }, });
                else
                    YearCount[year]++;
                MonthCount[year][month]++;

                if (order.RealPrice.HasValue)
                {
                    if (YearPrice.TryAdd(year, order.RealPrice.Value))
                        MonthPrice.Add(year, new() { { 1, 0 }, { 2, 0 }, { 3, 0 }, { 4, 0 }, { 5, 0 }, { 6, 0 }, { 7, 0 }, { 8, 0 }, { 9, 0 }, { 10, 0 }, { 11, 0 }, { 12, 0 }, });
                    else
                        YearPrice[year] += order.RealPrice.Value;
                    MonthPrice[year][month] += order.RealPrice.Value;
                }
            }

            // if (order.Removed)
            // {
            //     if (!YearRemoved.TryAdd(year, 1))
            //         YearRemoved[year]++;
            //     if (!MonthRemoved.TryAdd(year, new() { { month, 1 } }))
            //         if (!MonthRemoved[year].TryAdd(month, 1))
            //             MonthRemoved[year][month]++;
            // }
            // else
            // {
            //     if (!YearCount.TryAdd(year, 1))
            //         YearCount[year]++;
            //     if (!MonthCount.TryAdd(year, new() { { month, 1 } }))
            //         if (!MonthCount[year].TryAdd(month, 1))
            //             MonthCount[year][month]++;

            //     if (order.RealPrice.HasValue)
            //     {
            //         if (!YearPrice.TryAdd(year, order.RealPrice.Value))
            //             YearPrice[year] += order.RealPrice.Value;
            //         if (!MonthPrice.TryAdd(year, new() { { month, order.RealPrice.Value } }))
            //             if (!MonthPrice[year].TryAdd(month, order.RealPrice.Value))
            //                 MonthPrice[year][month] += order.RealPrice.Value;
            //     }
            // }
        }

        var chart = await JS.InvokeAsync<IJSObjectReference>("import", "./js/chart.min.js");
        var module = await JS.InvokeAsync<IJSObjectReference>("import", "./Pages/Anal.razor.js");
        await module.InvokeVoidAsync("showCharts",
        new
        {
            YearCount,
            MonthCount,
            YearPrice,
            MonthPrice,
            YearRemoved,
            MonthRemoved,
        });
    }
}