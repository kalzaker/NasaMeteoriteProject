using Microsoft.EntityFrameworkCore;
using NasaMeteoriteSomeServices.Services.Interfaces;
using Shared.Domain;
using Shared.DTOs;
using Shared.Models;

namespace NasaMeteoriteSomeServices.Services.Implementations
{
    public class MeteoriteService : IMeteoriteService
    {
        private readonly IMeteoriteRepository _repository;
        private readonly MeteoriteDomainService _domainService;
        private readonly Dictionary<string, IMeteoriteSort> _sort;

        public MeteoriteService(IMeteoriteRepository repository, MeteoriteDomainService domainService)
        {
            _repository = repository;
            _domainService = domainService;
            _sort = new Dictionary<string, IMeteoriteSort>
            {
                { "year", new SortByYear() },
                { "count", new SortByCount() },
                { "mass", new SortByMass() }
            };
        }

        public async Task<List<Meteorite>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<List<MeteoriteGroupedDto>> GetFilteredGroupedMeteoritesAsync(
        int? yearFrom, int? yearTo, string? recclass, string? nameContains, string sortBy)
        {
            if (yearFrom.HasValue && yearTo.HasValue && yearFrom > yearTo)
                throw new ArgumentException("yearFrom cannot be greater than yearTo.");

            if (string.IsNullOrEmpty(sortBy))
                sortBy = "year";

            var filter = new MeteoriteFilter
            {
                YearFrom = yearFrom,
                YearTo = yearTo,
                Recclass = recclass,
                NameContains = nameContains
            };

            var query = _repository.GetMeteoritesQueryable();
            query = filter.Apply(query);

            var groupedQuery = _domainService.GroupByYear(query);

            var sortKey = sortBy.ToLower();
            var sortStrategy = _sort.ContainsKey(sortKey) ? _sort[sortKey] : _sort["year"];
            groupedQuery = sortStrategy.ApplySort(groupedQuery);

            return await groupedQuery.ToListAsync();
        }
    }
}
