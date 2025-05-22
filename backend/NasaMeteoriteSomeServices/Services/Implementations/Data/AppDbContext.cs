using Microsoft.EntityFrameworkCore;
using Shared.Models;

namespace NasaMeteoriteSomeServices.Services.Implementations
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
