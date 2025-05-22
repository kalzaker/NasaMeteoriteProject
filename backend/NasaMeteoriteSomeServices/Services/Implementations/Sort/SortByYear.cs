using NasaMeteoriteSomeServices.Services.Interfaces;
using Shared.DTOs;

namespace NasaMeteoriteSomeServices.Services.Implementations
{
    public class SortByYear : IMeteoriteSort
    {
        public IOrderedQueryable<MeteoriteGroupedDto> ApplySort(IQueryable<MeteoriteGroupedDto> query)
        {
            return query.OrderBy(g => g.Year);
        }
    }
}
