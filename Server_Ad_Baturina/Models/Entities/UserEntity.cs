using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server_Ad_Baturina.Models.Entities;
public class UserEntity
{
    public Guid Id { get; set; }

    public string? Avatar { get; set; }

    public string? Email { get; set; }

    public string Name { get; set; }

    public string Password { get; set; }

    public string Role { get; set; }
}