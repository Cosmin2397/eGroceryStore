using eGroceryStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace eGroceryStore.Data.Services
{
    public class OrdersService : IOrdersService
    {
        private readonly AppDbContext _context;
        public OrdersService(AppDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "admin")]
        public async Task<List<Order>> GetOrdersAsync()
        {
            var orders = await _context.Orders.Include(n => n.OrderItems).ThenInclude(n => n.Product).ToListAsync();
            return orders;
        }

        public async Task<List<Order>> GetOrdersByUserIdAsync(string userId)
        {
            var orders = await _context.Orders.Include(n => n.OrderItems).ThenInclude(n => n.Product).Where(n => n.UserId == userId).ToListAsync();
            return orders;
        }

        public async Task<Order> GetOrdersByIdAsync(int id)
        {
            var order = await _context.Orders.Include(n => n.OrderItems).ThenInclude(n => n.Product).FirstOrDefaultAsync(n => n.Id == id);
            return order;
        }

        public async Task StoreOrderAsync(List<ShoppingCartItem> items, string userId, string userEmailAddress, string address)
        {
            var order = new Order()
            {
                UserId = userId,
                Email = userEmailAddress,
                Status = StatusEnum.Registred,
                Address = address,
            };
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            foreach (var item in items)
            {
                var orderItem = new OrderItem()
                {
                    Quantity = item.Quantity,
                    ProductId = item.Product.Id,
                    OrderId = order.Id,
                    Price = item.Product.Price
                };
                await _context.OrderItems.AddAsync(orderItem);
            }
            await _context.SaveChangesAsync();
        }
    }
}
