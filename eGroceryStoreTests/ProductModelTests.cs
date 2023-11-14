using eGroceryStore.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eGroceryStoreTests
{
    public class ProductModelTests
    {
        [Fact]
        public void Product_Id_ShouldHaveKeyAttribute()
        {
            // Arrange
            var propertyInfo = typeof(Product).GetProperty("Id");

            // Act
            var keyAttribute = Assert.Single(propertyInfo.GetCustomAttributes(typeof(KeyAttribute), false));

            // Assert
            Assert.NotNull(keyAttribute);
        }


        [Fact]
        public void Product_Name_CanBeNull()
        {
            // Act //Arrange
            var product = new Product()
            {
                Name = null,
            };

            // Assert
            Assert.Null(product.Name);
        }


        [Fact]
        public void Product_PhotoUrl_CanBeNull()
        {
            // Act //Arrange
            var product = new Product()
            {
                ProductPictureUrl = null,
            };

            // Assert
            Assert.Null(product.ProductPictureUrl);
        }

        [Fact]
        public void Product_Description_CanBeNull()
        {
            // Act //Arrange
            var product = new Product()
            {
                Description = null,
            };

            // Assert
            Assert.Null(product.Description);
        }

        [Fact]
        public void Product_Ingredients_CanBeNull()
        {
            // Act //Arrange
            var product = new Product()
            {
                Ingredients = null,
            };

            // Assert
            Assert.Null(product.Ingredients);
        }

        [Fact]
        public void Product_ProductStock_CantBeNull()
        {
            // Act //Arrange
            var product = new Product(){};

            // Assert
            Assert.Equal(product.ProductStock, 0);
        }

        [Fact]
        public void Product_Price_CantBeNull()
        {
            // Act //Arrange
            var product = new Product() { };

            // Assert
            Assert.Equal(product.Price, 0);
        }

        [Fact]
        public void Product_Price_CanBeAdded()
        {
            // Act //Arrange
            var product = new Product() 
            { 
                Price  = 1,
            };

            // Assert
            Assert.Equal(product.Price, 1);
        }

        [Fact]
        public void Product_ProductStock_CanBeAdded()
        {
            // Act //Arrange
            var product = new Product() 
            {
                ProductStock = 1,
            };

            // Assert
            Assert.Equal(product.ProductStock, 1);
        }

        [Fact]
        public void Product_Name_CanBeAdded()
        {
            // Act //Arrange
            var product = new Product()
            {
                Name = "name"
            };

            // Assert
            Assert.Equal(product.Name, "name");
        }

        [Fact]
        public void Product_Description_CanBeAdded()
        {
            // Act //Arrange
            var product = new Product()
            {
               Description = "description",
            };

            // Assert
            Assert.Equal(product.Description, "description");
        }

        [Fact]
        public void Product_Ingredients_CanBeAdded()
        {
            // Act //Arrange
            var product = new Product()
            {
                Ingredients = "ingredients",
            };

            // Assert
            Assert.Equal(product.Ingredients, "ingredients");
        }

        [Fact]
        public void Product_Category_CanBeAdded()
        {
            // Act //Arrange
            var product = new Product()
            {
                Category = new Category() {  Id = 1, Name = "cat1", Products = null}
            };

            // Assert
            Assert.Equal(product.Category.Name, "cat1");
        }

        [Fact]
        public void Product_Brand_CanBeAdded()
        {
            // Act //Arrange
            var product = new Product()
            {
                Brand = new Brand { Id = 1, Description = "desc", Name = "name", LogoUrl = "logo", Products = null}
            };

            // Assert
            Assert.Equal(product.Brand.Description, "desc");
        }
    }
}
