using Microsoft.EntityFrameworkCore;
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

        public Task AddAsync(Meteorite entity)
        {
            _context.Meteorites.Add(entity);
            return Task.CompletedTask;
        }

        public void Update(Meteorite entity)
        {
            _context.Meteorites.Update(entity);
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
    }
}
