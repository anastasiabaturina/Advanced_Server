using AutoMapper;
using Server_Ad_Baturina.Exceptions;
using Server_Ad_Baturina.Models.DTOs;
using Server_Ad_Baturina.Models.Entities;
using Server_Ad_Baturina.Models.Responses;
using Server_advanced_Baturina.Interfaces;
using Server_advanced_Baturina.Models.DTOs;
using System.Data;

namespace Server_advanced_Baturina.Services;

public class NewsService : INewsService
{
    private readonly INewsRepository _newsRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public NewsService(INewsRepository newsRepository, IUserRepository userRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor)
    {
        _newsRepository = newsRepository;
        _userRepository = userRepository;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<CreateNewsSuccessResponse> CreateAsync(
        NewsDto newsDto, 
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetInfoByIdAsync(newsDto.UserId, cancellationToken);

        if(user == null)
        {
            throw new NotFoundException("The user was not found");
        }

        var news = _mapper.Map<NewsEntity>(newsDto);
        news.Image = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}/api/v1/files/{newsDto.Image}";
        news.User = user;
        news.Tags = newsDto.Tags.Distinct().Select(x => new TagsEntity { Title = x }).ToList();

        await _newsRepository.CreateAsync(news, cancellationToken);

        var result = _mapper.Map<CreateNewsSuccessResponse>(news);

        return result;
    }

    public async Task PutAsync(NewsUpdateDto newsDto, CancellationToken cancellationToken)
    {
        var image = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}/api/v1/files/{newsDto.Image}";
        await _newsRepository.PutAsync(newsDto.Id, newsDto.Description, image, newsDto.Tags,newsDto.Title, cancellationToken);
    }


    public async Task DeleteAsync(
        DeleteNewsDto newsDto,
        CancellationToken cancellationToken)
    {
        if(!await _newsRepository.FindAsync(newsDto.Id, cancellationToken))
        {
            throw new NotFoundException("The news was not found");
        }

        await _newsRepository.DeleteAsync(newsDto.Id, cancellationToken);
    }

    public async Task<PageableResponse<List<GetNewsOutDto>>> GetPaginatedAsync(
        int page, 
        int perPage, 
        CancellationToken cancellationToken)
    {
        var news = await _newsRepository.GetPaginatedAsync(page, perPage, cancellationToken);

        var result = new PageableResponse<List<GetNewsOutDto>>
        {
            Content = news.Select(x => new GetNewsOutDto
            {
                Description = x.Description,
                Id = x.Id,
                Image = x.Image,
                Title = x.Title,
                Tags = x.Tags.Select(t => new TagsEntity() { Id = t.Id, Title = t.Title }).ToList(),
                UserId = x.User.Id,
                Username = x.User.Name
            }).ToList(),
            NumberOfElement = await _newsRepository.GetAllAsync(cancellationToken)
        };

        return result;
    }

    public async Task<PageableResponse<List<GetNewsOutDto>>> FindAsync(
        string? author, 
        string? keywords, 
        string[]? tags, 
        int page, 
        int perPage, 
        CancellationToken cancellationToken)
    {
        var listNews = await _newsRepository.FilterAsync(author, keywords, tags, page, perPage, cancellationToken);

        var result = new PageableResponse<List<GetNewsOutDto>>
        {
            Content = listNews.News.Select(x => new GetNewsOutDto
            {
                Description = x.Description,
                Id = x.Id,
                Image = x.Image,
                Title = x.Title,
                Tags = x.Tags.Select(t => new TagsEntity() { Id = t.Id, Title = t.Title }).ToList(),
                UserId = x.User.Id,
                Username = x.User.Name
            }).ToList(),
            NumberOfElement = listNews.Count
        };

        return result;
    }

    public async Task<PageableResponse<List<GetNewsOutDto>>> GetUserAsync(
        int page, 
        int perPage, 
        Guid id, 
        CancellationToken cancellationToken)
    {
        var listNews = await _newsRepository.GetUserNewsAsync(page, perPage, id, cancellationToken);

        var result = new PageableResponse<List<GetNewsOutDto>>
        {
            Content = listNews.News.Select(x => new GetNewsOutDto
            {
                Description = x.Description,
                Id = x.Id,
                Image = x.Image,
                Title = x.Title,
                Tags = x.Tags.Select(t => new TagsEntity() { Id = t.Id, Title = t.Title }).ToList(),
                UserId = x.User.Id,
                Username = x.User.Name
            }).ToList(),
            NumberOfElement = listNews.Count
        };

        return result; 
    }
}