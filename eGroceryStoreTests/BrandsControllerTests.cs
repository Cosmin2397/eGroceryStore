using eGroceryStore.Controllers;
using eGroceryStore.Data;
using eGroceryStore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace eGroceryStoreTests
{
    public class BrandsControllerTests
    {
        private DbContextOptions<AppDbContext> CreateDbContextOptions(string databaseName)
        {
            return new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: databaseName)
                .Options;
        }

        private void PopulateTestData(AppDbContext context)
        {
            context.Brands.Add(new Brand { Id = 1, Name = "Brand 1", LogoUrl = "logo1.png", Description = "Description 1" });
            context.Brands.Add(new Brand { Id = 2, Name = "Brand 2", LogoUrl = "logo2.png", Description = "Description 2" });

            context.SaveChanges();
        }

        private BrandsController CreateControllerWithAdminUser(DbContextOptions<AppDbContext> options)
        {
            using (var context = new AppDbContext(options))
            {
                PopulateTestData(context);
            }

            var contextForController = new AppDbContext(options);
            var controller = new BrandsController(contextForController);
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
        public async Task Index_ReturnsViewResult_WithAListOfBrands()
        {
            // Arrange
            var options = CreateDbContextOptions("Index_ReturnsViewResult_WithAListOfBrands");

            var controller = new BrandsController(new AppDbContext(options));

            // Act
            var result = await controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Brand>>(viewResult.ViewData.Model);
            Assert.Equal(0, model.Count()); // No brands in the empty in-memory database
        }

        [Fact]
        public async Task BrandsList_ReturnsViewResult_WhenUserIsAdmin()
        {
            // Arrange
            var options = CreateDbContextOptions("BrandsList_ReturnsViewResult_WhenUserIsAdmin");

            var controller = CreateControllerWithAdminUser(options);

            // Act
            var result = await controller.BrandsList();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Brand>>(viewResult.ViewData.Model);
            Assert.Equal(2, model.Count()); // Two brands in the in-memory database
        }

        [Fact]
        public async Task Details_ReturnsViewResult_WhenBrandExists()
        {
            // Arrange
            var options = CreateDbContextOptions("Details_ReturnsViewResult_WhenBrandExists");
            using (var context = new AppDbContext(options))
            {
                PopulateTestData(context);
            }
            var controller = new BrandsController(new AppDbContext(options));

            // Act
            var result = await controller.Details(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Brand>(viewResult.ViewData.Model);
            Assert.Equal("Brand 1", model.Name);
            Assert.Equal("logo1.png", model.LogoUrl);
            Assert.Equal("Description 1", model.Description);
        }

        [Fact]
        public async Task Details_ReturnsNotFoundResult_WhenBrandDoesNotExist()
        {
            // Arrange
            var options = CreateDbContextOptions("Details_ReturnsNotFoundResult_WhenBrandDoesNotExist");

            var controller = new BrandsController(new AppDbContext(options));

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
        }

        [Fact]
        public async Task Create_POST_RedirectsToIndex_WhenModelStateIsValid()
        {
            // Arrange
            var options = CreateDbContextOptions("Create_POST_RedirectsToIndex_WhenModelStateIsValid");

            using (var context = new AppDbContext(options))
            {
                var controller = new BrandsController(context);
                var brand = new Brand
                {
                    Name = "New Brand",
                    LogoUrl = "newlogo.png",
                    Description = "New Description",
                };

                // Act
                var result = await controller.Create(brand);

                // Assert
                var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
                Assert.Equal("Index", redirectToActionResult.ActionName);
            }
        }

        [Fact]
        public async Task Edit_GET_ReturnsViewResult_WhenUserIsAdmin()
        {
            // Arrange
            var options = CreateDbContextOptions("Edit_GET_ReturnsViewResult_WhenUserIsAdmin");

            var controller = CreateControllerWithAdminUser(options);

            // Act
            var result = await controller.Edit(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Brand>(viewResult.ViewData.Model);
            Assert.Equal("Brand 1", model.Name);
        }

        [Fact]
        public async Task Edit_GET_ReturnsNotFoundResult_WhenIdIsNull()
        {
            // Arrange
            var options = CreateDbContextOptions("Edit_GET_ReturnsNotFoundResult_WhenIdIsNull");

            var controller = CreateControllerWithAdminUser(options);

            // Act
            var result = await controller.Edit(null);

            // Assert
            Assert.IsType<NotFoundResult>(result);

        }

        [Fact]
        public async Task Edit_GET_ReturnsNotFoundResult_WhenBrandIsNull()
        {
            // Arrange
            var options = CreateDbContextOptions("Edit_GET_ReturnsNotFoundResult_WhenBrandIsNull");


                var controller = CreateControllerWithAdminUser(options);

                // Act
                var result = await controller.Edit(99);

                // Assert
                Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_POST_RedirectsToIndex_WhenModelStateIsValid()
        {
            // Arrange
            var options = CreateDbContextOptions("Edit_POST_RedirectsToIndex_WhenModelStateIsValid");


            var controller = CreateControllerWithAdminUser(options);
            var brand = new Brand
            {
                Id = 1,
                Name = "Edited Brand",
                LogoUrl = "editedlogo.png",
                Description = "Edited Description",
            };

            // Act
            var result = await controller.Edit(1, brand);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task Edit_POST_ReturnsNotFoundResult_WhenIdDoesNotMatchBrand()
        {
            // Arrange
            var options = CreateDbContextOptions("Edit_POST_ReturnsNotFoundResult_WhenIdDoesNotMatchBrand");

            var controller = CreateControllerWithAdminUser(options);
            var brand = new Brand
            {
                Id = 2,
                Name = "Edited Brand",
                LogoUrl = "editedlogo.png",
                Description = "Edited Description",
            };

            // Act
            var result = await controller.Edit(1, brand);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsViewResult_WhenBrandExists()
        {
            // Arrange
            var options = CreateDbContextOptions("Delete_ReturnsViewResult_WhenBrandExists");

            var controller = CreateControllerWithAdminUser(options);

            // Act
            var result = await controller.Delete(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Brand>(viewResult.ViewData.Model);
            Assert.Equal("Brand 1", model.Name);
        }

        [Fact]
        public async Task Delete_ReturnsNotFoundResult_WhenBrandDoesNotExist()
        {
            // Arrange
            var options = CreateDbContextOptions("Delete_ReturnsNotFoundResult_WhenBrandDoesNotExist");

            var controller = CreateControllerWithAdminUser(options);

            // Act
            var result = await controller.Delete(3);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteConfirmed_RemovesBrandAndRedirectsToIndex()
        {
            // Arrange
            var options = CreateDbContextOptions("DeleteConfirmed_RemovesBrandAndRedirectsToIndex");

            var controller = CreateControllerWithAdminUser(options);

            // Act
            var result = await controller.DeleteConfirmed(1);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);

            using (var context = new AppDbContext(options))
            {
                Assert.Null(context.Brands.Find(1));
            }
        }
    }
}
