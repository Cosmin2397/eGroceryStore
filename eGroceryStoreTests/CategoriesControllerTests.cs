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
    public class CategoriesControllerTests
    {
        private DbContextOptions<AppDbContext> CreateDbContextOptions(string databaseName)
        {
            return new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: databaseName)
                .Options;
        }

        private void PopulateTestData(AppDbContext context)
        {
            context.Categories.Add(new Category { Id = 1, Name = "Category 1" });
            context.Categories.Add(new Category { Id = 2, Name = "Category 2" });

            context.SaveChanges();
        }

        private CategoriesController CreateControllerWithAdminUser(DbContextOptions<AppDbContext> options)
        {
            using (var context = new AppDbContext(options))
            {
                PopulateTestData(context);
            }

            var contextForController = new AppDbContext(options);
            var controller = new CategoriesController(contextForController);
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
        public async Task Index_ReturnsViewResult_WithAListOfCategories()
        {
            // Arrange
            var options = CreateDbContextOptions("Index_ReturnsViewResult_WithAListOfCategories");

            var controller = new CategoriesController(new AppDbContext(options));

            // Act
            var result = await controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Category>>(viewResult.ViewData.Model);
            Assert.Equal(0, model.Count()); 
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
                var controller = new CategoriesController(context);
                var category = new Category
                {
                    Name = "New Category",
                };

                // Act
                var result = await controller.Create(category);

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
            var model = Assert.IsType<Category>(viewResult.ViewData.Model);
            Assert.Equal("Category 1", model.Name);
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
        public async Task Edit_GET_ReturnsNotFoundResult_WhenCategoryIsNull()
        {
            // Arrange
            var options = CreateDbContextOptions("Edit_GET_ReturnsNotFoundResult_WhenCategoryIsNull");

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
            var category = new Category
            {
                Id = 1,
                Name = "Edited Category",
            };

            // Act
            var result = await controller.Edit(1, category);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task Edit_POST_ReturnsNotFoundResult_WhenIdDoesNotMatchCategory()
        {
            // Arrange
            var options = CreateDbContextOptions("Edit_POST_ReturnsNotFoundResult_WhenIdDoesNotMatchCategory");

            var controller = CreateControllerWithAdminUser(options);
            var category = new Category
            {
                Id = 2,
                Name = "Edited Category",
            };

            // Act
            var result = await controller.Edit(4, category);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsViewResult_WhenCategoryExists()
        {
            // Arrange
            var options = CreateDbContextOptions("Delete_ReturnsViewResult_WhenCategoryExists");

            var controller = CreateControllerWithAdminUser(options);

            // Act
            var result = await controller.Delete(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Category>(viewResult.ViewData.Model);
            Assert.Equal("Category 1", model.Name);
        }

        [Fact]
        public async Task Delete_ReturnsNotFoundResult_WhenCategoryDoesNotExist()
        {
            // Arrange
            var options = CreateDbContextOptions("Delete_ReturnsNotFoundResult_WhenCategoryDoesNotExist");

            var controller = CreateControllerWithAdminUser(options);

            // Act
            var result = await controller.Delete(3);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteConfirmed_RemovesCategoryAndRedirectsToIndex()
        {
            // Arrange
            var options = CreateDbContextOptions("DeleteConfirmed_RemovesCategoryAndRedirectsToIndex");

            var controller = CreateControllerWithAdminUser(options);

            // Act
            var result = await controller.DeleteConfirmed(1);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);

            using (var context = new AppDbContext(options))
            {
                Assert.Null(context.Categories.Find(1));
            }
        }
    }
}
