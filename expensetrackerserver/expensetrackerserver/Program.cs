using expensetrackerserver.Data;
using expensetrackerserver.Middleware;
using expensetrackerserver.Repositories;
using expensetrackerserver.Services;
using expensetrackerserver.Middleware;
using Microsoft.AspNetCore.Mvc;
using expensetrackerserver.DTOs;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<DapperContext>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();


builder.Services.AddControllers();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(x => x.Value!.Errors.Count > 0)
            .ToDictionary(
            x => x.Key,
            x => x.Value!.Errors
                 .Select(e => e.ErrorMessage)
                 .ToArray());

        var response = new ErrorResponseDto
        {
            StatusCode = StatusCodes.Status400BadRequest,
            Message = "Validation Failed.",
            Errors = errors
        };

        return new BadRequestObjectResult(response);


    };
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseMiddleware<GlobalExceptionMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
