using Shared.DTOs;

namespace NasaMeteoriteSomeServices.Services.Interfaces
{
    public interface IMeteoriteSort
    {
        IOrderedQueryable<MeteoriteGroupedDto> ApplySort(IQueryable<MeteoriteGroupedDto> query);
    }
}
