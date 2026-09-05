using FarmaciaWebApp.Data;
using FarmaciaWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FarmaciaWebApp.Controllers
{
    public class ReportesController : Controller
    {
        private readonly FarmaciaDbContext _context;

        public ReportesController(FarmaciaDbContext context) => _context = context;

        //reporte de medicamentos vendidos por proveedor
        public async Task<IActionResult> PorProveedor()
        {
            var reporte = await _context.DetallesVenta
                .Include(d => d.Medicamento)
                .ThenInclude(m => m.Proveedor)
                .GroupBy(d => new {
                    ProveedorNombre = d.Medicamento.Proveedor.Nombre,
                    MedicamentoNombre = d.Medicamento.Nombre
                })
                .Select(g => new ReporteProveedorViewModel
                {
                    Proveedor = g.Key.ProveedorNombre,
                    Medicamento = g.Key.MedicamentoNombre,
                    CantidadVendida = g.Sum(d => d.Cantidad),
                    TotalGenerado = g.Sum(d => d.Cantidad * d.PrecioUnitario)
                })
                .OrderBy(r => r.Proveedor)
                .ToListAsync();

            return View(reporte);
        }
    }
}
