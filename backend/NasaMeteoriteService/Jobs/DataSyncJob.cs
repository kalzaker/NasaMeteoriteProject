using Microsoft.EntityFrameworkCore;
using NasaMeteoriteService.Data;
using NasaMeteoriteService.Models;
using Quartz;
using System.Text.Json;

namespace NasaMeteoriteService.Jobs
{
    public class DataSyncJob : IJob
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<DataSyncJob> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        private const string SourceUrl = "https://raw.githubusercontent.com/biggiko/nasa-dataset/refs/heads/main/y77d-th95.json";

        public DataSyncJob(AppDbContext dbContext, ILogger<DataSyncJob> logger, IHttpClientFactory httpClientFactory)
        {
            _dbContext = dbContext;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("Starting data sync job...");

            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                var response = await httpClient.GetStringAsync(SourceUrl);

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var nasaData = JsonSerializer.Deserialize<List<MeteoriteDto>>(response, options);

                if (nasaData == null)
                {
                    _logger.LogWarning("No data received from NASA.");
                    return;
                }

                var existing = await _dbContext.Meteorites.ToListAsync();
                var existingDict = existing.ToDictionary(m => m.NasaId);

                var incomingIds = new HashSet<string>(nasaData.Select(d => d.Id));

                foreach (var dto in nasaData)
                {
                    if (existingDict.TryGetValue(dto.Id, out var existingMeteorite))
                    {
                        if (IsDifferent(existingMeteorite, dto))
                        {
                            UpdateEntity(existingMeteorite, dto);
                        }
                    }
                    else
                    {
                        _dbContext.Meteorites.Add(ToEntity(dto));
                    }
                }

                var toRemove = existing.Where(m => !incomingIds.Contains(m.NasaId)).ToList();
                _dbContext.Meteorites.RemoveRange(toRemove);

                await _dbContext.SaveChangesAsync();
                _logger.LogInformation("Data sync completed: {Added} new, {Updated} updated, {Deleted} removed.",
                    nasaData.Count - existingDict.Count,
                    existingDict.Values.Count(m => incomingIds.Contains(m.NasaId)),
                    toRemove.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Data sync job failed.");
            }
        }

        private bool IsDifferent(Meteorite entity, MeteoriteDto dto)
        {
            return entity.Name != dto.Name ||
                   entity.Nametype != dto.Nametype ||
                   entity.Recclass != dto.Recclass ||
                   entity.Mass != TryParseFloat(dto.Mass) ||
                   entity.Fall != dto.Fall ||
                   entity.Year != TryParseDate(dto.Year) ||
                   entity.Reclat != TryParseFloat(dto.Reclat) ||
                   entity.Reclong != TryParseFloat(dto.Reclong) ||
                   entity.GeoLat != (dto.Geolocation?.Coordinates?.ElementAtOrDefault(1) ?? 0) ||
                   entity.GeoLong != (dto.Geolocation?.Coordinates?.ElementAtOrDefault(0) ?? 0);
        }

        private void UpdateEntity(Meteorite entity, MeteoriteDto dto)
        {
            entity.Name = dto.Name;
            entity.Nametype = dto.Nametype;
            entity.Recclass = dto.Recclass;
            entity.Mass = TryParseFloat(dto.Mass);
            entity.Fall = dto.Fall;
            entity.Year = TryParseDate(dto.Year);
            entity.Reclat = TryParseFloat(dto.Reclat);
            entity.Reclong = TryParseFloat(dto.Reclong);
            entity.GeoLat = dto.Geolocation?.Coordinates?.ElementAtOrDefault(1);
            entity.GeoLong = dto.Geolocation?.Coordinates?.ElementAtOrDefault(0);
        }

        private Meteorite ToEntity(MeteoriteDto dto) => new Meteorite
        {
            NasaId = dto.Id,
            Name = dto.Name,
            Nametype = dto.Nametype,
            Recclass = dto.Recclass,
            Mass = TryParseFloat(dto.Mass),
            Fall = dto.Fall,
            Year = TryParseDate(dto.Year),
            Reclat = TryParseFloat(dto.Reclat),
            Reclong = TryParseFloat(dto.Reclong),
            GeoLat = dto.Geolocation?.Coordinates?.ElementAtOrDefault(1),
            GeoLong = dto.Geolocation?.Coordinates?.ElementAtOrDefault(0)
        };

        private float? TryParseFloat(string? input)
            => float.TryParse(input, out var result) ? result : null;

        private DateTime? TryParseDate(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;

            return DateTime.TryParse(input, out var result)
                ? DateTime.SpecifyKind(result, DateTimeKind.Utc)
                : null;
        }
    }
}