using eGroceryStore.Models;
using Microsoft.EntityFrameworkCore;

namespace eGroceryStore.Data.Services
{
    public interface IShoppingCart
    {
        string ShoppingCartId { get; set; }
        List<ShoppingCartItem> ShoppingCartItems { get; set; }

        void AddToCart(Product product);
        void RemoveFromCart(Product product);
        List<ShoppingCartItem> GetShoppingCartItems();
        double GetShoppingCartTotal();
        Task ClearShoppingCartAsync();
    }
}
