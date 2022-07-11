using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eGroceryStore.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        public string? ProductPictureUrl { get; set; }

        public string? Name { get; set; }

        public double Price { get; set; }

        public int ProductStock { get; set; }

        public string? Description { get; set; }

        public string? Ingredients { get; set; }


        //Relationships
        public int BrandId { get; set; }

        [ForeignKey("BrandId")]
        public Brand? Brand { get; set; }

        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }
    }
}
