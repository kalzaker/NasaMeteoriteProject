using Microsoft.Extensions.Logging;
using NasaMeteoriteService.Models;
using NasaMeteoriteSomeServices.Services.Interfaces;
using System.Text.Json;
using Shared.Models;

namespace NasaMeteoriteSomeServices.Services.Implementations
{
    public class MeteoriteSyncService : IMeteoriteSyncService
    {
        private readonly IMeteoriteRepository _repository;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<MeteoriteSyncService> _logger;

        public MeteoriteSyncService(
            IMeteoriteRepository repository,
            IHttpClientFactory httpClientFactory,
            ILogger<MeteoriteSyncService> logger)
        {
            _repository = repository;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task SyncMeteoriteDataAsync()
        {
            _logger.LogInformation("Starting data sync...");

            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.GetStringAsync(Shared.Misc.SourceUrl.GetUrl());

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var nasaData = JsonSerializer.Deserialize<List<MeteoriteDto>>(response, options);

            if (nasaData == null)
            {
                _logger.LogWarning("No data received.");
                return;
            }

            var existing = await _repository.GetAllAsync();
            var existingDict = existing.ToDictionary(m => m.NasaId);
            var incomingIds = new HashSet<string>(nasaData.Select(x => x.Id));

            foreach (var dto in nasaData)
            {
                if (existingDict.TryGetValue(dto.Id, out var entity))
                {
                    if (IsDifferent(entity, dto))
                    {
                        UpdateEntity(entity, dto);
                        _repository.Update(entity);
                    }
                }
                else
                {
                    var newEntity = ToEntity(dto);
                    await _repository.AddAsync(newEntity);
                }
            }

            var toRemove = existing.Where(m => !incomingIds.Contains(m.NasaId)).ToList();
            _repository.RemoveRange(toRemove);

            await _repository.SaveChangesAsync();
            _logger.LogInformation("Sync done: Added/Updated = {0}, Removed = {1}",
                nasaData.Count - existingDict.Count, toRemove.Count);
        }

        private static bool IsDifferent(Meteorite entity, MeteoriteDto dto) =>
            entity.Name != dto.Name ||
            entity.Nametype != dto.Nametype ||
            entity.Recclass != dto.Recclass ||
            entity.Mass != TryParseFloat(dto.Mass) ||
            entity.Fall != dto.Fall ||
            entity.Year != TryParseDate(dto.Year) ||
            entity.Reclat != TryParseFloat(dto.Reclat) ||
            entity.Reclong != TryParseFloat(dto.Reclong) ||
            entity.GeoLat != (dto.Geolocation?.Coordinates?.ElementAtOrDefault(1) ?? 0) ||
            entity.GeoLong != (dto.Geolocation?.Coordinates?.ElementAtOrDefault(0) ?? 0);

        private static void UpdateEntity(Meteorite e, MeteoriteDto d)
        {
            e.Name = d.Name;
            e.Nametype = d.Nametype;
            e.Recclass = d.Recclass;
            e.Mass = TryParseFloat(d.Mass);
            e.Fall = d.Fall;
            e.Year = TryParseDate(d.Year);
            e.Reclat = TryParseFloat(d.Reclat);
            e.Reclong = TryParseFloat(d.Reclong);
            e.GeoLat = d.Geolocation?.Coordinates?.ElementAtOrDefault(1);
            e.GeoLong = d.Geolocation?.Coordinates?.ElementAtOrDefault(0);
        }

        private static Meteorite ToEntity(MeteoriteDto d) => new()
        {
            NasaId = d.Id,
            Name = d.Name,
            Nametype = d.Nametype,
            Recclass = d.Recclass,
            Mass = TryParseFloat(d.Mass),
            Fall = d.Fall,
            Year = TryParseDate(d.Year),
            Reclat = TryParseFloat(d.Reclat),
            Reclong = TryParseFloat(d.Reclong),
            GeoLat = d.Geolocation?.Coordinates?.ElementAtOrDefault(1),
            GeoLong = d.Geolocation?.Coordinates?.ElementAtOrDefault(0)
        };

        private static float? TryParseFloat(string? input) =>
            float.TryParse(input, out var res) ? res : null;

        private static DateTime? TryParseDate(string? input) =>
            DateTime.TryParse(input, out var date)
                ? DateTime.SpecifyKind(date, DateTimeKind.Utc)
                : null;
    }
}
