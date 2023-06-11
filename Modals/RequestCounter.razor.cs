using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ciklonalozi.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace ciklonalozi.Modals;

public partial class RequestCounter : IDisposable
{
    [Inject] public IDbContextFactory<AppDbContext> DbFactory { get; set; } = null!;
    readonly PeriodicTimer _periodicTimer = new(TimeSpan.FromMinutes(15));
    List<Request> Data = new();
    int NewCount;
    bool Shown;
    protected override async Task OnInitializedAsync()
    {
        RequestPublisher.OnReceived += Refresh;
        RunTimer();
        await RefreshAsync();
    }

    async void RunTimer()
    {
        while (await _periodicTimer.WaitForNextTickAsync())
            await RefreshAsync();
    }
    async void Refresh() => await RefreshAsync();
    public async Task RefreshAsync()
    {
        using var db = DbFactory.CreateDbContext();
        Data = await db.Requests
            .AsNoTracking()
            .OrderByDescending(r => r.OrderId == null)
            .ThenBy(r => r.Created)
            .ToListAsync();

        NewCount = Data.Count(r => r.OrderId == null);
        //StateHasChanged();
        await InvokeAsync(StateHasChanged);
    }
    public void Show()
    {
        Shown = true;
        StateHasChanged();
    }
    public void Hide()
    {
        Shown = false;
        StateHasChanged();
    }

    string ButtonClasses()
    {
        return NewCount == 0 ? "btn-outline-success" : "btn-success pulse";
    }

    public void Dispose()
    {
        RequestPublisher.OnReceived -= Refresh;
        _periodicTimer?.Dispose();
    }
}