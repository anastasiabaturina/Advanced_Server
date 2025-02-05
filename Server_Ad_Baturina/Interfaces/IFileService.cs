using Server_Ad_Baturina.Models.DTOs;
using Server_Ad_Baturina.Models.Responses;

namespace Server_Ad_Baturina.Interfaces;

public interface IFileService
{
    Task<UploadFileResponse> UploadAsync(UploadFileDto uploadFileDto, CancellationToken cancellationToken);

    Task<Stream> GetAsync(string fileName, CancellationToken cancellationToken);
}