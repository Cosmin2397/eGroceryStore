using eGroceryStore.Controllers;
using eGroceryStore.Data;
using eGroceryStore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Security.Claims;

namespace eGroceryStoreTests
{
    public class ProductsControllerTests
    {
        private DbContextOptions<AppDbContext> CreateDbContextOptions(string databaseName)
        {
            return new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: databaseName)
                .Options;
        }

        private void PopulateTestData(AppDbContext context)
        {
            var brand = new Brand { Id = 1, Name = "Brand 1" };
            var category = new Category { Id = 1, Name = "Category 1" };

            context.Products.Add(new Product { Id = 1, Name = "Product 1", Price = 10.0, ProductStock = 5, BrandId = brand.Id, CategoryId = category.Id, Brand = brand, Category = category });
            context.Products.Add(new Product { Id = 2, Name = "Product 2", Price = 15.0, ProductStock = 10, BrandId = brand.Id, CategoryId = category.Id, Brand = brand, Category = category });

            context.SaveChanges();
        }

        private ProductsController CreateControllerWithTestData()
        {
            var options = CreateDbContextOptions("Index_ReturnsAViewResult_WithAListOfProducts");

            using (var context = new AppDbContext(options))
            {
                PopulateTestData(context);
            }

            var contextForController = new AppDbContext(options);
            return new ProductsController(contextForController);
        }

        private ProductsController CreateControllerWithAdminUser()
        {
            var options = CreateDbContextOptions("ProductsList_ReturnsViewResult_WhenUserIsAdmin");

            using (var context = new AppDbContext(options))
            {
                PopulateTestData(context);
            }

            var contextForController = new AppDbContext(options);
            var controller = new ProductsController(contextForController);
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
            new Claim(ClaimTypes.Name, "admin"),
            new Claim(ClaimTypes.Role, "admin"),
            }, "mock"));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            return controller;
        }

        [Fact]
        public async Task Index_ReturnsAViewResult_WithAListOfProducts()
        {
            // Arrange
            var controller = CreateControllerWithTestData();

            // Act
            var result = await controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Product>>(viewResult.ViewData.Model);
            Assert.Equal(2, model.Count());
            var productNames = model.Select(p => p.Name).ToList();
            Assert.Contains("Product 1", productNames);
            Assert.Contains("Product 2", productNames);
        }

        [Fact]
        public async Task ProductsList_ReturnsViewResult_WhenUserIsAdmin()
        {
            // Arrange
            var controller = CreateControllerWithAdminUser();

            // Act
            var result = await controller.ProductsList();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Product>>(viewResult.ViewData.Model);
            Assert.Equal(2, model.Count());
        }

        [Fact]
        public async Task Details_ReturnsViewResult_WhenProductExists()
        {
            // Arrange
            var options = CreateDbContextOptions("Details_ReturnsViewResult_WhenProductExists");

            using (var context = new AppDbContext(options))
            {
                // Populate the in-memory database with test data
                var brand = new Brand { Id = 1, Name = "Brand 1" };
                var category = new Category { Id = 1, Name = "Category 1" };

                context.Products.Add(new Product { Id = 1, Name = "Product 1", Price = 10.0, ProductStock = 5, BrandId = brand.Id, CategoryId = category.Id, Brand = brand, Category = category });
                context.SaveChanges();
            }

            using (var context = new AppDbContext(options))
            {
                var controller = new ProductsController(context);

                // Act
                var result = await controller.Details(1);

                // Assert
                var viewResult = Assert.IsType<ViewResult>(result);
                var model = Assert.IsType<Product>(viewResult.ViewData.Model);
                Assert.Equal("Product 1", model.Name);
                Assert.Equal(10.0, model.Price);
                Assert.Equal(5, model.ProductStock);
                Assert.Equal(1, model.BrandId);
                Assert.Equal(1, model.CategoryId);
            }
        }

        [Fact]
        public async Task Details_ReturnsNotFoundResult_WhenProductDoesNotExist()
        {
            // Arrange
            var options = CreateDbContextOptions("Details_ReturnsNotFoundResult_WhenProductDoesNotExist");

            using (var context = new AppDbContext(options))
            {
                // Check when products doesn't exist!
            }

            using (var context = new AppDbContext(options))
            {
                var controller = new ProductsController(context);

                // Act
                var result = await controller.Details(1);

                // Assert
                Assert.IsType<NotFoundResult>(result);
            }
        }
    }
}