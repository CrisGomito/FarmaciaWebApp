using System.ComponentModel.DataAnnotations.Schema;

namespace FarmaciaWebApp.Models
{
    public class Venta
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
        [Column(TypeName = "decimal(18,2)")] public decimal Total { get; set; }

        public ICollection<DetalleVenta> Detalles { get; set; } = new List<DetalleVenta>();
    }
}
