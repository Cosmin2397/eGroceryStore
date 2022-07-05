using System.ComponentModel.DataAnnotations;

namespace eGroceryStore.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public string ProductBrand { get; set; }

        public double Price { get; set; }

        public int ProductStock { get; set; }

        public string  Description { get; set; }

        public string Ingredients { get; set; }
    }
}
