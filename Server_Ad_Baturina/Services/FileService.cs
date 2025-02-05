using Server_Ad_Baturina.Exceptions;
using Server_Ad_Baturina.Interfaces;
using Server_Ad_Baturina.Models.DTOs;
using Server_Ad_Baturina.Models.Responses;
namespace Server_Ad_Baturina.Services;

public class FileService : IFileService
{
    private readonly IWebHostEnvironment _webHostEnvironment;

    public FileService(IWebHostEnvironment webHostEnvironment)
    {
        _webHostEnvironment = webHostEnvironment;
    }

    public async Task<UploadFileResponse> UploadAsync(
        UploadFileDto uploadFileDto, 
        CancellationToken cancellationToken)
    {
        var uniqueName = $"{Guid.NewGuid()}_{uploadFileDto.File.FileName.Replace(" ", "_")}";
        var path = Path.Combine(_webHostEnvironment.WebRootPath, uniqueName);

        await using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read, 4096, useAsync: true))
        {
            await uploadFileDto.File.CopyToAsync(stream, cancellationToken);
        }

        var response = new UploadFileResponse
        {
            FileName = uniqueName,
        };

        return response;
    }

    public async Task<Stream> GetAsync(
        string fileName, 
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            throw new ArgumentException("The file name cannot be empty");
        }

        var filePath = Path.Combine(_webHostEnvironment.WebRootPath, fileName);


        if (!File.Exists(filePath))
        {
            throw new NotFoundException("File not found");
        }

        var memoryStream = new MemoryStream();

        await using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true))
        {
            await fileStream.CopyToAsync(memoryStream, cancellationToken);
        }

        memoryStream.Seek(0, SeekOrigin.Begin);

        return memoryStream;
    }
}