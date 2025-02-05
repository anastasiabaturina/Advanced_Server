namespace Server_Ad_Baturina.Models.Requests;

public class NewsRequest
{
    public string Description { get; set; }

    public string Image { get; set; }

    public string[] Tags { get; set; }

    public string Title { get; set; }
}