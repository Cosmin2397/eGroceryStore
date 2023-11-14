using eGroceryStore.Data.Services;

namespace eGroceryStore.ViewModels
{
    public class ShoppingCartViewModel
    {
        public IShoppingCart ShoppingCart { get; set; }

        public double ShoppingCartTotal { get; set; }
    }
}
