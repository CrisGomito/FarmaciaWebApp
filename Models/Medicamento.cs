using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FarmaciaWebApp.Models
{
    public class Medicamento
    {
        public int Id { get; set; }
        [Required] public string Nombre { get; set; } = string.Empty;
        [Column(TypeName = "decimal(18,2)")] public decimal Precio { get; set; }
        public int Stock { get; set; }

        [Display(Name = "Proveedor")]
        public int ProveedorId { get; set; }
        public Proveedor? Proveedor { get; set; }
    }
}
