using FarmaciaWebApp.Data;
using FarmaciaWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FarmaciaWebApp.Controllers
{
    public class VentasController : Controller
    {
        private readonly FarmaciaDbContext _context;

        public VentasController(FarmaciaDbContext context) => _context = context;

        public async Task<IActionResult> Index()
        {
            return View(await _context.Ventas.Include(v => v.Detalles).ToListAsync());
        }

        // GET: Ventas/Create
        public IActionResult Create()
        {
            ViewData["MedicamentoId"] = new SelectList(_context.Medicamentos.Where(m => m.Stock > 0), "Id", "Nombre");
            return View();
        }

        // POST: Ventas/Create
        //logica de negocio: validacion de stock y calculo de totales
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int MedicamentoId, int Cantidad)
        {
            var medicamento = await _context.Medicamentos.FindAsync(MedicamentoId);

            if (medicamento == null || medicamento.Stock < Cantidad)
            {
                ModelState.AddModelError("", "Stock insuficiente o medicamento inválido.");
                ViewData["MedicamentoId"] = new SelectList(_context.Medicamentos.Where(m => m.Stock > 0), "Id", "Nombre");
                return View();
            }

            // Crear Venta
            var venta = new Venta { Fecha = DateTime.Now, Total = medicamento.Precio * Cantidad };

            // Crear Detalle
            var detalle = new DetalleVenta
            {
                MedicamentoId = medicamento.Id,
                Cantidad = Cantidad,
                PrecioUnitario = medicamento.Precio
            };

            venta.Detalles.Add(detalle);
            _context.Ventas.Add(venta);

            // Reducir Stock
            medicamento.Stock -= Cantidad;
            _context.Update(medicamento);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        // POST: Ventas/CreateMultiple
        [HttpPost]
        public async Task<IActionResult> CreateMultiple([FromBody] VentaMultipleViewModel modelo)
        {
            if (modelo == null || !modelo.Detalles.Any())
            {
                return Json(new { success = false, message = "El carrito está vacío." });
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var venta = new Venta { Fecha = DateTime.Now, Total = 0 };
                _context.Ventas.Add(venta);
                await _context.SaveChangesAsync();

                foreach (var item in modelo.Detalles)
                {
                    var medicamento = await _context.Medicamentos.FindAsync(item.MedicamentoId);

                    if (medicamento == null || medicamento.Stock < item.Cantidad)
                    {
                        //si falla un stock hacemos rollback
                        await transaction.RollbackAsync();
                        return Json(new { success = false, message = $"Stock insuficiente para el medicamento ID {item.MedicamentoId}." });
                    }

                    var detalle = new DetalleVenta
                    {
                        VentaId = venta.Id,
                        MedicamentoId = medicamento.Id,
                        Cantidad = item.Cantidad,
                        PrecioUnitario = medicamento.Precio
                    };
                    _context.DetallesVenta.Add(detalle);

                    venta.Total += (item.Cantidad * medicamento.Precio);

                    medicamento.Stock -= item.Cantidad;
                    _context.Update(medicamento);
                }

                _context.Update(venta);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Json(new { success = true, redirectUrl = Url.Action("Index", "Ventas") });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Json(new { success = false, message = "Error del servidor: " + ex.Message });
            }
        }
    }
}
