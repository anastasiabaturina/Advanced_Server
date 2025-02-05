using Server_Ad_Baturina.Models.Responses;
using Server_advanced_Baturina.Models.DTOs;

namespace Server_Ad_Baturina.Interfaces;

public interface IAuthService
{
    Task<SignInUserResponse> RegisterAsync(RegisterUserDto registerUserDto, CancellationToken cancellationToken);

    Task<SignInUserResponse> LoginAsync(SignInDto signInDto, CancellationToken cancellationToken);
}