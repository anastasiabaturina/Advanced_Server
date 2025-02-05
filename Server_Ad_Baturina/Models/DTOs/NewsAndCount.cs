using Server_Ad_Baturina.Models.Entities;

namespace Server_Ad_Baturina.Models.DTOs;

public record NewsAndCount
{
    public List<NewsEntity>? News { get; set; }
    public long Count { get; set; }
}
