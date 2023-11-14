using eGroceryStore.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eGroceryStoreTests
{
    public class CategoryModelTests
    {
        [Fact]
        public void Category_Id_ShouldHaveKeyAttribute()
        {
            // Arrange
            var propertyInfo = typeof(Category).GetProperty("Id");

            // Act
            var keyAttribute = Assert.Single(propertyInfo.GetCustomAttributes(typeof(KeyAttribute), false));

            // Assert
            Assert.NotNull(keyAttribute);
        }

        [Fact]
        public void Category_Name_CanBeNull()
        {
            // Act //Arrange
            var category = new Category
            {
                Id = 1,
            };

            // Assert
            Assert.Null(category.Name);
        }

        [Fact]
        public void Category_Products_CanBeNull()
        {
            // Act //Arrange
            var category = new Category
            {
                Id = 1,
            };

            // Assert
            Assert.Null(category.Products);
        }

        [Fact]
        public void Category_Name_CanBeSet()
        {
            // Act //Arrange
            var category = new Category
            {
                Id = 1,
                Name = "Category 1"
            };

            // Assert
            Assert.Equal("Category 1", category.Name);
        }
    }
}
