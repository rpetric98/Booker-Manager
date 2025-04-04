using AutoMapper;
using Data.Models;
using WebAPP.Models;

namespace WebAPP.Mapping
{
    public class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<UserDetail, UserDetailVM>();
            CreateMap<UserMessage, UserMessageVM>();
            CreateMap<PropertyAmenity, PropertyAmenityVM>();
            CreateMap<Property, PropertyVM>();
            CreateMap<PropertyType, PropertyTypeVM>();
            CreateMap<Amenity, AmenityVM>();
        }
    }

}
