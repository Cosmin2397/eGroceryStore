using eGroceryStore.Areas.Data;
using eGroceryStore.Data;
using eGroceryStore.Data.Services;
using eGroceryStore.Models;
using eGroceryStore.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace eGroceryStore.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ShoppingCart _shoppingCart;
        private readonly IProductsService _productsService;
        private readonly IOrdersService _ordersService;
        private readonly UserManager<ApplicationUser> _userManager;
        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        public OrdersController(IProductsService productsService, ShoppingCart shoppingCart, IOrdersService ordersService, UserManager<ApplicationUser> userManager)
        {
            _productsService = productsService;
            _shoppingCart = shoppingCart;
            _ordersService = ordersService;
            _userManager = userManager;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            ApplicationUser user = await GetCurrentUserAsync();
            var currentUserId = user.Id;
            var orders = await _ordersService.GetOrdersByUserIdAsync(currentUserId);
            return View(orders);    
        }

        [Authorize]
        public async Task<IActionResult> GetOrderByUserId(string? user)
        {
            var orders = await _ordersService.GetOrdersByUserIdAsync(user);
            return View(orders);
        }

        [Authorize]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var order = await _ordersService.GetOrdersByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAllOrders()
        {

            var orders = await _ordersService.GetOrdersAsync();
            return View(orders);
        }

        [Authorize]
        public IActionResult ShoppingCart()
        {
            var items = _shoppingCart.GetShoppingCartItems();
            _shoppingCart.ShoppingCartItems = items;

            var response = new ShoppingCartViewModel()
            {
                ShoppingCart = _shoppingCart,
                ShoppingCartTotal = _shoppingCart.GetShoppingCartTotal()
            };

            return View(response);
        }

        [Authorize]
        public async Task<IActionResult> AddItemToShoppingCart(int id)
        {
            var item = await _productsService.GetProductByIdAsync(id);

            if (item != null && item.ProductStock > 0)
            {
                _shoppingCart.AddToCart(item);
            }
            return RedirectToAction(nameof(ShoppingCart));
        }

        [Authorize]
        public async Task<IActionResult> RemoveItemFromShoppingCart(int id)
        {
            var item = await _productsService.GetProductByIdAsync(id);

            if (item != null)
            {
                _shoppingCart.RemoveFromCart(item);
            }
            return RedirectToAction(nameof(ShoppingCart));
        }

        [Authorize]
        public async Task<IActionResult> CompleteOrder()
        {
            var items = _shoppingCart.GetShoppingCartItems();
            var user = await GetCurrentUserAsync();
            string userId = user.Id;
            string userEmailAddress = user.Email;
            string address = user.Address;

            await _ordersService.StoreOrderAsync(items, userId, userEmailAddress, address);
            await _shoppingCart.ClearShoppingCartAsync();

            return View("OrderCompleted");
        }
    }
}
