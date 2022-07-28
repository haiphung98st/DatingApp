using API.DTOs;
using API.Entities;
using API.Extensions;
using AutoMapper;

namespace API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<AppUser, MemberDto>()
                .ForMember(des => des.PhotoUrl, opt => opt.MapFrom(
                src => src.Photos.FirstOrDefault(x => x.IsMainPhoto).Url))
                .ForMember(des => des.Age, opt => opt.MapFrom(
                    src => src.DateOfBirth.CalculateAge()));

            CreateMap<Photo, PhotoDto>();
            CreateMap<UpdateMemberInfoDto, AppUser>();
        }
    }
}
