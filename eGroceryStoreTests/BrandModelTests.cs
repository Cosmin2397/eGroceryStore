using eGroceryStore.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eGroceryStoreTests
{
    public class BrandModelTests
    {
        [Fact]
        public void Brand_Id_ShouldHaveKeyAttribute()
        {
            // Arrange
            var propertyInfo = typeof(Brand).GetProperty("Id");

            // Act
            var keyAttribute = Assert.Single(propertyInfo.GetCustomAttributes(typeof(KeyAttribute), false));

            // Assert
            Assert.NotNull(keyAttribute);
        }

        [Fact]
        public void Brand_Name_CanBeNull()
        {
            // Act //Arrange
            var brand = new Brand
            {
                Id = 1,
            };

            // Assert
            Assert.Null(brand.Name);
        }

        [Fact]
        public void Brand_LogoUrl_CanBeNull()
        {
            // Act //Arrange
            var brand = new Brand
            {
                Id = 1,
            };

            // Assert
            Assert.Null(brand.LogoUrl);
        }

        [Fact]
        public void Brand_Description_CanBeNull()
        {
            // Act //Arrange
            var brand = new Brand
            {
                Id = 1,
            };

            // Assert
            Assert.Null(brand.Description);
        }

        [Fact]
        public void Brand_Products_CanBeNull()
        {
            // Act //Arrange
            var brand = new Brand
            {
                Id = 1,
            };

            // Assert
            Assert.Null(brand.Products);
        }

        [Fact]
        public void Brand_Name_CanBeSet()
        {
            // Act //Arrange
            var brand = new Brand
            {
                Id = 1,
                Name = "name"
            };

            // Assert
            Assert.Equal("name", brand.Name);
        }

        [Fact]
        public void Brand_LogoUrl_CanBeSet()
        {
            // Act //Arrange
            var brand = new Brand
            {
                Id = 1,
                LogoUrl = "logo"
            };

            // Assert
            Assert.Equal("logo", brand.LogoUrl);
        }

        [Fact]
        public void Brand_Description_CanBeSet()
        {
            // Act //Arrange
            var brand = new Brand
            {
                Id = 1,
                Description = "description"
            };

            // Assert
            Assert.Equal("description", brand.Description);
        }

    }
}
