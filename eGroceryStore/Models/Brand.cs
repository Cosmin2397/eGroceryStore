using System.ComponentModel.DataAnnotations;

namespace eGroceryStore.Models
{
    public class Brand
    {
        [Key]
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? LogoUrl { get; set; }

        public string? Description { get; set; }

        //Relationships
        public List<Product>? Products { get; set; }
    }
}
