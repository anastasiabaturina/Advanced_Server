using AutoMapper;
using Server_Ad_Baturina.Exceptions;
using Server_Ad_Baturina.Models.DTOs;
using Server_Ad_Baturina.Models.Responses;
using Server_advanced_Baturina.Interfaces;

namespace Server_advanced_Baturina.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UserService(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<List<PublicUserResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        var listUsers = await _userRepository.GetAllAsync(cancellationToken);
        var publicUserViews = _mapper.Map<List<PublicUserResponse>>(listUsers);

        return publicUserViews;
    }

    public async Task<PutUserResponse> ReplaseAsync(
        PutUserDto putUserDto, 
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetInfoByIdAsync(putUserDto.Id, cancellationToken);

        if (user == null)
        {
            throw new NotFoundException("The user was not found");
        }

        var putUser = await _userRepository.ReplaceAsync(putUserDto.Id, putUserDto.Avatar, putUserDto.Email, putUserDto.Name, putUserDto.Role, cancellationToken);
        var putUserResponse = _mapper.Map<PutUserResponse>(putUser);

        return putUserResponse;
    }

    public async Task DeleteAsync(DeleteUserDto deleteUserDto, CancellationToken cancellationToken) =>
        await _userRepository.DeleteAsync(deleteUserDto.Id, cancellationToken);

    public async Task<PublicUserResponse> GetAsync(InfoUserDto userDto, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetInfoByIdAsync(userDto.Id, cancellationToken);

        if (user == null)
        {
            throw new NotFoundException("The user was not found"); 
        }

        var userInfo = _mapper.Map<PublicUserResponse>(user);

        return userInfo;
    }
}