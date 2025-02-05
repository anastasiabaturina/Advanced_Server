using System.Net;

namespace Server_Ad_Baturina.Models.Responses;

public class BaseSuccessResponse
{
    public int StatusCode { get; set; }

    public bool Success { get; set; }

    public Error? Error { get; set; }
}