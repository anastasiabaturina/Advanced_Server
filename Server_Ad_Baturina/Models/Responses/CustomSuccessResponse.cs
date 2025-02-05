namespace Server_Ad_Baturina.Models.Responses;

public class CustomSuccessResponse<T> : BaseSuccessResponse
{
    public T? Data { get; set; }
}