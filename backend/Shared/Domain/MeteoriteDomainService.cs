using Shared.DTOs;
using Shared.Models;

namespace Shared.Domain
{
    public class MeteoriteDomainService
    {
        public IQueryable<MeteoriteGroupedDto> GroupByYear(IQueryable<Meteorite> query)
        {
            return query
                .Where(m => m.Year.HasValue)
                .GroupBy(m => m.Year.Value.Year)
                .Select(g => new MeteoriteGroupedDto
                {
                    Year = g.Key,
                    Count = g.Count(),
                    TotalMass = g.Sum(m => m.Mass ?? 0)
                });
        }
    }
}
