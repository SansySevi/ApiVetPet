using Microsoft.EntityFrameworkCore;
using NugetVetPet.Models;

namespace ApiVetPet.Data
{
    public class UsuariosContext : DbContext
    {
        public UsuariosContext(DbContextOptions<UsuariosContext> options)
            : base(options) { }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Mascota> Mascotas { get; set; }
        public DbSet<Tratamiento> Tratamientos { get; set; }
        public DbSet<Procedimiento> Procedimientos { get; set; }
        public DbSet<Prueba> Pruebas { get; set; }
        public DbSet<Vacuna> Vacunas { get; set; }
        public DbSet<Cita> Citas { get; set; }
        public DbSet<Evento> Eventos { get; set; }

        public DbSet<Servicio> Servicios { get; set; }
        public DbSet<FAQ> FAQs { get; set; }
    }
}
