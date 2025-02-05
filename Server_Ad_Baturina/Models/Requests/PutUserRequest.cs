namespace Server_Ad_Baturina.Models.Requests;

public class PutUserRequest
{
    public string Avatar { get; set; }

    public string Email { get; set; }

    public string Name { get; set; }

    public string Role { get; set; }
}