using Shared.DTOs;

namespace NasaMeteoriteSomeServices.Services.Interfaces
{
    public interface INasaApiClient
    {
        Task<List<MeteoriteDto>> FetchMeteoriteDataAsync();
    }
}
