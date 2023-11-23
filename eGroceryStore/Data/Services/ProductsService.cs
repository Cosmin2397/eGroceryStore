using eGroceryStore.Data.Base;
using eGroceryStore.Models;
using Microsoft.EntityFrameworkCore;

namespace eGroceryStore.Data.Services
{
    public class ProductsService : EntityBaseRepository<Product>, IProductsService
    {
        private readonly AppDbContext _context;

        public ProductsService(AppDbContext context) : base(context)
        {
            _context = context;
        }

        // Retrieves product details by its ID along with associated brand and category
        public async Task<Product> GetProductByIdAsync(int id)
        {
            var productDetails = await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .FirstOrDefaultAsync(n => n.Id == id);

            return productDetails;
        }
    }
}
