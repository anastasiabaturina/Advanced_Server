using System.Text.Json.Serialization;

namespace Server_Ad_Baturina.Models.Entities;

public class NewsEntity
{
    public Guid Id { get; set; }

    public string? Description { get; set; }

    public string? Image { get; set; }

    public string Title { get; set; }

    public Guid UserId { get; set; }

    public UserEntity User { get; set; }

    [JsonIgnore]
    public List<TagsEntity>? Tags { get; set; }
}