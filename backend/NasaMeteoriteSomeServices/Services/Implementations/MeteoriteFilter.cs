using Shared.Models;

namespace NasaMeteoriteSomeServices.Services.Implementations
{
    public class MeteoriteFilter
    {
        public int? YearFrom { get; set; }
        public int? YearTo { get; set; }
        public string? Recclass { get; set; }
        public string? NameContains { get; set; }

        public IQueryable<Meteorite> Apply(IQueryable<Meteorite> query)
        {
            if (YearFrom.HasValue)
                query = query.Where(m => m.Year.HasValue && m.Year.Value.Year >= YearFrom.Value);

            if (YearTo.HasValue)
                query = query.Where(m => m.Year.HasValue && m.Year.Value.Year <= YearTo.Value);

            if (!string.IsNullOrWhiteSpace(Recclass))
                query = query.Where(m => m.Recclass == Recclass);

            if (!string.IsNullOrWhiteSpace(NameContains))
                query = query.Where(m => m.Name.ToLower().Contains(NameContains.ToLower()));

            return query;
        }
    }
}
