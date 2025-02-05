namespace Server_Ad_Baturina.Models.Responses;

public class PageableResponse<T>
{
    public T? Content { get; set; }

    public long? NumberOfElement { get; set; }
}