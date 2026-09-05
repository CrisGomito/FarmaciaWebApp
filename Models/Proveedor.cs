using System.ComponentModel.DataAnnotations;

namespace FarmaciaWebApp.Models
{
    public class Proveedor
    {
        public int Id { get; set; }
        [Required] public string Nombre { get; set; } = string.Empty;
        public string? Telefono { get; set; }
        public string? Direccion { get; set; }

        public ICollection<Medicamento> Medicamentos { get; set; } = new List<Medicamento>();
    }
}
