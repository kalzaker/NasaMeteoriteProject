using Microsoft.Extensions.Logging;
using NasaMeteoriteSomeServices.Services.Interfaces;
using Shared.DTOs;
using Shared.Models;
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using System.Text.Json;

namespace NasaMeteoriteSomeServices.Services.Implementations
{
    public class MeteoriteSyncService : IMeteoriteSyncService
    {
        private readonly IMeteoriteRepository _repository;
        private readonly INasaApiClient _nasaApiClient;
        private readonly IMapper _mapper;
        private readonly ILogger<MeteoriteSyncService> _logger;

        public MeteoriteSyncService(
            IMeteoriteRepository repository,
            INasaApiClient nasaApiClient,
            IMapper mapper,
            ILogger<MeteoriteSyncService> logger)
        {
            _repository = repository;
            _nasaApiClient = nasaApiClient;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task SyncMeteoriteDataAsync()
        {
            _logger.LogInformation("Starting data sync...");
            try
            {
                var nasaData = await _nasaApiClient.FetchMeteoriteDataAsync();
                if (!nasaData.Any())
                {
                    _logger.LogWarning("No data received.");
                    return;
                }
                await ProcessMeteoriteDataAsync(nasaData);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Failed to fetch data from NASA API.");
                throw;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize NASA API response.");
                throw;
            }
            catch (ValidationException ex)
            {
                _logger.LogError(ex, "Data validation failed.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during sync.");
                throw;
            }
        }

        private async Task ProcessMeteoriteDataAsync(List<MeteoriteDto> nasaData)
        {
            var validData = new List<MeteoriteDto>();
            foreach (var dto in nasaData)
            {
                var results = ValidateMeteoriteDto(dto);
                if (!results.Any())
                {
                    validData.Add(dto);
                }
                else
                {
                    _logger.LogWarning("Invalid meteorite data: {Errors}", string.Join("; ", results.Select(r => r.ErrorMessage)));
                }
            }

            if (!validData.Any())
            {
                _logger.LogWarning("No valid data after validation.");
                return;
            }

            using var transaction = await _repository.BeginTransactionAsync();
            try
            {
                var existingIds = await _repository.GetAllNasaIdsAsync();
                var incomingIds = new HashSet<string>(validData.Select(x => x.Id));

                var toAdd = new List<Meteorite>();
                var toUpdate = new List<Meteorite>();

                foreach (var dto in validData)
                {
                    if (!existingIds.Contains(dto.Id))
                    {
                        toAdd.Add(_mapper.Map<Meteorite>(dto));
                    }
                    else
                    {
                        var entity = await _repository.GetByNasaIdAsync(dto.Id);
                        if (entity != null)
                        {
                            var mapped = _mapper.Map<Meteorite>(dto);
                            if (!AreEntitiesEqual(entity, mapped))
                            {
                                _mapper.Map(dto, entity);
                                toUpdate.Add(entity);
                            }
                        }
                    }
                }

                var toRemove = existingIds.Except(incomingIds)
                                         .Select(id => new Meteorite { NasaId = id })
                                         .ToList();

                if (toAdd.Any()) await _repository.AddRangeAsync(toAdd);
                if (toUpdate.Any()) _repository.UpdateRange(toUpdate);
                if (toRemove.Any()) _repository.RemoveRange(toRemove);

                await _repository.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Sync done: Added/Updated = {0}, Removed = {1}",
                    validData.Count - existingIds.Count, toRemove.Count);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Sync failed, transaction rolled back.");
                throw;
            }
        }

        private static List<ValidationResult> ValidateMeteoriteDto(MeteoriteDto dto)
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(dto);
            Validator.TryValidateObject(dto, context, results, true);
            return results;
        }

        private static bool AreEntitiesEqual(Meteorite existing, Meteorite mapped)
        {
            return existing.NasaId == mapped.NasaId &&
                   existing.Name == mapped.Name &&
                   existing.Nametype == mapped.Nametype &&
                   existing.Recclass == mapped.Recclass &&
                   existing.Mass == mapped.Mass &&
                   existing.Fall == mapped.Fall &&
                   existing.Year == mapped.Year &&
                   existing.Reclat == mapped.Reclat &&
                   existing.Reclong == mapped.Reclong;
        }
    }
}
    