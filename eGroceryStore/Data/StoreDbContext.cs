using Microsoft.EntityFrameworkCore;

namespace eGroceryStore.Data
{
    public class StoreDbContext : DbContext
    {
        public StoreDbContext(DbContextOptions<StoreDbContext> options) 
        : base(options)
        {

        }
    }
}
