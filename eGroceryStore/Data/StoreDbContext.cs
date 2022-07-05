using eGroceryStore.Models;
using Microsoft.EntityFrameworkCore;

namespace eGroceryStore.Data
{
    public class StoreDbContext : DbContext
    {
        public StoreDbContext(DbContextOptions<StoreDbContext> options) 
        : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }

        public DbSet<Brand> Brands { get; set; }

        public DbSet<Category> Categories { get; set; }
    }
}
