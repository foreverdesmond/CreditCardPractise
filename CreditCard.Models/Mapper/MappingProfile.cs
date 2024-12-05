using AutoMapper;
using CreditCard.Models.DTOs;

namespace CreditCard.Models.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreditCard, CreditCardDto>();
            CreateMap<CreditCardDto, CreditCard>();
        }
    }
}