using Server_Ad_Baturina.Models.DTOs;
using Server_Ad_Baturina.Models.Entities;

namespace Server_advanced_Baturina.Interfaces;

public interface INewsRepository
{
    Task CreateAsync(NewsEntity newsEntity, CancellationToken cancellationToken);

    Task PutAsync(Guid id, string description, string image, string[] tags, string title, CancellationToken cancellationToken);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken);

    Task<bool> FindAsync(Guid id, CancellationToken cancellationToken);

    Task<List<NewsEntity>> GetPaginatedAsync(int page, int perPage, CancellationToken cancellationToken);

    Task<long> GetAllAsync(CancellationToken cancellationToken);

    Task<NewsAndCount> FilterAsync(string author, string keywords, string[] tags, int page, int perPage, CancellationToken cancellationToken);

    Task<NewsAndCount> GetUserNewsAsync(int page, int perPage, Guid id, CancellationToken cancellationToken);
}