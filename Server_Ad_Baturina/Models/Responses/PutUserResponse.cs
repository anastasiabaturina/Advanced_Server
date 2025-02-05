namespace Server_Ad_Baturina.Models.Responses;

public class PutUserResponse
{
    public string Avatar { get; set; }

    public string Email { get; set; }

    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Role { get; set; }
}