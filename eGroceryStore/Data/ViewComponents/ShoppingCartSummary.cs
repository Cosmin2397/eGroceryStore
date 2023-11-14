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

        public IViewComponentResult Invoke()
        {
            var items = _shoppingCart.GetShoppingCartItems();
            var totalItems = items.Count;

            return View(totalItems);
        }
    }
}
