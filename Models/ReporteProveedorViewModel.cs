namespace FarmaciaWebApp.Models
{
    public class ReporteProveedorViewModel
    {
        public string Proveedor { get; set; } = string.Empty;
        public string Medicamento { get; set; } = string.Empty;
        public int CantidadVendida { get; set; }
        public decimal TotalGenerado { get; set; }
    }
}
