using System.Threading.Tasks;
using ciklonalozi.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ciklonalozi.Pages
{
    [AllowAnonymous]
    public class Qr : PageModel
    {
        public Order? Order { get; private set; }
        public bool Fail { get; private set; }
        private readonly IDbContextFactory<AppDbContext> _dbFactory;
        public Qr(IDbContextFactory<AppDbContext> factory)
        {
            _dbFactory = factory;
        }
        public async Task OnGet(string id)
        {
            int orderId;
            try
            {
                orderId = C.Hasher.Ids.DecodeSingle(id.ToUpper());
            }
            catch (System.Exception)
            {
                Fail = true;
                return;
            }

            using var db = _dbFactory.CreateDbContext();
            Order = await db.Orders.FindAsync(orderId);
        }
        public async Task OnPost(string id, string? e, string? p, string? a)
        {
            int orderId;
            try
            {
                orderId = C.Hasher.Ids.DecodeSingle(id.ToUpper());
            }
            catch (System.Exception)
            {
                Fail = true;
                return;
            }

            using var db = _dbFactory.CreateDbContext();
            Order = await db.Orders.FindAsync(orderId);
        }
    }
}