using Shared.DTOs;
using Shared.Models;

namespace NasaMeteoriteSomeServices.Services.Interfaces
{
    public interface IMeteoriteService
    {
        Task<List<Meteorite>> GetAllAsync();
        Task<List<MeteoriteGroupedDto>> GetFilteredGroupedMeteoritesAsync(
            int? yearFrom, int? yearTo, string? recclass, string? nameContains, string sortBy);
    }
}
