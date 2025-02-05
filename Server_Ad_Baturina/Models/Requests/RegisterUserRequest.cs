namespace Server_Ad_Baturina.Models.Requests;

public class RegisterUserRequest
{
    public string Avatar { get; set; }

    public string Email { get; set; }

    public string Name { get; set; }

    public string Password { get; set; }

    public string Role { get; set; }
}