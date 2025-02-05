using System.Text.Json.Serialization;

namespace Server_Ad_Baturina.Models.Entities;

public class TagsEntity
{
    public Guid Id { get; set; }

    public string Title { get; set; }

    [JsonIgnore]
    public List<NewsEntity> News { get; set; }
}