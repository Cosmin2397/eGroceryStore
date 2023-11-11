using eGroceryStore.Models;

namespace eGroceryStore.Data.Services
{
    public interface IOrdersService
    {
        Task StoreOrderAsync(List<ShoppingCartItem> items, string userId, string userEmailAddress, string address);
        Task<List<Order>> GetOrdersByUserIdAsync(string userId);
        Task<List<Order>> GetOrdersAsync();
        Task<Order> GetOrdersByIdAsync(int id);
    }
}
