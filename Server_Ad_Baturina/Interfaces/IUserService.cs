using Server_Ad_Baturina.Models.DTOs;
using Server_Ad_Baturina.Models.Responses;

namespace Server_advanced_Baturina.Interfaces;

public interface IUserService
{
    Task<List<PublicUserResponse>> GetAllAsync(CancellationToken cancellationToken);

    Task<PutUserResponse> ReplaseAsync(PutUserDto putUserDto, CancellationToken cancellationToken);
    
    Task DeleteAsync(DeleteUserDto deleteUserDto, CancellationToken cancellationToken);

    Task<PublicUserResponse> GetAsync(InfoUserDto infoUserDto, CancellationToken cancellationToken);
}