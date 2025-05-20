using Microsoft.EntityFrameworkCore;
using NasaMeteoriteService.Models;

namespace NasaMeteoriteService.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Meteorite> Meteorites { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Meteorite>()
                .HasIndex(m => m.NasaId)
                .IsUnique();
        }
    }
}
