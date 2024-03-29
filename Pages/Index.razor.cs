using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ciklonalozi.Data;
using ciklonalozi.Modals;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace ciklonalozi.Pages
{
    public partial class Index
    {
        const int MAX = 100;
        int _resultCount = 0;
        [Inject] public IDbContextFactory<AppDbContext> DbFactory { get; set; } = null!;
        DateTime Today { get; } = DateTime.UtcNow.Date;
        DateTime Yesterday { get; } = DateTime.UtcNow.AddDays(-1).Date;
        Dictionary<DateTime, List<Order>> Orders { get; set; } = new();
        Dictionary<DateTime, int> Efforts { get; set; } = new();
        protected int? OrderNo;
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

            if (OrderNo.HasValue && OrderNo.Value > 0)
            {
                var order = await query.SingleOrDefaultAsync(o => o.OrderId == OrderNo.Value);
                OrderNo = null;
                if (order != null)
                    await EditOrderModal!.Show(order);
                return;
            }

            if (!string.IsNullOrWhiteSpace(Query))
            {
                var normalized = C.Normalize(Query);
                var words = normalized.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                foreach (var word in words)
                {
                    var likeStr = $"%{word}%";
                    query = query.Where(o =>
                        EF.Functions.Like(o.ContactNameNormalized, likeStr) ||
                        EF.Functions.Like(o.ContactPhone!, likeStr) ||
                        EF.Functions.Like(o.DescriptionNormalized!, likeStr));
                }
            }
            else
            {
                if (From.HasValue)
                    query = query.Where(o => o.Arrival > From.Value);

                if (To.HasValue)
                    query = query.Where(o => o.Arrival < To.Value);
            }

            query = query.OrderBy(o => o.Arrival);

            var results = await query.Take(MAX).ToListAsync();
            _resultCount = results.Count;

            Orders.Clear();
            Efforts.Clear();
            foreach (var order in results)
            {
                if (!Orders.ContainsKey(order.Arrival.Date))
                {
                    Orders.Add(order.Arrival.Date, new());
                    Efforts.Add(order.Arrival.Date, 0);
                }

                Orders[order.Arrival.Date].Add(order);
                Efforts[order.Arrival.Date] += order.Effort;
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
        string GetStatusRowClass(Order order)
        {
            if (order.Removed)
                return "text-decoration-line-through table-secondary";
            if (order.Returned.HasValue)
                return "table-secondary";
            if (order.Arrival.Date < Today && !order.Arrived.HasValue)
                return "table-secondary";
            if (order.Arrival.Date == Today && !order.Arrived.HasValue)
                return "table-secondary";
            if (order.Arrival.Date > Today && !order.Arrived.HasValue)
                return "table-secondary";
            if (order.Arrived.HasValue && order.Arrived.Value.Date < Yesterday && !order.Completed.HasValue)
                return "table-danger";
            if (order.Arrived.HasValue && !order.Completed.HasValue)
                return "table-warning";
            if (order.Completed.HasValue)
                return "table-success";

            return string.Empty;
        }
        string GetStatusTitle(Order order)
        {
            if (order.Removed)
                return "Nije došao / odustao";
            if (order.Returned.HasValue)
                return "Vraćeno";
            if (order.Arrival.Date < Today && !order.Arrived.HasValue)
                return "Nije došao";
            if (order.Arrival.Date > Today && !order.Arrived.HasValue)
                return "U dolasku";
            if (order.Arrival.Date == Today && !order.Arrived.HasValue)
                return "Danas dolazi";
            if (order.Arrived.HasValue && order.Arrived.Value.Date < Yesterday && !order.Completed.HasValue)
                return "Nije završeno";
            if (order.Arrived.HasValue && !order.Completed.HasValue)
                return "U radu";
            if (order.Completed.HasValue)
                return "Završeno";

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
        static double GetEffortPercentage(int totalEffort)
        {
            var percentage = ((double)totalEffort / (double)C.MaxEffort) * 100;
            if (percentage > 100)
                percentage = 100;

            return percentage;
        }
        static string GetEffortStyle(double percentage)
        {
            if (percentage < 60)
                return $"width: {percentage}%; background-color: var(--bs-green);";
            if (percentage < 80)
                return $"width: {percentage}%; background-color: var(--bs-yellow);";

            return $"width: {percentage}%; background-color:var(--bs-red);";
        }
    }
}