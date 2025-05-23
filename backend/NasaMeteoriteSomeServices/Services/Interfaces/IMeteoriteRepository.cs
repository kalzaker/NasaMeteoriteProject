using Shared.Models;
using Microsoft.EntityFrameworkCore.Storage;

namespace NasaMeteoriteSomeServices.Services.Interfaces
{
    public interface IMeteoriteRepository
    {
        Task<List<Meteorite>> GetAllAsync();
        Task<List<string>> GetAllNasaIdsAsync();
        Task<Meteorite> GetByNasaIdAsync(string nasaId);
        Task AddAsync(Meteorite entity);
        Task AddRangeAsync(IEnumerable<Meteorite> entities);
        void Update(Meteorite entity);
        void UpdateRange(IEnumerable<Meteorite> entities);
        void RemoveRange(IEnumerable<Meteorite> entities);
        Task SaveChangesAsync();
        IQueryable<Meteorite> GetMeteoritesQueryable();
        Task<IDbContextTransaction> BeginTransactionAsync();
    }
}
