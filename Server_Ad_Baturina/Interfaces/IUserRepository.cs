using Server_Ad_Baturina.Models.Entities;

namespace Server_advanced_Baturina.Interfaces;

public interface IUserRepository
{
    Task RegisterAsync(UserEntity user, CancellationToken cancellationToken);

    Task<UserEntity?> FindAsync(string email, CancellationToken cancellationToken);

    Task<List<UserEntity>> GetAllAsync(CancellationToken cancellationToken);

    Task<UserEntity> ReplaceAsync(Guid id, string avatar, string email, string name, string role, CancellationToken cancellationToken);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken);

    Task<UserEntity> GetInfoByIdAsync(Guid id, CancellationToken cancellationToken);
}