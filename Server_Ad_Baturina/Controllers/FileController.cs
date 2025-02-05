using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Server_Ad_Baturina.Interfaces;
using Server_Ad_Baturina.Models.DTOs;
using Server_Ad_Baturina.Models.Requests;
using Server_Ad_Baturina.Models.Responses;

namespace Server_advanced_Baturina.Controllers;

[Route("api/v1/files")]
[ApiController]

public class FileController : ControllerBase
{
    public readonly IMapper _mapper;
    public readonly IFileService _fileService;

    public FileController(IMapper mapper, IFileService fileService)
    {
        _mapper = mapper;
        _fileService = fileService;
    }

    [HttpPost]
    public async Task<ActionResult<BaseSuccessResponse>> UploadAsync(
        UploadFileRequest uploadFileRequest, 
        CancellationToken cancellationToken)
    {
        var file = _mapper.Map<UploadFileDto>(uploadFileRequest);

        var data =  await _fileService.UploadAsync(file, cancellationToken);

        var response = new CustomSuccessResponse<UploadFileResponse>
        {
            Data = data,
            StatusCode = StatusCodes.Status201Created,
            Success = true
        };

        return Created(nameof(UploadAsync), response); 
    }

    [HttpGet("{filename}")]
    public async Task<ActionResult<Stream>> GetFileAsync(
        [FromRoute]string fileName,
        CancellationToken cancellationToken)
    {
        var fileResponse = await _fileService.GetAsync(fileName, cancellationToken);

        return File(fileResponse, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
    }
}