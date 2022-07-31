using eGroceryStore.Data;
using Microsoft.AspNetCore.Mvc;

namespace eGroceryStore.ViewComponents
{
        [ViewComponent(Name = "Category")]
        public class CategoryViewComponent : ViewComponent
        {
            private readonly AppDbContext _context;
            public CategoryViewComponent(AppDbContext applicationDbContext)
            {
                _context = applicationDbContext;
            }

            public async Task<IViewComponentResult> InvokeAsync()
            {
                return   View("Index", _context.Categories.ToList());
            }
        }
}
