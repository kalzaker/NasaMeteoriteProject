using Shared.Models;

namespace NasaMeteoriteSomeServices.Services.Interfaces
{
    public interface IMeteoriteRepository
    {
        Task<List<Meteorite>> GetAllAsync();
        Task AddAsync(Meteorite entity);
        void Update(Meteorite entity);
        void RemoveRange(IEnumerable<Meteorite> entities);
        Task SaveChangesAsync();
        IQueryable<Meteorite> GetMeteoritesQueryable();
    }
}
