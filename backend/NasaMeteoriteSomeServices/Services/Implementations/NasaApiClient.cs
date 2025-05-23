using Microsoft.Extensions.Logging;
using NasaMeteoriteSomeServices.Services.Interfaces;
using Shared.DTOs;
using System.Text.Json;

namespace NasaMeteoriteSomeServices.Services.Implementations
{
    public class NasaApiClient : INasaApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<NasaApiClient> _logger;

        public NasaApiClient (IHttpClientFactory httpClientFactory, ILogger<NasaApiClient> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<List<MeteoriteDto>> FetchMeteoriteDataAsync()
        {
            _logger.LogInformation("Fetching meteorite data from NASA API...");
            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync(Shared.Misc.SourceUrl.GetUrl());
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var data = JsonSerializer.Deserialize<List<MeteoriteDto>>(content, options);
            if (data == null)
            {
                _logger.LogWarning("No data received from NASA API.");
                return [];
            }
            return data;
        }
    }
}
