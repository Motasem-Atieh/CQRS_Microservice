using AutoMapper;
using CQRS_Microservice.Dto;
using CQRS_Microservice.Models;   // Namespace for your DTOs

namespace CQRS_Microservice.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Mapping from Product to ProductDto
            CreateMap<Product, ProductDto>();

            // Mapping from ProductDto to Product
            CreateMap<ProductDto, Product>();
        }
    }
}
