using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server_Ad_Baturina.Extension;
using Server_Ad_Baturina.Models.DTOs;
using Server_Ad_Baturina.Models.Requests;
using Server_Ad_Baturina.Models.Responses;
using Server_advanced_Baturina.Interfaces;
using Server_advanced_Baturina.Models.DTOs;

namespace Server_advanced_Baturina.Controllers;

[Route("api/v1/news")]
[ApiController]
[Authorize]

public class NewsController : ControllerBase
{
    private readonly INewsService _newsService;
    private readonly IMapper _mapper;

    public NewsController(INewsService newsService, IMapper mapper)
    {
        _newsService = newsService;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<ActionResult<CustomSuccessResponse<CreateNewsSuccessResponse>>> CreateAsync(
        NewsRequest newsRequest, 
        CancellationToken cancellationToken)
    {
        var newsDto = _mapper.Map<NewsDto>(newsRequest);
        var userId = HttpContext.GetUserId();
        newsDto.UserId = userId;

        var newsId = await _newsService.CreateAsync(newsDto, cancellationToken);

        var response = new CustomSuccessResponse<CreateNewsSuccessResponse>
        {
            Data = newsId,
            StatusCode = StatusCodes.Status201Created,
            Success = true
        };

        return Created(nameof(CreateAsync), response);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<BaseSuccessResponse>> PutAsync(
        [FromRoute]Guid id,
        [FromBody] NewsRequest newsRequest, 
        CancellationToken cancellationToken)
    {
        var newsDto = _mapper.Map<NewsUpdateDto>(newsRequest);
        newsDto.Id = id;

        await _newsService.PutAsync(newsDto, cancellationToken);

        var response = new BaseSuccessResponse
        {
            StatusCode = StatusCodes.Status200OK,
            Success = true
        };

        return Ok(response);
    }

    [HttpDelete("{id}")]

    public async Task<ActionResult<BaseSuccessResponse>> DeleteAsync(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var newsDto = _mapper.Map<DeleteNewsDto>(id);

        await _newsService.DeleteAsync(newsDto, cancellationToken);

        var response = new BaseSuccessResponse
        {
            StatusCode = StatusCodes.Status200OK,
            Success = true
        };

        return Ok(response);
    }

    [HttpGet]
    public async Task<ActionResult<CustomSuccessResponse<PageableResponse<List<GetNewsOutDto>>>>> GetAsync(
        [FromQuery] int page, 
        [FromQuery] int perPage, 
        CancellationToken cancellationToken)
    {
        var news = await _newsService.GetPaginatedAsync(page, perPage, cancellationToken);

        var response = new CustomSuccessResponse<PageableResponse<List<GetNewsOutDto>>>
        {
            Data = news,
            StatusCode = StatusCodes.Status200OK,
            Success = true
        };

        return Ok(response);
    }
    
    [HttpGet("find")]
    public async Task<ActionResult<CustomSuccessResponse<PageableResponse<List<GetNewsOutDto>>>>> FindAsync(
        [FromQuery]string? author,
        [FromQuery] string? keywords, 
        [FromQuery] string[]? tags,
        [FromQuery] int page, 
        [FromQuery] int perPage,
        CancellationToken cancellationToken)
    {
        var news = await _newsService.FindAsync(author, keywords, tags, page, perPage, cancellationToken);

        var response = new CustomSuccessResponse<PageableResponse<List<GetNewsOutDto>>>
        {
            Data = news,
            StatusCode = StatusCodes.Status200OK,
            Success = true
        };

        return Ok(response);
    }

    [HttpGet("userId/{id}")]
    public async Task<ActionResult<CustomSuccessResponse<PageableResponse<List<GetNewsOutDto>>>>> GetUserAsync(
        [FromQuery] int page, 
        [FromQuery] int perPage, 
        [FromRoute]Guid id,
        CancellationToken cancellationToken)
    {
        var news = await _newsService.GetUserAsync(page, perPage, id, cancellationToken);

        var response = new CustomSuccessResponse<PageableResponse<List<GetNewsOutDto>>>
        {
            Data = news,
            StatusCode = StatusCodes.Status200OK,
            Success = true
        };

        return Ok(response);
    }
}