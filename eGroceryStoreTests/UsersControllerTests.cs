using eGroceryStore.Areas.Data;
using eGroceryStore.Controllers;
using eGroceryStore.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace eGroceryStoreTests
{
    public class UsersControllerTests
    {
        private readonly Mock<UserManager<ApplicationUser>> userManagerMock;
        private readonly UsersController controller;

        public UsersControllerTests()
        {
            userManagerMock = new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(),
                null, null, null, null, null, null, null, null);

            controller = new UsersController(userManagerMock.Object, MockDbContext());
        }

        private AppDbContext MockDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public void Index_ReturnsViewResult_WithListOfUsers_WhenUserIsAdmin()
        {
            // Arrange
            SetupUserIsAdmin();

            // Act
            var result = controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<ApplicationUser>>(viewResult.ViewData.Model);
            Assert.Equal(0, model.Count); // Modify this based on your actual data
        }

        [Fact]
        public void Index_ReturnsUnauthorizedResult_WhenUserIsNotAdmin()
        {
            // Arrange
            SetupUserIsNotAdmin();

            // Act
            var result = controller.Index();

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedResult>(result);
            Assert.NotNull(unauthorizedResult);
            Assert.Equal(401, unauthorizedResult.StatusCode);
        }

        [Fact]
        public async Task UserData_ReturnsViewResult_WhenUserIsAuthenticated()
        {
            // Arrange
            var userId = "user1";
            var user = new ApplicationUser { Id = userId, UserName = "user1" };
            SetupUser(user);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Name, userId) }, "mock")) }
            };

            // Act
            var result = await controller.UserData();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ApplicationUser>(viewResult.ViewData.Model);
            Assert.Equal(userId, model.Id);
        }

        [Fact]
        public async Task UserData_ReturnsNull_WhenUserIsNotAuthenticated()
        {
            // Arrange
            userManagerMock.Setup(mock => mock.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                           .ReturnsAsync((ApplicationUser)null);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal()
                }
            };

            // Act
            var result = await controller.UserData();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.Model);
            Assert.Equal((null), viewResult.StatusCode);
        }




        private void SetupUserIsAdmin()
        {
            SetupUser(new ApplicationUser { Id = "admin", UserName = "admin@example.com" });

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "admin"),
                new Claim(ClaimTypes.Role, "admin")
            };

            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };
        }

        private void SetupUserIsNotAdmin()
        {
            SetupUser(new ApplicationUser { Id = "nonadmin", UserName = "nonadmin@example.com" });

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
        }

        private void SetupUser(ApplicationUser user)
        {
            userManagerMock.Setup(mock => mock.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(user);
        }
    }
}
