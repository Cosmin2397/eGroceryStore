using eGroceryStore.Data.Services;
using Microsoft.AspNetCore.Mvc;

namespace eGroceryStore.Data.ViewComponents
{
    public class ShoppingCartSummary : ViewComponent
    {
        private readonly IShoppingCart _shoppingCart;

        public ShoppingCartSummary(IShoppingCart shoppingCart)
        {
            _shoppingCart = shoppingCart;
        }

        // Invokes the view component to display the shopping cart summary
        public IViewComponentResult Invoke()
        {
            // Retrieves shopping cart items and calculates total items
            var items = _shoppingCart.GetShoppingCartItems();
            var totalItems = items.Count;

            // Returns the view with the total number of items in the shopping cart
            return View(totalItems);
        }
    }
}

