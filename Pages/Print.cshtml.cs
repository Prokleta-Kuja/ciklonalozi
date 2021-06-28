using System.Threading.Tasks;
using ciklonalozi.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ciklonalozi.Pages
{
    public class Print : PageModel
    {
        public Order? Order { get; private set; }
        private readonly IDbContextFactory<AppDbContext> _dbFactory;

        public Print(IDbContextFactory<AppDbContext> factory)
        {
            _dbFactory = factory;
        }
        public async Task OnGet(int id)
        {
            using var db = _dbFactory.CreateDbContext();
            Order = await db.Orders.FindAsync(id);
        }
    }
}