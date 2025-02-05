using Server_Ad_Baturina.Middlewaries;
using Server_Ad_Baturina.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddServices();

var app = builder.Build();

app.UseMiddleware<ExceptionHandingMiddleware>();

app.UseStaticFiles();

app.MapControllers();

app.UseAuthentication();

app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();


