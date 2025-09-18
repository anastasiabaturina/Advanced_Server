using AutoMapper;
using DotNetEnv;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Server_Ad_Baturina.Exceptions;
using Server_Ad_Baturina.Interfaces;
using Server_Ad_Baturina.Models.Entities;
using Server_Ad_Baturina.Models.Responses;
using Server_advanced_Baturina.Interfaces;
using Server_advanced_Baturina.Models.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;

namespace Server_advanced_Baturina.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher<UserEntity> _passwordHasher;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _contextAccessor;

    public AuthService(IUserRepository userRepository, IPasswordHasher<UserEntity> passwordHasher, IMapper mapper, IHttpContextAccessor contextAccessor)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _mapper = mapper;
        _contextAccessor = contextAccessor;
    }

    public async Task<SignInUserResponse> RegisterAsync(
        RegisterUserDto registerUserDto, 
        CancellationToken cancellationToken)
    {
        if (await _userRepository.FindAsync(registerUserDto.Email, cancellationToken) != null)
        {
            throw new BadRequestException("The user already exists");
        };

        var user = _mapper.Map<UserEntity>(registerUserDto);
        user.Password = _passwordHasher.HashPassword(user, user.Password);
        user.Avatar = $"{_contextAccessor.HttpContext.Request.Scheme}://{_contextAccessor.HttpContext.Request.Host}/api/v1/files/{registerUserDto.Avatar}";
        await _userRepository.RegisterAsync(user, cancellationToken);

        var signInResponse = _mapper.Map<SignInUserResponse>(user);
        signInResponse.Token = GenerateToken(user.Id);

        return signInResponse;
    }

    public async Task<SignInUserResponse> LoginAsync(
        SignInDto signInDto,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.FindAsync(signInDto.Email, cancellationToken);

        if (user == null)
        {
            throw new NotFoundException("The user was not found");
        }

        if (_passwordHasher.VerifyHashedPassword(user, user.Password, signInDto.Password) == PasswordVerificationResult.Failed)
        {
            throw new AuthenticationException("Invalid password");
        }

        var signInResponse = _mapper.Map<SignInUserResponse>(user);
        signInResponse.Avatar = $"{_contextAccessor.HttpContext.Request.Scheme}://{_contextAccessor.HttpContext.Request.Host}/api/v1/files/{user.Avatar}";
        signInResponse.Token = GenerateToken(user.Id);

        return signInResponse;
    }

    private string GenerateToken(Guid id)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Env.GetString("JWT")));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, id.ToString()),
        };

        var token = new JwtSecurityToken(
            issuer: Env.GetString("JWT_ISSUER"),
            audience: Env.GetString("JWT_AUDIENCE"),
            claims: claims,
            signingCredentials: credentials,
            expires: DateTime.UtcNow.AddHours(Env.GetDouble("JWT_LIFETIME"))
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}