using eGroceryStore.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eGroceryStoreTests
{
    public class OrderModelTests
    {
        [Fact]
        public void Order_Id_ShouldHaveKeyAttribute()
        {
            // Arrange
            var propertyInfo = typeof(Order).GetProperty("Id");

            // Act
            var keyAttribute = Assert.Single(propertyInfo.GetCustomAttributes(typeof(KeyAttribute), false));

            // Assert
            Assert.NotNull(keyAttribute);
        }


        [Fact]
        public void Order_Email_CanBeNull()
        {
            // Act //Arrange
            var order = new Order
            {
                Id = 1,
                Email = null,
                UserId = "1",
                Status = eGroceryStore.Data.StatusEnum.Registred,
                OrderItems = new List<OrderItem>()
            };

            // Assert
            Assert.Null(order.Email);
        }

        [Fact]
        public void Order_Address_CanBeNull()
        {
            // Act //Arrange
            var order = new Order
            {
                Id = 1,
                Email = "abc@c.com",
                UserId = "1",
                Status = eGroceryStore.Data.StatusEnum.Registred,
                OrderItems = new List<OrderItem>(),
                Address = null
            };

            // Assert
            Assert.Null(order.Address);
        }

        [Fact]
        public void Order_UserId_CanBeNull()
        {
            // Act //Arrange
            var order = new Order
            {
                Id = 1,
                Email = "a@a.com",
                UserId = null,
                Status = eGroceryStore.Data.StatusEnum.Registred,
                OrderItems = new List<OrderItem>()
            };

            // Assert
            Assert.Null(order.UserId);
        }

        [Fact]
        public void Order_OrderItems_CanBeNull()
        {
            // Act //Arrange
            var order = new Order
            {
                Id = 1,
                Email = "a@a.com",
                UserId = "1",
                Status = eGroceryStore.Data.StatusEnum.Registred,
                OrderItems = null
            };

            // Assert
            Assert.Null(order.OrderItems);
        }

        [Fact]
        public void Order_Status_SetToRegistred()
        {
            // Act //Arrange
            var order = new Order
            {
                Id = 1,
                Email = "a@a.com",
                UserId = "1",
            };

            // Assert
            Assert.True(order.Status == eGroceryStore.Data.StatusEnum.Registred);
        }

        [Fact]
        public void Order_Status_CanSetProcessed()
        {
            // Act //Arrange
            var order = new Order
            {
                Id = 1,
                Email = "a@a.com",
                UserId = "1",
                Status = eGroceryStore.Data.StatusEnum.Processed,
            };

            // Assert
            Assert.True(order.Status == eGroceryStore.Data.StatusEnum.Processed);
        }

        [Fact]
        public void Order_Status_CanSetDelivered()
        {
            // Act //Arrange
            var order = new Order
            {
                Id = 1,
                Email = "a@a.com",
                UserId = "1",
                Status = eGroceryStore.Data.StatusEnum.Delivered,
            };

            // Assert
            Assert.True(order.Status == eGroceryStore.Data.StatusEnum.Delivered);
        }

        [Fact]
        public void Order_Email_CanBeSet()
        {
            // Act //Arrange
            var order = new Order
            {
                Id = 1,
                Email = "abc@gmail.com",
                UserId = "1",
                Status = eGroceryStore.Data.StatusEnum.Registred,
                OrderItems = new List<OrderItem>()
            };

            // Assert
            Assert.Equal("abc@gmail.com", order.Email);
        }

        [Fact]
        public void Order_UserId_CanBeSet()
        {
            // Act //Arrange
            var order = new Order
            {
                Id = 1,
                Email = "a@a.com",
                UserId = "1",
                Status = eGroceryStore.Data.StatusEnum.Registred,
                OrderItems = new List<OrderItem>()
            };

            // Assert
            Assert.Equal("1", order.UserId);
        }

        [Fact]
        public void Order_Address_CanHaveValues()
        {
            // Act //Arrange
            var order = new Order
            {
                Id = 1,
                Email = "abc@c.com",
                UserId = "1",
                Status = eGroceryStore.Data.StatusEnum.Registred,
                OrderItems = new List<OrderItem>(),
                Address = "Address"
            };

            // Assert
            Assert.Equal("Address", order.Address);
        }

        [Fact]
        public void Order_OrderItems_CanAddOneOrderItem()
        {
            // Act //Arrange
            var order = new Order
            {
                Id = 1,
                Email = "a@a.com",
                UserId = "1",
                Status = eGroceryStore.Data.StatusEnum.Registred,
                OrderItems = new List<OrderItem> { new OrderItem { Id = 1, Order = null, OrderId = 1, Price = 1, Product = null, ProductId = 1, Quantity = 1 } }
            };

            // Assert
            Assert.Equal(order.OrderItems[0].Quantity, 1);
        }


        [Fact]
        public void Order_OrderItems_CanAddTwoOrderItems()
        {
            // Act //Arrange
            var order = new Order
            {
                Id = 1,
                Email = "a@a.com",
                UserId = "1",
                Status = eGroceryStore.Data.StatusEnum.Registred,
                OrderItems = new List<OrderItem> 
                { new OrderItem { Id = 1, Order = null, OrderId = 1, Price = 1, Product = null, ProductId = 1, Quantity = 1 },
                new OrderItem { Id = 2, Order = null, OrderId = 1, Price = 2, Product = null, ProductId = 1, Quantity = 2 }
                }
            };

            // Assert
            Assert.Equal(order.OrderItems[1].Quantity, 2);
        }
    }
}
