﻿using System.ComponentModel.DataAnnotations;

namespace eGroceryStore.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        public string ProductPictureUrl { get; set; }

        public string Name { get; set; }

        public Brand ProductBrand { get; set; }

        public double Price { get; set; }

        public int ProductStock { get; set; }

        public string  Description { get; set; }

        public string Ingredients { get; set; }
    }
}
