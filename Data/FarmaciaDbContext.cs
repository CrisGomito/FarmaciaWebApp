using FarmaciaWebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace FarmaciaWebApp.Data
{
    public class FarmaciaDbContext : DbContext
    {
        public FarmaciaDbContext(DbContextOptions<FarmaciaDbContext> options) : base(options) { }

        public DbSet<Proveedor> Proveedores { get; set; }
        public DbSet<Medicamento> Medicamentos { get; set; }
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<DetalleVenta> DetallesVenta { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Evitar borrado en cascada para proteger el historial de ventas
            modelBuilder.Entity<DetalleVenta>()
                .HasOne(d => d.Medicamento)
                .WithMany()
                .HasForeignKey(d => d.MedicamentoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
