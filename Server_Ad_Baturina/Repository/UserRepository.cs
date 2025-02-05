using Microsoft.EntityFrameworkCore;
using Server_Ad_Baturina.Models.Entities;
using Server_advanced_Baturina.Interfaces;
using Server_advanced_Baturina.Models;

namespace Server_advanced_Baturina.Repository;

public class UserRepository : IUserRepository
{
    private readonly Context _context;

    public UserRepository(Context context)
    {
        _context = context;
    }

    public async Task RegisterAsync(
        UserEntity user,
        CancellationToken cancellationToken)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<UserEntity?> FindAsync(string email, CancellationToken cancellationToken) => await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    

    public async Task<List<UserEntity>> GetAllAsync(CancellationToken cancellationToken) => await _context.Users.ToListAsync(cancellationToken);

    public async Task<UserEntity> ReplaceAsync(
        Guid id, 
        string avatar, 
        string email, 
        string name, 
        string role, 
        CancellationToken cancellationToken)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        user.Email = email;
        user.Avatar = avatar;
        user.Name = name;
        user.Role = role;

        await _context.SaveChangesAsync(cancellationToken);

        return user;
    }

    public async Task DeleteAsync(
        Guid id, 
        CancellationToken cancellationToken)
    {
        var userEntity = await _context.Users.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        _context.Users.Remove(userEntity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<UserEntity> GetInfoByIdAsync(Guid id, CancellationToken cancellationToken) => await _context.Users.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);   
}