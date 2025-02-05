using Server_Ad_Baturina.Models.Requests;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Server_Ad_Baturina.Interfaces;
using Server_Ad_Baturina.Models.Responses;
using Server_advanced_Baturina.Models.DTOs;

namespace Server_advanced_Baturina.Controllers;

[Route("api/v1/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IMapper _mapper;

    public AuthController(IAuthService authService, IMapper mapper)
    {
        _authService = authService;
        _mapper = mapper;
    }

    [HttpPost("register")]
    public async Task<ActionResult<CustomSuccessResponse<SignInUserResponse>>> RegisterAsync(
        [FromBody]RegisterUserRequest registerRequest, 
        CancellationToken cancellationToken)
    {
        var registerDto = _mapper.Map<RegisterUserDto>(registerRequest);
        var signInResponse = await _authService.RegisterAsync(registerDto, cancellationToken);

        var response = new CustomSuccessResponse<SignInUserResponse>()
        {
            Data = signInResponse,
            StatusCode = StatusCodes.Status201Created,
            Success = true
        };

        return Created(nameof(RegisterAsync), response);
    }

    [HttpPost("sign-in")]
    public async Task<ActionResult<CustomSuccessResponse<SignInUserResponse>>> LoginAsync(
        SignInRequest signInRequest,
        CancellationToken cancellationToken)
    {
        var signInDto = _mapper.Map<SignInDto>(signInRequest);

        var signInUserResponse = await _authService.LoginAsync(signInDto, cancellationToken);

        var response = new CustomSuccessResponse<SignInUserResponse>()
        {
            Data = signInUserResponse,
            StatusCode = StatusCodes.Status200OK,
            Success = true
        };

        return Ok(response);
    }
}