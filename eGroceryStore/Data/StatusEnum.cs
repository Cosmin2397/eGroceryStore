using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace eGroceryStore.Data
{
    public enum StatusEnum
    {
        [Display(Name = "Registred")]
        Registred = 1,

        [Display(Name = "Processed")]
        Processed,

        [Display(Name = "Delivered")]
        Delivered
    }
}
