using System.Threading.Tasks;
using ciklonalozi.Data;
using HashidsNet;
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
        private readonly Hashids _ids;
        public Qr(IDbContextFactory<AppDbContext> factory)
        {
            _dbFactory = factory;
            _ids = new Hashids(C.Env.SALT, 4, C.Env.ALPHABET);
        }
        public async Task OnGet(string id)
        {
            int orderId;
            try
            {
                orderId = _ids.DecodeSingle(id.ToUpper());
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