using System.ComponentModel.DataAnnotations;

namespace eGroceryStore.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        //Relationships
        public List<Product> Products { get; set; }

    }
}
