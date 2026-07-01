using System.Text.Json;
using Microsoft.AspNetCore.Http;
using expensetrackerserver.Exceptions;
using expensetrackerserver.DTOs;


namespace expensetrackerserver.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(
            HttpContext context,
            Exception exception)
        {
            context.Response.ContentType = "application/json";
            int statusCode = exception switch
            {
                EmailAlreadyExistsException => StatusCodes.Status409Conflict,
                UsernameAlreadyExistsException => StatusCodes.Status409Conflict,
                InvalidPasswordException => StatusCodes.Status400BadRequest,
                InvalidPreferredCalendarException => StatusCodes.Status400BadRequest,
                InvalidCredentialsException => StatusCodes.Status404NotFound,
                UserNotFoundException => StatusCodes.Status404NotFound,
                _ => StatusCodes.Status500InternalServerError
            };

            context.Response.StatusCode = statusCode;

            var response = new ErrorResponseDto
            {
                StatusCode = statusCode,
                Message = exception.Message
            };

            var json = JsonSerializer.Serialize(response);

            await context.Response.WriteAsync(json);
        }
    }
}
