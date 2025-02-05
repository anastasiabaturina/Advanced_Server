using AutoFixture;
using AutoMapper;
using Moq;
using Server_Ad_Baturina.Exceptions;
using Server_Ad_Baturina.Models.DTOs;
using Server_Ad_Baturina.Models.Entities;
using Server_Ad_Baturina.Models.Responses;
using Server_advanced_Baturina.Interfaces;
using Server_advanced_Baturina.Services;

namespace Server_Ad_Baturina.Tests;

public class UserServiceTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _fixture = new Fixture();
        _userRepositoryMock = new Mock<IUserRepository>();
        _mapperMock = new Mock<IMapper>();

        _userService = new UserService(
            _userRepositoryMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnListPublicUserResponse()
    {
        var users = _fixture.CreateMany<UserEntity>(5).ToList();
        var publicUserResponse = _fixture.CreateMany<PublicUserResponse>(5).ToList();

        _userRepositoryMock.Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(users);

        _mapperMock.Setup(mapper => mapper.Map<List<PublicUserResponse>>(users))
            .Returns(publicUserResponse);

        var result = await _userService.GetAllAsync(CancellationToken.None);

        Assert.Equal(publicUserResponse.Count, result.Count);
        _userRepositoryMock.Verify(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mapperMock.Verify(mapper => mapper.Map<List<PublicUserResponse>>(users), Times.Once);
    }

    [Fact]
    public async Task ReplaseAsync_ShouldReturnPutUserResponse_WhenUserExists()
    {
        var putUserDto = _fixture.Create<PutUserDto>();
        var existingUser = _fixture.Create<UserEntity>();
        var replacedUser = _fixture.Create<UserEntity>();
        var putUserResponse = _fixture.Create<PutUserResponse>();

        _userRepositoryMock.Setup(repo => repo.GetInfoByIdAsync(putUserDto.Id, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(existingUser);

        _userRepositoryMock.Setup(repo => repo.ReplaceAsync(putUserDto.Id, putUserDto.Avatar, putUserDto.Email, putUserDto.Name, putUserDto.Role, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(replacedUser);

        _mapperMock.Setup(mapper => mapper.Map<PutUserResponse>(replacedUser))
                   .Returns(putUserResponse);

        var result = await _userService.ReplaseAsync(putUserDto, CancellationToken.None);

        Assert.Equal(putUserResponse, result);
        _userRepositoryMock.Verify(repo => repo.GetInfoByIdAsync(putUserDto.Id, It.IsAny<CancellationToken>()), Times.Once);
        _userRepositoryMock.Verify(repo => repo.ReplaceAsync(putUserDto.Id, putUserDto.Avatar, putUserDto.Email, putUserDto.Name, putUserDto.Role, It.IsAny<CancellationToken>()), Times.Once);
        _mapperMock.Verify(mapper => mapper.Map<PutUserResponse>(replacedUser), Times.Once);
    }

    [Fact]
    public async Task ReplaseAsync_ShouldThrowNotFoundException_WhenUserDoesNotExist()
    {
        var putUserDto = _fixture.Create<PutUserDto>();
        _userRepositoryMock.Setup(repo => repo.GetInfoByIdAsync(putUserDto.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserEntity)null);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            _userService.ReplaseAsync(putUserDto, CancellationToken.None));

        _userRepositoryMock.Verify(repo => repo.GetInfoByIdAsync(putUserDto.Id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteUser_ShouldCallDeleteRepositoryMethod()
    {
        var deleteUserDto = _fixture.Create<DeleteUserDto>();

        await _userService.DeleteAsync(deleteUserDto, CancellationToken.None);

        _userRepositoryMock.Verify(repo => repo.DeleteAsync(deleteUserDto.Id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAsync_returnsPublicUserResponse()
    {
        var userDto = _fixture.Create<InfoUserDto>();

        var user = _fixture.Create<UserEntity>();

        var publicUserResponse = _fixture.Create<PublicUserResponse>();

        _userRepositoryMock.Setup(repo => repo.GetInfoByIdAsync(userDto.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _mapperMock.Setup(mapper => mapper.Map<PublicUserResponse>(user))
            .Returns(publicUserResponse);

        var result = await _userService.GetAsync(userDto, CancellationToken.None);

        Assert.NotNull(result);
        _userRepositoryMock.Verify(repo => repo.GetInfoByIdAsync(userDto.Id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAsync_ShouldThrowNotFoundException_WhenUserDoesNotExist()
    {
        var user = _fixture.Create<InfoUserDto>();

        _userRepositoryMock.Setup(repo => repo.GetInfoByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserEntity)null);

        await Assert.ThrowsAsync<NotFoundException>(() =>
           _userService.GetAsync(user, CancellationToken.None));

        _userRepositoryMock.Verify(repo => repo.GetInfoByIdAsync(user.Id, It.IsAny<CancellationToken>()), Times.Once);
    }
}