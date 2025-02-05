using Server_Ad_Baturina.Exceptions;
using Server_Ad_Baturina.Models.Responses;
using System.Security.Authentication;

namespace Server_Ad_Baturina.Middlewaries;

public class ExceptionHandingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ArgumentException ex)
        {
            var code = StatusCodes.Status400BadRequest;
            var errorMessage = ex.Message;
            await HandleExceptionAsync(context, code, errorMessage);
        }
        catch (AuthenticationException ex)
        {
            var code = StatusCodes.Status401Unauthorized;
            var errorMessage = ex.Message;
            await HandleExceptionAsync(context, code, errorMessage);
        }
        catch (NotFoundException ex)
        {
            var code = StatusCodes.Status404NotFound;
            var errorMessage = ex.Message;
            await HandleExceptionAsync(context, code, errorMessage);
        }
        catch (BadRequestException ex)
        {
            var code = StatusCodes.Status400BadRequest;
            var errorMessage = ex.Message;
            await HandleExceptionAsync(context, code, errorMessage);
        }
        catch (Exception)
        {
            var code = StatusCodes.Status500InternalServerError;
            var errorMessage = "Internal server error.";
            await HandleExceptionAsync(context, code, errorMessage);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, int code, string errorMessage)
    {
        var response = new BaseSuccessResponse
        {
            StatusCode = code,
            Success = false,
            Error = new Error
            {
                Message = errorMessage,
            }
        };

        return context.Response.WriteAsJsonAsync(response);
    }
}