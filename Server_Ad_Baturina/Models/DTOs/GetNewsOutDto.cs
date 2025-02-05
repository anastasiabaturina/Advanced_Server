using Server_Ad_Baturina.Models.Entities;

namespace Server_advanced_Baturina.Models.DTOs;

public class GetNewsOutDto
{
    public string Description { get; set; }

    public Guid Id { get; set; }

    public string Image { get; set; }

    public List<TagsEntity> Tags { get; set; }

    public string Title { get; set; }

    public Guid UserId { get; set; }

    public string Username { get; set; }
}