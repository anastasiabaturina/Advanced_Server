using System.Security.Claims;

namespace Server_Ad_Baturina.Extension;

public static class HttpContextExtensions
{
    public static Guid GetUserId(this HttpContext httpContext)
    {
        return Guid.Parse(httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
    }
}