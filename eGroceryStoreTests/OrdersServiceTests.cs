using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using eGroceryStore.Data.Services;
using eGroceryStore.Models;
using global::eGroceryStore.Data.Services;
using global::eGroceryStore.Data;
using global::eGroceryStore.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace eGroceryStore.Tests
{
    public class OrdersServiceTests
    {
        private readonly DbContextOptions<AppDbContext> _dbContextOptions;

        public OrdersServiceTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "OrdersServiceTestsDb")
                .Options;
        }

        private async Task SeedDataAsync()
        {
            using (var context = new AppDbContext(_dbContextOptions))
            {
                var product = new Product { Id = 1, Name = "Product 1", Price = 10.0, ProductStock = 5 };
                await context.Products.AddAsync(product);

                var order = new Order { Id = 1, UserId = "user1", Status = StatusEnum.Registred, Address = "Address", Email = "email@mail.com" };

                order.OrderItems = new List<OrderItem>();

                var orderItem = new OrderItem { Quantity = 2, ProductId = 1, OrderId = 1, Price = product.Price, Id = 1, Order = order, Product = product };

                order.OrderItems.Add(orderItem);

                await context.Orders.AddAsync(order);
                await context.SaveChangesAsync();
            }
        }

        [Fact]
        public async Task GetOrdersByUserIdAsync_ReturnsUserOrders()
        {
            // Arrange
            using (var context = new AppDbContext(_dbContextOptions))
            {
                var ordersService = new OrdersService(context);

                // Act
                await SeedDataAsync(); // Ensure proper await
                var orders = await ordersService.GetOrdersByUserIdAsync("user1");

                // Assert
                Assert.Single(orders);
                Assert.Equal("user1", orders[0].UserId);
            }
        }



        [Fact]
        public async Task GetOrdersByIdAsync_ReturnsOrderById()
        {
            // Arrange
            using (var context = new AppDbContext(_dbContextOptions))
            {
                var ordersService = new OrdersService(context);

                // Act
                var order = await ordersService.GetOrdersByIdAsync(1);

                // Assert
                Assert.NotNull(order);
                Assert.Equal(1, order.Id);
            }
        }

        [Fact]
        public async Task GetOrdersByIdAsync_ReturnsNull_WhenOrderDoesNotExist()
        {
            // Arrange
            using (var context = new AppDbContext(_dbContextOptions))
            {
                var ordersService = new OrdersService(context);

                // Act
                var order = await ordersService.GetOrdersByIdAsync(4);

                // Assert
                Assert.Null(order);
            }
        }

        [Fact]
        public async Task StoreOrderAsync_AddsOrderToDatabase()
        {
            // Arrange
            using (var context = new AppDbContext(_dbContextOptions))
            {
                var ordersService = new OrdersService(context);
                var items = new List<ShoppingCartItem> { new ShoppingCartItem { Product = new Product { Id = 2, Name = "Product 1", Price = 10.0 }, Quantity = 2 } };

                // Act
                await ordersService.StoreOrderAsync(items, "user2", "user@example.com", "123 Main St");

                // Assert
                var orders = await context.Orders.ToListAsync();
                var order = orders[1];
                Assert.NotNull(order);
                Assert.Equal("user2", order.UserId);
                Assert.Equal(StatusEnum.Registred, order.Status);
                Assert.Equal("user@example.com", order.Email);
                Assert.Equal("123 Main St", order.Address);
            }
        }

        [Fact]
        public async Task UpdateOrderAsync_UpdatesOrderStatus()
        {
            // Arrange
            using (var context = new AppDbContext(_dbContextOptions))
            {
                var ordersService = new OrdersService(context);

                // Act
                await ordersService.UpdateOrderAsync(1, StatusEnum.Delivered);

                // Assert
                var order = await context.Orders.FindAsync(1);
                Assert.NotNull(order);
                Assert.Equal(StatusEnum.Delivered, order.Status);
            }
        }

        [Fact]
        public async Task UpdateOrderAsync_ThrowsException_WhenOrderNotFound()
        {
            // Arrange
            using (var context = new AppDbContext(_dbContextOptions))
            {
                var ordersService = new OrdersService(context);

                // Act/Assert
                await Assert.ThrowsAsync<ApplicationException>(() => ordersService.UpdateOrderAsync(4, StatusEnum.Delivered));
            }
        }


        [Fact]
        public async Task GetOrdersAsync_ReturnsAllOrders_WhenUserIsAdmin()
        {
            // Arrange
            using (var context = new AppDbContext(_dbContextOptions))
            {
                var ordersService = new OrdersService(context);

                // Act
                var orders = await ordersService.GetOrdersAsync();

                // Assert
                Assert.Equal(orders.Count, 2);
            }
        }
    }
}
