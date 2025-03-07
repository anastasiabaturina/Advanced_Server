using AutoFixture;
using AutoMapper;
using DotNetEnv;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using Server_Ad_Baturina.Exceptions;
using Server_Ad_Baturina.Models.Entities;
using Server_Ad_Baturina.Models.Responses;
using Server_advanced_Baturina.Interfaces;
using Server_advanced_Baturina.Models.DTOs;
using Server_advanced_Baturina.Services;
using System.Security.Authentication;

namespace Server_Ad_Baturina.Tests;

public class AuthServiceTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IPasswordHasher<UserEntity>> _passwordHasherMock;
    private readonly Mock<IMapper> _mapperMock; 
    private readonly Mock<IHttpContextAccessor> _contextAccessorMock;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _fixture = new Fixture();
        Env.TraversePath().Load();

        _userRepositoryMock = new Mock<IUserRepository>();
        _passwordHasherMock = new Mock<IPasswordHasher<UserEntity>>();
        _mapperMock = new Mock<IMapper>(); 
        _contextAccessorMock = new Mock<IHttpContextAccessor>();

        _authService = new AuthService(
            _userRepositoryMock.Object,
            _passwordHasherMock.Object,
            _mapperMock.Object,
            _contextAccessorMock.Object
        );
    }

    [Fact]
    public async Task RegisterAsync_ShouldReturnToken_WhenUserIsSuccessfullyRegistered()
    {
        var registerUserDto = _fixture.Create<RegisterUserDto>();

        var user = _fixture.Build<UserEntity>()
            .With(u => u.Password, "someHashedPassword") 
            .Create();

        var signInResponse = _fixture.Create<SignInUserResponse>();

        _userRepositoryMock.Setup(repo => repo.FindAsync(registerUserDto.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserEntity)null); 

        _passwordHasherMock.Setup(ph => ph.HashPassword(It.IsAny<UserEntity>(), It.IsAny<string>()))
            .Returns("someHashedPassword"); 

        _mapperMock.Setup(mapper => mapper.Map<UserEntity>(registerUserDto))
            .Returns(user);

        var fakeHttpContext = new DefaultHttpContext();
        _contextAccessorMock.Setup(ca => ca.HttpContext).Returns(fakeHttpContext);

        _mapperMock.Setup(mapper => mapper.Map<SignInUserResponse>(user))
            .Returns(signInResponse);

        var result = await _authService.RegisterAsync(registerUserDto, CancellationToken.None);

        Assert.NotNull(result);
        Assert.NotNull(result.Token);
        _userRepositoryMock.Verify(repo => repo.RegisterAsync(It.IsAny<UserEntity>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_UserExists_ShouldThrowBadRequestException()
    {
        var registerUserDto = _fixture.Create<RegisterUserDto>();

        var user = _fixture.Create<UserEntity>();

        _userRepositoryMock.Setup(repo => repo.FindAsync(registerUserDto.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        await Assert.ThrowsAsync<BadRequestException>(() => _authService.RegisterAsync(registerUserDto, CancellationToken.None));
    }

    [Fact]
    public async Task LoginAsync_ValidCredentials_ReturnSignInUserResponse()
    {
        var signInDto = _fixture.Create<SignInDto>();
        var user = _fixture.Create<UserEntity>();
        var signInResponse = _fixture.Create<SignInUserResponse>();

        _userRepositoryMock.Setup(repo => repo.FindAsync(signInDto.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _passwordHasherMock.Setup(ph => ph.VerifyHashedPassword(user, user.Password, signInDto.Password))
            .Returns(PasswordVerificationResult.Success);

        _mapperMock.Setup(mapper => mapper.Map<SignInUserResponse>(user))
            .Returns(signInResponse);

        var result = await _authService.LoginAsync(signInDto, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(signInResponse, result);
        Assert.NotNull(result.Token);
    }

    [Fact]
    public async Task LoginAsync_UserNotFound_ShouldThrowNotFoundException()
    {
        var signInDto = _fixture.Create<SignInDto>();
        _userRepositoryMock
            .Setup(repo => repo.FindAsync(signInDto.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserEntity)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _authService.LoginAsync(signInDto, CancellationToken.None));
    }

    [Fact]
    public async Task LoginAsync_InvalidPassword_ShouldThrowAuthenticationException()
    {
        var signInDto = _fixture.Create<SignInDto>();
        var user = _fixture.Create<UserEntity>();

        _userRepositoryMock
            .Setup(repo => repo.FindAsync(signInDto.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _passwordHasherMock
            .Setup(p => p.VerifyHashedPassword(user, user.Password, signInDto.Password))
            .Returns(PasswordVerificationResult.Failed);

        await Assert.ThrowsAsync<AuthenticationException>(() => _authService.LoginAsync(signInDto, CancellationToken.None));
    }
}