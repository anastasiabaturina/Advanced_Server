using AutoFixture;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Moq;
using Server_Ad_Baturina.Exceptions;
using Server_Ad_Baturina.Models.DTOs;
using Server_Ad_Baturina.Models.Entities;
using Server_Ad_Baturina.Models.Responses;
using Server_advanced_Baturina.Interfaces;
using Server_advanced_Baturina.Models;
using Server_advanced_Baturina.Models.DTOs;
using Server_advanced_Baturina.Services;

namespace Server_Ad_Baturina.Tests;

public class NewsServiceTests
{
    private readonly IFixture _fixture;
    private readonly Mock<INewsRepository> _newsRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IHttpContextAccessor> _contextAccessorMock;
    private readonly NewsService _newsService;

    public NewsServiceTests()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        _newsRepositoryMock = new Mock<INewsRepository>();
        _mapperMock = new Mock<IMapper>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _contextAccessorMock = new Mock<IHttpContextAccessor>();

        _newsService = new NewsService(
            _newsRepositoryMock.Object,
            _userRepositoryMock.Object,
            _mapperMock.Object,
            _contextAccessorMock.Object);
    }

    [Fact]
    public async Task CreateAsync_ReturnCreateNewsSuccessResponse()
    {
        var newsDto = _fixture.Create<NewsDto>();
        var user = _fixture.Create<UserEntity>();
        var response = _fixture.Create<CreateNewsSuccessResponse>();
        var news = _fixture.Create<NewsEntity>();

        _userRepositoryMock.Setup(repo => repo.GetInfoByIdAsync(newsDto.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _mapperMock.Setup(mapper => mapper.Map<NewsEntity>(newsDto))
            .Returns(news);

        _contextAccessorMock.Setup(x => x.HttpContext.Request.Scheme).Returns("https");
        _contextAccessorMock.Setup(x => x.HttpContext.Request.Host).Returns(new HostString("localhost"));

        _newsRepositoryMock.Setup(repo => repo.CreateAsync(news, It.IsAny<CancellationToken>()));

        _mapperMock.Setup(mapper => mapper.Map<CreateNewsSuccessResponse>(news))
            .Returns(response);

        var result = await _newsService.CreateAsync(newsDto, CancellationToken.None);

        Assert.NotNull(result);
        Assert.IsType<CreateNewsSuccessResponse>(result);
        _userRepositoryMock.Verify(repo => repo.GetInfoByIdAsync(newsDto.UserId, It.IsAny<CancellationToken>()), Times.Once);
        _newsRepositoryMock.Verify(repo => repo.CreateAsync(news, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowNotFoundException_WhenUserNotFound()
    {
        var newsDto = _fixture.Create<NewsDto>();
  
        _userRepositoryMock.Setup(x => x.GetInfoByIdAsync(newsDto.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserEntity)null); 

        await Assert.ThrowsAsync<NotFoundException>(() => _newsService.CreateAsync(newsDto, CancellationToken.None));
    }

    [Fact]
    public async Task PutAsync()
    {
        var newsDto = _fixture.Create<NewsUpdateDto>();

        _contextAccessorMock.Setup(x => x.HttpContext.Request.Scheme).Returns("https");
        _contextAccessorMock.Setup(x => x.HttpContext.Request.Host).Returns(new HostString("localhost"));
        
        await _newsService.PutAsync(newsDto, CancellationToken.None);

        _newsRepositoryMock.Verify(repo => repo.PutAsync(newsDto.Id, newsDto.Description, It.IsAny<string>(), newsDto.Tags, newsDto.Title, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteNewsSuccessfully()
    {
        var newsDto = _fixture.Create<DeleteNewsDto>();

        _newsRepositoryMock.Setup(repo => repo.FindAsync(newsDto.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _newsRepositoryMock.Setup(x => x.DeleteAsync(newsDto.Id, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await _newsService.DeleteAsync(newsDto, CancellationToken.None);

        _newsRepositoryMock.Verify(x => x.DeleteAsync(newsDto.Id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrowNotFoundException_WhenNewsNotFound()
    {
        var deleteDto = _fixture.Build<DeleteNewsDto>().Create();
        _newsRepositoryMock.Setup(repo => repo.FindAsync(deleteDto.Id, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        await Assert.ThrowsAsync<NotFoundException>(() => _newsService.DeleteAsync(deleteDto, CancellationToken.None));
    }

    [Fact]
    public async Task GetPaginatedAsync_ReturnPageableResponseListGetNewsOutDto()
    {
        var page = _fixture.Create<int>();
        var perPage = _fixture.Create<int>();

        var news = _fixture.CreateMany<NewsEntity>(10).ToList();

        _newsRepositoryMock.Setup(repo => repo.GetPaginatedAsync(page, perPage, It.IsAny<CancellationToken>()))
            .ReturnsAsync(news);

        _newsRepositoryMock.Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(10);

        var result = await _newsService.GetPaginatedAsync(page, perPage, CancellationToken.None);

        Assert.Equal(10, result.NumberOfElement);
        Assert.Equal(10, result.Content.Count);
    }

    [Fact]
    public async Task FindAsync_returnPagebleResponse()
    {
        var author = _fixture.Create<string>();
        var keywords = _fixture.Create<string>();
        var tags = _fixture.Create<string[]>();
        var page = _fixture.Create<int>();
        var perPage = _fixture.Create<int>();

        var news = _fixture.Build<NewsAndCount>()
            .With(n => n.Count, 10)
            .Create();

        _newsRepositoryMock.Setup(repo => repo.FilterAsync(author, keywords, tags, page, perPage, It.IsAny<CancellationToken>())).
            ReturnsAsync(news);

        var result = await _newsService.FindAsync(author, keywords, tags, page, perPage, CancellationToken.None);

        Assert.IsType<PageableResponse<List<GetNewsOutDto>>> (result);
        Assert.Equal(10, result.NumberOfElement);
    }

    [Fact]
    public async Task GetUserAsync_ReturnPageableResponseListGetNewsOutDto()
    {
        var page = _fixture.Create<int>();
        var perPage = _fixture.Create<int>();
        var id = _fixture.Create<Guid>();

        var news = _fixture.Build<NewsAndCount>()
            .With(n => n.Count, 10)
            .Create();

        _newsRepositoryMock.Setup(repo => repo.GetUserNewsAsync(page, perPage, id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(news);

        var result = await _newsService.GetUserAsync(page, perPage, id, CancellationToken.None);

        Assert.IsType<PageableResponse<List<GetNewsOutDto>>>(result);
        Assert.Equal(10, result.NumberOfElement);
    }
}