using eGroceryStore.Data;
using System.ComponentModel.DataAnnotations;

namespace eGroceryStore.Models
{
        public class Order
        {
            [Key]
            public int Id { get; set; }

            public string Email { get; set; }

            public string UserId { get; set; }

            public StatusEnum Status { get; set; } = StatusEnum.Registred;

            public string Address { get; set; }

            public List<OrderItem> OrderItems { get; set; }
        }
}
