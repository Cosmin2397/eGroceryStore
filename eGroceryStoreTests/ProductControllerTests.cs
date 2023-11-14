using eGroceryStore.Controllers;
using eGroceryStore.Data;
using eGroceryStore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

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
            if (!context.Products.Any())
            {
                var brand = new Brand { Id = 1, Name = "Brand 1" };
                var category = new Category { Id = 2, Name = "Category 2" };

                context.Products.Add(new Product { Id = 1, Name = "Product 1", Price = 10.0, ProductStock = 5, BrandId = brand.Id, CategoryId = category.Id, Brand = brand, Category = category });
                context.Products.Add(new Product { Id = 2, Name = "Product 2", Price = 15.0, ProductStock = 10, BrandId = brand.Id, CategoryId = category.Id, Brand = brand, Category = category });

                context.SaveChanges();
            }
        }


        private ProductsController CreateControllerWithTestData(DbContextOptions<AppDbContext> options)
        {
            using (var context = new AppDbContext(options))
            {
                PopulateTestData(context);
            }

            var contextForController = new AppDbContext(options);
            return new ProductsController(contextForController);
        }

        private ProductsController CreateControllerWithAdminUser(DbContextOptions<AppDbContext> options)
        {
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
            var options = CreateDbContextOptions("Index_ReturnsAViewResult_WithAListOfProducts");

            using (var context = new AppDbContext(options))
            {
                PopulateTestData(context);
            }

            var controller = new ProductsController(new AppDbContext(options));

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
            var options = CreateDbContextOptions("ProductsList_ReturnsViewResult_WhenUserIsAdmin");

            using (var context = new AppDbContext(options))
            {
                PopulateTestData(context);

                var controller = CreateControllerWithAdminUser(options);

                // Act
                var result = await controller.ProductsList();

                // Assert
                var viewResult = Assert.IsType<ViewResult>(result);
                var model = Assert.IsAssignableFrom<IEnumerable<Product>>(viewResult.ViewData.Model);
                Assert.Equal(2, model.Count());
            }
        }

        [Fact]
        public async Task Details_ReturnsViewResult_WhenProductExists()
        {
            // Arrange
            var options = CreateDbContextOptions("Details_ReturnsViewResult_WhenProductExists");

            var controller = CreateControllerWithTestData(options);

            // Act
            var result = await controller.Details(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Product>(viewResult.ViewData.Model);
            Assert.Equal("Product 1", model.Name);
            Assert.Equal(10.0, model.Price);
            Assert.Equal(5, model.ProductStock);
            Assert.Equal(1, model.BrandId);
            Assert.Equal(2, model.CategoryId);
        }

        [Fact]
        public async Task Details_ReturnsNotFoundResult_WhenProductDoesNotExist()
        {
            // Arrange
            var options = CreateDbContextOptions("Details_ReturnsNotFoundResult_WhenProductDoesNotExist");

            var controller = CreateControllerWithTestData(options);

            // Act
            var result = await controller.Details(3);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Create_GET_ReturnsViewResult_WhenUserIsAdmin()
        {
            // Arrange
            var options = CreateDbContextOptions("Create_GET_ReturnsViewResult_WhenUserIsAdmin");

            var controller = CreateControllerWithAdminUser(options);

            // Act
            var result = controller.Create();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.NotNull(viewResult.ViewData["BrandId"]);
            Assert.NotNull(viewResult.ViewData["CategoryId"]);
        }

        [Fact]
        public async Task Create_POST_RedirectsToIndex_WhenModelStateIsValid()
        {
            // Arrange
            var options = CreateDbContextOptions("Create_POST_RedirectsToIndex_WhenModelStateIsValid");

            using (var context = new AppDbContext(options))
            {
                var controller = new ProductsController(context);
                var product = new Product
                {
                    Name = "New Product",
                    Price = 20.0,
                    ProductStock = 10,
                    BrandId = 1,
                    CategoryId = 2,
                };

                // Act
                var result = await controller.Create(product);

                // Assert
                var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
                Assert.Equal("Index", redirectToActionResult.ActionName);
            }
        }

        [Fact]
        public async Task Edit_POST_RedirectsToIndex_WhenModelStateIsValid()
        {
            // Arrange
            var options = CreateDbContextOptions("Edit_POST_RedirectsToIndex_WhenModelStateIsValid1");

            using (var context = new AppDbContext(options))
            {
                PopulateTestData(context);
                var controller = CreateControllerWithAdminUser(options);

                var product = new Product
                {
                    Id = 1,
                    Name = "Edited Product",
                    Price = 25.0,
                    ProductStock = 15,
                    BrandId = 1,
                    CategoryId = 2,
                };

                // Act
                var result = await controller.Edit(1, product);

                // Assert
                var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
                Assert.Equal("Index", redirectToActionResult.ActionName);
            }
        }


        [Fact]
        public async Task Edit_GET_ReturnsNotFoundResult_WhenProductIsNull()
        {
            // Arrange
            var options = CreateDbContextOptions("Edit_GET_ReturnsNotFoundResult_WhenProductIsNull");

            using (var context = new AppDbContext(options))
            {
                PopulateTestData(context);
            }

            var controller = CreateControllerWithAdminUser(options);
            var product = new Product
            {
                Id = 2,
                Name = "Edited Product",
                Price = 25.0,
                ProductStock = 15,
                BrandId = 1,
                CategoryId = 2,
            };
            // Act
            var result = await controller.Edit(99, product);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }


        [Fact]
        public async Task Edit_POST_ReturnsNotFoundResult_WhenIdDoesNotMatchProduct()
        {
            // Arrange
            var options = CreateDbContextOptions("Edit_POST_ReturnsNotFoundResult_WhenIdDoesNotMatchProduct");


            var controller = CreateControllerWithAdminUser(options);
            var product = new Product
            {
                Id = 2,
                Name = "Edited Product",
                Price = 25.0,
                ProductStock = 15,
                BrandId = 1,
                CategoryId = 2,
            };

            // Act
            var result = await controller.Edit(1, product);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsViewResult_WhenProductExists()
        {
            // Arrange
            var options = CreateDbContextOptions("Delete_ReturnsViewResult_WhenProductExists");

            using (var context = new AppDbContext(options))
            {
                PopulateTestData(context);
                var controller = CreateControllerWithAdminUser(options);

                // Act
                var result = await controller.Delete(1);

                // Assert
                var viewResult = Assert.IsType<ViewResult>(result);
                var model = Assert.IsType<Product>(viewResult.ViewData.Model);
                Assert.Equal("Product 1", model.Name);
                Assert.Equal(10.0, model.Price);
                Assert.Equal(5, model.ProductStock);
                Assert.Equal(1, model.BrandId);
                Assert.Equal(2, model.CategoryId);
            }
        }

        [Fact]
        public async Task Delete_ReturnsNotFoundResult_WhenProductDoesNotExist()
        {
            // Arrange
            var options = CreateDbContextOptions("Delete_ReturnsNotFoundResult_WhenProductDoesNotExist");

            var controller = CreateControllerWithAdminUser(options);

            // Act
            var result = await controller.Delete(3);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteConfirmed_RemovesProductAndRedirectsToIndex()
        {
            // Arrange
            var options = CreateDbContextOptions("DeleteConfirmed_RemovesProductAndRedirectsToIndex");

            var controller = CreateControllerWithAdminUser(options);

            // Act
            var result = await controller.DeleteConfirmed(1);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);

            using (var context = new AppDbContext(options))
            {
                Assert.Null(context.Products.Find(1));
            }
        }

        [Fact]
        public async Task GetCategoryProducts_ReturnsViewResult_WhenCategoryExists()
        {
            // Arrange
            var options = CreateDbContextOptions("GetCategoryProducts_ReturnsViewResult_WhenCategoryExists");

            using (var context = new AppDbContext(options))
            {
                PopulateTestData(context);
            }

            var controller = new ProductsController(new AppDbContext(options));

            // Act
            var result = await controller.GetCategoryProducts(2);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<Product>>(viewResult.ViewData.Model);
            Assert.Equal(2, model.Count);
        }

        [Fact]
        public async Task GetCategoryProducts_ReturnsNotFoundResult_WhenCategoryDoesNotExist()
        {
            // Arrange
            var options = CreateDbContextOptions("GetCategoryProducts_ReturnsNotFoundResult_WhenCategoryDoesNotExist");
            var controller = new ProductsController(new AppDbContext(options));

            // Act
            var result = await controller.GetCategoryProducts(99);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetBrandProducts_ReturnsViewResult_WhenBrandExists()
        {
            // Arrange
            var options = CreateDbContextOptions("GetBrandProducts_ReturnsViewResult_WhenBrandExists");

            using (var context = new AppDbContext(options))
            {
                PopulateTestData(context);
            }

            var controller = new ProductsController(new AppDbContext(options));

            // Act
            var result = await controller.GetBrandProducts(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<Product>>(viewResult.ViewData.Model);
            Assert.Equal(2, model.Count);
        }

        [Fact]
        public async Task GetBrandProducts_ReturnsNotFoundResult_WhenBrandDoesNotExist()
        {
            // Arrange
            var options = CreateDbContextOptions("GetBrandProducts_ReturnsNotFoundResult_WhenBrandDoesNotExist");

            var controller = new ProductsController(new AppDbContext(options));

            // Act
            var result = await controller.GetBrandProducts(99);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
