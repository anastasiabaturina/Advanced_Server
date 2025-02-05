using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server_Ad_Baturina.Extension;
using Server_Ad_Baturina.Models.DTOs;
using Server_Ad_Baturina.Models.Requests;
using Server_Ad_Baturina.Models.Responses;
using Server_advanced_Baturina.Interfaces;

namespace Server_advanced_Baturina.Controllers;

[Route("api/v1/users")]
[ApiController]
[Authorize]
public class UserController : Controller
{
    private readonly IUserService _userService;
    private readonly IMapper _mapper;

    public UserController(IUserService userService, IMapper mapper)
    {
        _userService = userService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<CustomSuccessResponse<List<PublicUserResponse>>>> GetAllAsync(CancellationToken cancellationToken)
    {
        var users = await _userService.GetAllAsync(cancellationToken);

        var response = new CustomSuccessResponse<List<PublicUserResponse>>
        {
            Data = users,
            StatusCode = StatusCodes.Status200OK,
            Success = true
        };

        return Ok(response);
    }

    [HttpPut]
    public async Task<ActionResult<CustomSuccessResponse<PutUserResponse>>> UpdateAsync(
        [FromBody] PutUserRequest putUserRequest, 
        CancellationToken cancellationToken)
    {
        var userId = HttpContext.GetUserId();

        var putUserDto = _mapper.Map<PutUserDto>(putUserRequest);
        putUserDto.Id = userId;

        var user = await _userService.ReplaseAsync(putUserDto, cancellationToken);

        var response = new CustomSuccessResponse<PutUserResponse>
        {
            Data = user,
            StatusCode = StatusCodes.Status200OK,
            Success = true
        };

        return Ok(response);
    }

    [HttpDelete]
    public async Task<ActionResult<BaseSuccessResponse>> DeleteAsync(CancellationToken cancellationToken)
    {
        var userId = HttpContext.GetUserId();

        var deleteUserdto = _mapper.Map<DeleteUserDto>(userId);

        await _userService.DeleteAsync(deleteUserdto, cancellationToken);

        var response = new BaseSuccessResponse
        {
            StatusCode = StatusCodes.Status200OK,
            Success = true
        };

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CustomSuccessResponse<PublicUserResponse>>> GetForIdAsync(
        [FromRoute] Guid id, 
        CancellationToken cancellationToken)
    {
        var infoDto = _mapper.Map<InfoUserDto>(id);

        var user = await _userService.GetAsync(infoDto, cancellationToken);

        var response = new CustomSuccessResponse<PublicUserResponse>
        {
            Data = user,
            StatusCode = StatusCodes.Status200OK,
            Success = true
        };

        return Ok(response);
    }

    [HttpGet("find")]
    public async Task<ActionResult<CustomSuccessResponse<PublicUserResponse>>> GetAsync(CancellationToken cancellationToken)
    { 
        var id = HttpContext.GetUserId();

        var infoDto = _mapper.Map<InfoUserDto>(id);

        var user = await _userService.GetAsync(infoDto, cancellationToken);

        var response = new CustomSuccessResponse<PublicUserResponse>
        {
            Data = user,
            StatusCode = StatusCodes.Status200OK,
            Success = true
        };

        return Ok(response);
    }
}