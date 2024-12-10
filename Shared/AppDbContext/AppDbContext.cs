using Microsoft.EntityFrameworkCore;
using Coactivos_Predial.Models;
using Coactivos_Predial.Shared.Models;

public class AppDbContext : DbContext
{
    public DbSet<Usuarios>? usuarios { get; set; }
    
   
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

      

    // ...
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Configura la cadena de conexión a tu base de datos PostgreSQL

            optionsBuilder.UseNpgsql("Host=77.37.124.191;Database=General_Coactivo;Username=postgres;Password=adminsql26");
        }
}
