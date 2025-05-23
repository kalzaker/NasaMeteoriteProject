using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using NasaMeteoriteSomeServices.Services.Interfaces;
using Shared.Models;

namespace NasaMeteoriteSomeServices.Services.Implementations
{
    public class MeteoriteRepository : IMeteoriteRepository
    {
        private readonly AppDbContext _context;

        public MeteoriteRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Meteorite>> GetAllAsync()
        {
            return await _context.Meteorites.ToListAsync();
        }

        public async Task<List<string>> GetAllNasaIdsAsync()
        {
            return await _context.Meteorites.Select(m => m.NasaId).ToListAsync();
        }

        public async Task<Meteorite> GetByNasaIdAsync(string nasaId)
        {
            return await _context.Meteorites.FirstOrDefaultAsync(m => m.NasaId == nasaId);
        }

        public Task AddAsync(Meteorite entity)
        {
            _context.Meteorites.Add(entity);
            return Task.CompletedTask;
        }

        public async Task AddRangeAsync(IEnumerable<Meteorite> entities)
        {
            await _context.Meteorites.AddRangeAsync(entities);
        }

        public void Update(Meteorite entity)
        {
            _context.Meteorites.Update(entity);
        }

        public void UpdateRange(IEnumerable<Meteorite> entities)
        {
            _context.Meteorites.UpdateRange(entities);
        }

        public void RemoveRange(IEnumerable<Meteorite> entities)
        {
            _context.Meteorites.RemoveRange(entities);
        }

        public Task SaveChangesAsync() => _context.SaveChangesAsync();

        public IQueryable<Meteorite> GetMeteoritesQueryable()
        {
            return _context.Meteorites.AsQueryable();
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }
    }
}
