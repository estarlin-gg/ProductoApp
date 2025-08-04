using System.ComponentModel.DataAnnotations;

namespace  ProductoApp.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; }

        [Range(0.01, 99999.99)]
        public decimal Price { get; set; }

        [Range(0, 10000)]
        public int Stock { get; set; }
    }
}
