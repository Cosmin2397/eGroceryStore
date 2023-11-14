using eGroceryStore.Areas.Data;
using eGroceryStore.Controllers;
using eGroceryStore.Data.Services;
using eGroceryStore.Data;
using eGroceryStore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace eGroceryStoreTests
{
    public class OrdersControllerTests
    {
        private Mock<IOrdersService> ordersServiceMock;
        private Mock<IProductsService> productsServiceMock;
        private Mock<IShoppingCart> shoppingCartMock;
        private Mock<UserManager<ApplicationUser>> userManagerMock;
        private OrdersController controller;

        private DbContextOptions<AppDbContext> CreateDbContextOptions(string databaseName)
        {
            return new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: databaseName)
                .Options;
        }

        private OrdersController CreateOrdersControllerWithUserAndMockServices(string userId)
        {
            var options = CreateDbContextOptions($"OrdersControllerTests_{userId}");
            userManagerMock = MockUserManagerWithUser(userId);
            controller = new OrdersController(MockProductsService(), MockShoppingCart(), MockOrdersServiceWithUser(userId), userManagerMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Name, userId) }, "mock")) }
                }
            };

            return controller;
        }

        private IProductsService MockProductsService()
        {
            var productServiceMock = new Mock<IProductsService>();
            productServiceMock.Setup(mock => mock.GetProductByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new Product { Id = 1, Name = "Product 1", Price = 10.0, ProductStock = 5 });

            return productServiceMock.Object;
        }

        private IShoppingCart MockShoppingCart()
        {
            var shoppingCartMock = new Mock<IShoppingCart>();
            shoppingCartMock.Setup(mock => mock.AddToCart(It.IsAny<Product>()));
            shoppingCartMock.Setup(mock => mock.ClearShoppingCartAsync());

            return shoppingCartMock.Object;
        }

        private IOrdersService MockOrdersServiceWithUser(string userId)
        {
            ordersServiceMock = new Mock<IOrdersService>();
            ordersServiceMock.Setup(mock => mock.GetOrdersByUserIdAsync(userId))
                .ReturnsAsync(new List<Order> { new Order { Id = 1, UserId = userId, Status = StatusEnum.Registred } });
            ordersServiceMock.Setup(mock => mock.GetOrdersByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) => new Order { Id = id, UserId = userId, Status = StatusEnum.Registred });
            ordersServiceMock.Setup(mock => mock.GetOrdersAsync())
                .ReturnsAsync(new List<Order> { new Order { Id = 1, UserId = "user1", Status = StatusEnum.Registred }, new Order { Id = 2, UserId = "user2", Status = StatusEnum.Registred } });
            ordersServiceMock.Setup(mock => mock.UpdateOrderAsync(It.IsAny<int>(), StatusEnum.Delivered))
            .Returns(Task.FromResult<object>(null));
            ordersServiceMock.Setup(mock => mock.StoreOrderAsync(It.IsAny<List<ShoppingCartItem>>(), userId, It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(true))
                .Callback(async (List<ShoppingCartItem> items, string userId, string userEmailAddress, string address) =>
                {
                    using (var context = new AppDbContext(CreateDbContextOptions($"StoreOrderAsync_{userId}")))
                    {
                        var order = new Order()
                        {
                            UserId = userId,
                            Email = userEmailAddress,
                            Status = StatusEnum.Registred,
                            Address = address,
                        };

                        await context.Orders.AddAsync(order);
                        await context.SaveChangesAsync();

                        foreach (var item in items)
                        {
                            var orderItem = new OrderItem()
                            {
                                Quantity = item.Quantity,
                                ProductId = item.Product.Id,
                                OrderId = order.Id,
                                Price = item.Product.Price
                            };
                            await context.OrderItems.AddAsync(orderItem);
                        }

                        await context.SaveChangesAsync();
                    }
                });

            return ordersServiceMock.Object;
        }

        private Mock<UserManager<ApplicationUser>> MockUserManagerWithUser(string userId)
        {
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            var userManagerMock = new Mock<UserManager<ApplicationUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            userManagerMock.Setup(mock => mock.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(new ApplicationUser { Id = userId });

            return userManagerMock;
        }


        [Fact]
        public async Task Index_ReturnsViewResult_WithOrdersForCurrentUser()
        {
            // Arrange
            var controller = CreateOrdersControllerWithUserAndMockServices("user1");

            // Act
            var result = await controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Order>>(viewResult.ViewData.Model);
            Assert.Single(model);
        }

        [Fact]
        public async Task GetOrderById_ReturnsViewResult_WhenOrderExists()
        {
            // Arrange
            var controller = CreateOrdersControllerWithUserAndMockServices("user1");

            // Act
            var result = await controller.GetOrderById(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Order>(viewResult.ViewData.Model);
            Assert.Equal(1, model.Id);
            Assert.Equal("user1", model.UserId);
        }

        [Fact]
        public async Task GetOrderById_ReturnsNotFoundResult_WhenOrderDoesNotExist()
        {
            // Arrange
            var controller = CreateOrdersControllerWithUserAndMockServices("user1");
            ordersServiceMock.Setup(mock => mock.GetOrdersByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) => null);

            // Act
            var result = await controller.GetOrderById(2);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
        }


        [Fact]
        public async Task GetAllOrders_ReturnsViewResult_WhenUserIsAdmin()
        {
            // Arrange
            var options = CreateDbContextOptions("GetAllOrders_ReturnsViewResult_WhenUserIsAdmin");
            var controller = new OrdersController(MockProductsService(), MockShoppingCart(), MockOrdersServiceWithUser("admin"), MockUserManagerWithUser("admin").Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Name, "admin"), new Claim(ClaimTypes.Role, "admin") }, "mock")) }
                }
            };

            // Act
            var result = await controller.GetAllOrders();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Order>>(viewResult.ViewData.Model);
            Assert.Equal(2, model.Count());
        }

        [Fact]
        public async Task ModifyOrderStatus_RedirectsToGetAllOrders_WhenModelStateIsValid()
        {
            // Arrange
            var controller = CreateOrdersControllerWithUserAndMockServices("admin");

            // Act
            var result = await controller.ModifyOrderStatus(1, StatusEnum.Delivered.ToString());

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("GetAllOrders", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task ShoppingCart_AddItemToShoppingCart_RedirectsToShoppingCart()
        {
            // Arrange
            var controller = new OrdersController(MockProductsService(), MockShoppingCart(), null, null);

            // Act
            var result = await controller.AddItemToShoppingCart(1);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ShoppingCart", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task RemoveItemFromShoppingCart_RedirectsToShoppingCart()
        {
            // Arrange
            var controller = new OrdersController(MockProductsService(), MockShoppingCart(), null, null);

            // Act
            var result = await controller.RemoveItemFromShoppingCart(1);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ShoppingCart", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task CompleteOrder_ReturnsViewResult_OrderComplete_Result()
        {
            // Arrange
            var controller = CreateOrdersControllerWithUserAndMockServices("user1");

            // Act
            var result = await controller.CompleteOrder();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("OrderCompleted", viewResult.ViewName);
        }
    }
}