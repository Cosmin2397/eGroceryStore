namespace eGroceryStore.Models
{
    public class Brand
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string LogoUrl { get; set; }

        public string Description { get; set; }

        //Relationships
        public List<Product> Products { get; set; }
    }
}
