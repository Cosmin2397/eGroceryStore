using eGroceryStore.Data.Base;
using eGroceryStore.Models;

namespace eGroceryStore.Data.Services
{
    public interface IProductsService : IEntityBaseRepository<Product>
    {
        Task<Product> GetProductByIdAsync(int id);
    }
}
