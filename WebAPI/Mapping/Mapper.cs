using AutoMapper;
using Data.Models;
using WebAPI.Dtos;

namespace WebAPI.Mapping
{
    public class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<Property, PropertyDto>();
            CreateMap<UserDetail, UserDetailDto>();
        }
    }
}
