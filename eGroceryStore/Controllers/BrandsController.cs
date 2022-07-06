using eGroceryStore.Data;
using Microsoft.AspNetCore.Mvc;

namespace eGroceryStore.Controllers
{
    public class BrandsController : Controller
    {
        private readonly AppDbContext _context;

        public BrandsController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var data = _context.Brands.ToList();
            return View();
        }
    }
}
