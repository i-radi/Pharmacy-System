using AutoMapper;
using PharmacyWebAPI.Models;
using PharmacyWebAPI.Models.Dto;

namespace PharmacyWebAPI.Utility.Settings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Drug, DrugGetDto>()
                .ReverseMap()
                .ForMember(dest => dest.Category, src => src.Ignore())
                .ForMember(dest => dest.Manufacturer, src => src.Ignore());

            CreateMap<Drug, DrugDetailsGetDto>()
                .ReverseMap();

            CreateMap<Drug, PostDrugDto>()
                .ForMember(dest => dest.Categories, src => src.Ignore())
                .ForMember(dest => dest.Manufacturers, src => src.Ignore())
                .ReverseMap();
            CreateMap<OrderDetail, OrderDetailsDto>()
               .ReverseMap()
               .ForMember(dest => dest.Drug, src => src.Ignore())
               .ForMember(dest => dest.Order, src => src.Ignore())
               .ForMember(dest => dest.OrderId, src => src.Ignore());

            CreateMap<Category, CategoryDto>()
                .ReverseMap();

            CreateMap<PrescriptionDetails, PrescriptionDetailsDto>()
               .ReverseMap()
               .ForMember(dest => dest.PrescriptionId, src => src.Ignore())
               .ForMember(dest => dest.Drug, src => src.Ignore())
               .ForMember(dest => dest.Prescription, src => src.Ignore());
        }
    }
}