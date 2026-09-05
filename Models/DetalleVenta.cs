using System.ComponentModel.DataAnnotations.Schema;

namespace FarmaciaWebApp.Models
{
    public class DetalleVenta
    {
        public int Id { get; set; }
        public int VentaId { get; set; }
        public Venta? Venta { get; set; }

        public int MedicamentoId { get; set; }
        public Medicamento? Medicamento { get; set; }

        public int Cantidad { get; set; }
        [Column(TypeName = "decimal(18,2)")] public decimal PrecioUnitario { get; set; }
    }
}
