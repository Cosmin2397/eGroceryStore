using System;
using System.Collections.Generic;
using eGroceryStore.Data;
using eGroceryStore.Data.Services;
using eGroceryStore.Models;
using FluentAssertions.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace eGroceryStore.Tests
{
    public class ShoppingCartTests
    {
        private readonly IServiceProvider _serviceProvider;

        public ShoppingCartTests()
        {
            var services = new ServiceCollection();
            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase(databaseName: "InMemoryDb"));


            _serviceProvider = services.BuildServiceProvider();
        }

        [Fact]
        public void AddToCart_ShouldIncreaseQuantity()
        {
            // Arrange
            using (var scope = _serviceProvider.CreateScope())
            {
                var shoppingCart = new ShoppingCart(scope.ServiceProvider.GetRequiredService<AppDbContext>());
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                dbContext.Database.EnsureDeleted();
                dbContext.Database.EnsureCreated();
                shoppingCart.ShoppingCartId = "new";
                var product = new Product { Id = 1, ProductStock = 10 };

                // Act
                shoppingCart.AddToCart(product);

                // Assert
                Assert.Single(shoppingCart.GetShoppingCartItems());
                Assert.Equal(1, shoppingCart.GetShoppingCartItems()[0].Quantity);
                shoppingCart.ClearShoppingCartAsync();
            }
        }

        [Fact]
        public void RemoveFromCart_ShouldDecreaseQuantity()
        {
            // Arrange
            using (var scope = _serviceProvider.CreateScope())
            {
                var shoppingCart = new ShoppingCart(scope.ServiceProvider.GetRequiredService<AppDbContext>());
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                dbContext.Database.EnsureDeleted();
                dbContext.Database.EnsureCreated();
                shoppingCart.ShoppingCartId = "new";
                var product = new Product { Id = 1, ProductStock = 10 };

                // Act
                shoppingCart.AddToCart(product);
                shoppingCart.RemoveFromCart(product);
                shoppingCart.ClearShoppingCartAsync();
                // Assert
                Assert.Empty(shoppingCart.GetShoppingCartItems());
            }
        }

        [Fact]
        public void AddToCart_ShouldDecreaseProductStock()
        {
            // Arrange
            using (var scope = _serviceProvider.CreateScope())
            {
                var shoppingCart = new ShoppingCart(scope.ServiceProvider.GetRequiredService<AppDbContext>());
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                dbContext.Database.EnsureDeleted();
                dbContext.Database.EnsureCreated();
                shoppingCart.ShoppingCartId = "new";
                var product = new Product { Id = 1, ProductStock = 10 };

                // Act
                shoppingCart.AddToCart(product);

                // Assert
                Assert.Equal(9, product.ProductStock);
                shoppingCart.ClearShoppingCartAsync();
            }
        }

        [Fact]
        public void RemoveFromCart_ShouldIncreaseProductStock()
        {
            // Arrange
            using (var scope = _serviceProvider.CreateScope())
            {
                var shoppingCart = new ShoppingCart(scope.ServiceProvider.GetRequiredService<AppDbContext>());
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                dbContext.Database.EnsureDeleted();
                dbContext.Database.EnsureCreated();
                shoppingCart.ShoppingCartId = "new";
                var product = new Product { Id = 1, ProductStock = 10 };

                // Act
                shoppingCart.AddToCart(product);
                shoppingCart.RemoveFromCart(product);

                // Assert
                Assert.Equal(10, product.ProductStock);
                shoppingCart.ClearShoppingCartAsync();
            }
        }

        [Fact]
        public void GetShoppingCartTotal_ShouldCalculateTotalPrice()
        {
            // Arrange
            using (var scope = _serviceProvider.CreateScope())
            {
                var shoppingCart = new ShoppingCart(scope.ServiceProvider.GetRequiredService<AppDbContext>());
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                dbContext.Database.EnsureDeleted();
                dbContext.Database.EnsureCreated();
                shoppingCart.ShoppingCartId = "new";
                var product1 = new Product { Id = 1, Price = 20 };
                var product2 = new Product { Id = 2, Price = 30 };

                // Act
                shoppingCart.AddToCart(product1);
                shoppingCart.AddToCart(product2);

                // Assert
                Assert.Equal(50, shoppingCart.GetShoppingCartTotal());
                shoppingCart.ClearShoppingCartAsync();
            }
        }

        [Fact]
        public void ClearShoppingCartAsync_ShouldRemoveAllItems()
        {
            // Arrange
            using (var scope = _serviceProvider.CreateScope())
            {
                var shoppingCart = new ShoppingCart(scope.ServiceProvider.GetRequiredService<AppDbContext>());
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                dbContext.Database.EnsureDeleted();
                dbContext.Database.EnsureCreated();
                shoppingCart.ShoppingCartId = "new";
                var product1 = new Product { Id = 1 };
                var product2 = new Product { Id = 2 };

                // Act
                shoppingCart.AddToCart(product1);
                shoppingCart.AddToCart(product2);
                shoppingCart.ClearShoppingCartAsync().Wait(); 

                // Assert
                Assert.Empty(shoppingCart.GetShoppingCartItems()); 
            }
        }

        [Fact]
        public void ClearShoppingCartAsync_ShouldRestoreProductStock()
        {
            // Arrange
            using (var scope = _serviceProvider.CreateScope())
            {
                var shoppingCart = new ShoppingCart(scope.ServiceProvider.GetRequiredService<AppDbContext>());
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                dbContext.Database.EnsureDeleted();
                dbContext.Database.EnsureCreated();
                shoppingCart.ShoppingCartId = "new";
                var product1 = new Product { Id = 1, ProductStock = 5 };
                var product2 = new Product { Id = 2, ProductStock = 8 };

                // Act
                shoppingCart.AddToCart(product1);
                shoppingCart.AddToCart(product2);
                shoppingCart.RemoveFromCart(product1);
                shoppingCart.RemoveFromCart(product2);

                // Assert
                Assert.Equal(5, product1.ProductStock); 
                Assert.Equal(8, product2.ProductStock); 
            }
        }

    }
}
