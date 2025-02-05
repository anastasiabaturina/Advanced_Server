namespace Server_Ad_Baturina.Models.DTOs;

public class NewsUpdateDto
{
    public Guid Id { get; set; }

    public string Description { get; set; }

    public string Image { get; set; }

    public string[] Tags { get; set; }

    public string Title { get; set; }
}