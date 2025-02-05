using Server_Ad_Baturina.Models.DTOs;
using Server_Ad_Baturina.Models.Responses;
using Server_advanced_Baturina.Models.DTOs;

namespace Server_advanced_Baturina.Interfaces;

public interface INewsService
{
    Task<CreateNewsSuccessResponse> CreateAsync(NewsDto newsDto, CancellationToken cancellationToken);

    Task PutAsync(NewsUpdateDto newsDto, CancellationToken cancellationToken);

    Task DeleteAsync(DeleteNewsDto newsDto, CancellationToken cancellationToken);

    Task<PageableResponse<List<GetNewsOutDto>>> GetPaginatedAsync(int page, int perPage, CancellationToken cancellationToken);

    Task<PageableResponse<List<GetNewsOutDto>>> FindAsync(string? author, string? keywords, string[]? tags, int page, int perPage, CancellationToken cancellationToken);

    Task<PageableResponse<List<GetNewsOutDto>>> GetUserAsync(int page, int perPage, Guid id, CancellationToken cancellationToken);
}