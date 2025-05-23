using AutoMapper;
using Shared.DTOs;
using Shared.Models;
using System.Globalization;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<MeteoriteDto, Meteorite>()
            .ForMember(dest => dest.NasaId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Nametype, opt => opt.MapFrom(src => src.Nametype))
            .ForMember(dest => dest.Recclass, opt => opt.MapFrom(src => src.Recclass))
            .ForMember(dest => dest.Mass, opt => opt.MapFrom(src => TryParseFloat(src.Mass)))
            .ForMember(dest => dest.Fall, opt => opt.MapFrom(src => src.Fall))
            .ForMember(dest => dest.Year, opt => opt.MapFrom(src => TryParseDate(src.Year)))
            .ForMember(dest => dest.Reclat, opt => opt.MapFrom(src => TryParseFloat(src.Reclat)))
            .ForMember(dest => dest.Reclong, opt => opt.MapFrom(src => TryParseFloat(src.Reclong)));
    }

    private static float? TryParseFloat(string? input) =>
        float.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out var res) ? res : null;

    private static DateTime? TryParseDate(string? input) =>
        DateTime.TryParse(input, out var date)
            ? DateTime.SpecifyKind(date, DateTimeKind.Utc)
            : null;
}