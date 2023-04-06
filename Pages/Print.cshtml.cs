using System;
using System.Threading.Tasks;
using ciklonalozi.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ciklonalozi.Pages
{
    public class Print : PageModel
    {
        public Order? Order { get; private set; }
        public string? OrderLink { get; set; }
        private readonly IDbContextFactory<AppDbContext> _dbFactory;

        public Print(IDbContextFactory<AppDbContext> factory)
        {
            _dbFactory = factory;
        }
        public async Task OnGet(int id)
        {
            OrderLink = C.Hasher.GetQrUrl(id);

            using var db = _dbFactory.CreateDbContext();
            Order = await db.Orders.FindAsync(id);

            if (Order == null)
                return;

            // Adjust for TZ
            Order.Arrival = TimeZoneInfo.ConvertTimeFromUtc(Order.Arrival, C.TZ);
            if (Order.Arrived.HasValue)
                Order.Arrived = TimeZoneInfo.ConvertTimeFromUtc(Order.Arrived.Value, C.TZ);
            if (Order.Completed.HasValue)
                Order.Completed = TimeZoneInfo.ConvertTimeFromUtc(Order.Completed.Value, C.TZ);
            if (Order.Returned.HasValue)
                Order.Returned = TimeZoneInfo.ConvertTimeFromUtc(Order.Returned.Value, C.TZ);
        }
    }
}