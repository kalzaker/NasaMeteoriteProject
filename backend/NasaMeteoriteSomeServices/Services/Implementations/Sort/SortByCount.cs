using NasaMeteoriteSomeServices.Services.Interfaces;
using Shared.DTOs;

namespace NasaMeteoriteSomeServices.Services.Implementations
{
    public class SortByCount : IMeteoriteSort
    {
        public IOrderedQueryable<MeteoriteGroupedDto> ApplySort(IQueryable<MeteoriteGroupedDto> query)
        {
            return query.OrderByDescending(g => g.Count);
        }
    }
}
