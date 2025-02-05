using AutoMapper;
using Server_Ad_Baturina.Models.DTOs;
using Server_Ad_Baturina.Models.Entities;
using Server_Ad_Baturina.Models.Requests;
using Server_Ad_Baturina.Models.Responses;
using Server_advanced_Baturina.Models.DTOs;

namespace Server_advanced_Baturina.Automapper;

public class ApiMappingProfile : Profile
{
    public ApiMappingProfile()
    {
        CreateMap<RegisterUserRequest, RegisterUserDto>()
            .ForMember(desc => desc.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(desc => desc.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(desc => desc.Avatar, opt => opt.MapFrom(src => src.Avatar))
            .ForMember(desc => desc.Password, opt => opt.MapFrom(src => src.Password))
            .ForMember(desc => desc.Role, opt => opt.MapFrom(src => src.Role));

        CreateMap<SignInRequest, SignInDto>()
            .ForMember(desc => desc.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(desc => desc.Password, opt => opt.MapFrom(src => src.Password));

        CreateMap<PutUserRequest, PutUserDto>()
            .ForMember(desc => desc.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(desc => desc.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(desc => desc.Role, opt => opt.MapFrom(src => src.Role))
            .ForMember(desc => desc.Avatar, opt => opt.MapFrom(src => src.Avatar));

        CreateMap<Guid, DeleteUserDto>()
            .ForMember(desc => desc.Id, opt => opt.MapFrom(src => src));

        CreateMap<Guid, InfoUserDto>()
            .ForMember(desc => desc.Id, opt => opt.MapFrom(src => src));

        CreateMap<NewsRequest, NewsDto>()
            .ForMember(desc => desc.Image, opt => opt.MapFrom(src => src.Image))
            .ForMember(desc => desc.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(desc => desc.Tags, opt => opt.MapFrom(src => src.Tags))
            .ForMember(desc => desc.Description, opt => opt.MapFrom(src => src.Description));

        CreateMap<NewsRequest, NewsUpdateDto>()
            .ForMember(desc => desc.Image, opt => opt.MapFrom(src => src.Image))
            .ForMember(desc => desc.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(desc => desc.Tags, opt => opt.MapFrom(src => src.Tags))
            .ForMember(desc => desc.Description, opt => opt.MapFrom(src => src.Description));

        CreateMap<Guid, DeleteNewsDto>()
           .ForMember(desc => desc.Id, opt => opt.MapFrom(src => src));

        CreateMap<RegisterUserDto, UserEntity>()
            .ForMember(desc => desc.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(desc => desc.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(desc => desc.Role, opt => opt.MapFrom(src => src.Role));

        CreateMap<UserEntity, SignInUserResponse>()
           .ForMember(desc => desc.Email, opt => opt.MapFrom(src => src.Email))
           .ForMember(desc => desc.Name, opt => opt.MapFrom(src => src.Name))
           .ForMember(desc => desc.Avatar, opt => opt.MapFrom(src => src.Avatar))
           .ForMember(desc => desc.Id, opt => opt.MapFrom(src => src.Id))
           .ForMember(desc => desc.Role, opt => opt.MapFrom(src => src.Role));

        CreateMap<UserEntity, PublicUserResponse>()
           .ForMember(desc => desc.Id, opt => opt.MapFrom(src => src.Id))
           .ForMember(desc => desc.Email, opt => opt.MapFrom(src => src.Email))
           .ForMember(desc => desc.Name, opt => opt.MapFrom(src => src.Name))
           .ForMember(desc => desc.Avatar, opt => opt.MapFrom(src => src.Avatar))
           .ForMember(desc => desc.Id, opt => opt.MapFrom(src => src.Id))
           .ForMember(desc => desc.Role, opt => opt.MapFrom(src => src.Role));

        CreateMap<UserEntity, PutUserResponse>()
           .ForMember(desc => desc.Id, opt => opt.MapFrom(src => src.Id))
           .ForMember(desc => desc.Email, opt => opt.MapFrom(src => src.Email))
           .ForMember(desc => desc.Name, opt => opt.MapFrom(src => src.Name))
           .ForMember(desc => desc.Avatar, opt => opt.MapFrom(src => src.Avatar))
           .ForMember(desc => desc.Id, opt => opt.MapFrom(src => src.Id))
           .ForMember(desc => desc.Role, opt => opt.MapFrom(src => src.Role));

        CreateMap<NewsDto, NewsEntity>()
           .ForMember(desc => desc.Image, opt => opt.MapFrom(src => src.Image))
           .ForMember(desc => desc.Title, opt => opt.MapFrom(src => src.Title))
           .ForMember(desc => desc.Tags, opt => opt.MapFrom(src => src.Tags))
           .ForMember(desc => desc.Description, opt => opt.MapFrom(src => src.Description));

        CreateMap<NewsEntity, CreateNewsSuccessResponse>()
           .ForMember(desc => desc.Id, opt => opt.MapFrom(src => src.Id));

        CreateMap<string, TagsEntity>()
           .ForMember(desc => desc.Title, opt => opt.MapFrom(src => src));

        CreateMap<UploadFileRequest, UploadFileDto>()
            .ForMember(desc => desc.File, opt => opt.MapFrom(src => src.File));
    }
}