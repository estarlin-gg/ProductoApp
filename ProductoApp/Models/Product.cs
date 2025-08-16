using System.ComponentModel.DataAnnotations;

namespace ProductoApp.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo Nombre es obligatorio")]
        public string Name { get; set; }

        [Required(ErrorMessage = "El campo Precio es obligatorio")]
        [Range(0.01, 1000000, ErrorMessage = "El precio debe ser un número positivo")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "El campo Stock es obligatorio")]
        [Range(0, int.MaxValue, ErrorMessage = "El stock debe ser cero o más")]
        public int Stock { get; set; }
    }
}
