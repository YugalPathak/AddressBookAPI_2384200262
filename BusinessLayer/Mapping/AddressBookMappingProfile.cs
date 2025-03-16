using AutoMapper;
using ModelLayer;
using ModelLayer.DTO;
using ModelLayer.Model;
using RepositoryLayer.Entity;

namespace BusinessLayer.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping between AddressBookEntry and AddressBookEntryDTO.
    /// </summary>
    public class AddressBookMappingProfile : Profile
    {
        public AddressBookMappingProfile()
        {
            CreateMap<AddressBookModel, AddressBookDTO>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FirstName + " " + src.LastName))
                .ReverseMap();
        }
    }
}