using eGroceryStore.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eGroceryStoreTests
{
    public class OrderITemModelTests
    {
        [Fact]
        public void OrderItem_Id_ShouldHaveKeyAttribute()
        {
            // Arrange
            var propertyInfo = typeof(OrderItem).GetProperty("Id");

            // Act
            var keyAttribute = Assert.Single(propertyInfo.GetCustomAttributes(typeof(KeyAttribute), false));

            // Assert
            Assert.NotNull(keyAttribute);
        }


        [Fact]
        public void OrderItem_Quantity_CantBeNull()
        {
            // Act //Arrange
            var orderItem = new OrderItem
            {
                Id = 1,
                Price = 1,
                ProductId = 1,
                Product = null,
                Order = null,
                OrderId = 1,
            };
            // Assert
            Assert.Equal(orderItem.Quantity, 0);
        }

        [Fact]
        public void OrderItem_Price_CantBeNull()
        {
            // Act //Arrange
            var orderItem = new OrderItem
            {
                Id = 1,
                ProductId = 1,
                Product = null,
                Order = null,
                OrderId = 1,
            };
            // Assert
            Assert.Equal(orderItem.Price, 0);
        }

        [Fact]
        public void OrderItem_OrderId_CantBeNull()
        {
            // Act //Arrange
            var orderItem = new OrderItem
            {
                Id = 1,
                ProductId = 1,
                Product = null,
                Order = null,
            };
            // Assert
            Assert.Equal(orderItem.OrderId, 0);
        }


        [Fact]
        public void OrderItem_ProductId_CantBeNull()
        {
            // Act //Arrange
            var orderItem = new OrderItem
            {
                Id = 1,
                Product = null,
                Order = null,
            };
            // Assert
            Assert.Equal(orderItem.ProductId, 0);
        }

        [Fact]
        public void OrderItem_Quantity_CanBeSet()
        {
            // Act //Arrange
            var orderItem = new OrderItem
            {
                Id = 1,
                Quantity = 1,
                Product = null,
                Order = null,
            };
            // Assert
            Assert.Equal(orderItem.Quantity, 1);
        }

        [Fact]
        public void OrderItem_Price_CanBeSet()
        {
            // Act //Arrange
            var orderItem = new OrderItem
            {
                Id = 1,
                Quantity = 1,
                Price = 1,
                Product = null,
                Order = null,
            };
            // Assert
            Assert.Equal(orderItem.Price, 1);
        }

        [Fact]
        public void OrderItem_Product_CanBeSet()
        {
            // Act //Arrange
            var orderItem = new OrderItem
            {
                Id = 1,
                Quantity = 1,
                Price = 1,
                Product = new Product { Id = 1, CategoryId = 1, Category = null, BrandId = 1, Brand = null, Description = "", Ingredients = "", Name  = "", Price = 1, ProductPictureUrl = "", ProductStock = 1},
                Order = null,
            };
            // Assert
            Assert.Equal(orderItem.Product.Price, 1);
        }

        [Fact]
        public void OrderItem_Order_CanBeSet()
        {
            // Act //Arrange
            var orderItem = new OrderItem
            {
                Id = 1,
                Quantity = 1,
                Price = 1,
                Product = null,
                Order = new Order { Id = 1, Address = "", Email = "", OrderItems = new List<OrderItem>(), Status = eGroceryStore.Data.StatusEnum.Processed, UserId = "1" }
            };
            // Assert
            Assert.Equal(orderItem.Order.UserId, "1");
        }
    }
}
