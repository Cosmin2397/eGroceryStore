using eGroceryStore.Data;
using Microsoft.AspNetCore.Mvc;

namespace eGroceryStore.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly AppDbContext _context;

        public CategoriesController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var data = _context.Categories.ToList();
            return View();
        }
    }
}
