using FastEndpoints;
using FastEndpoints.Swagger;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddCors()
    .AddFastEndpoints()
    .SwaggerDocument();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.UseCors(x => x
    .AllowAnyMethod()
    .AllowAnyHeader()
    .SetIsOriginAllowed(origin => true) // allow any origin
    //.WithOrigins("https://localhost:44351")); // Allow only this origin can also have multiple origins separated with comma
    .AllowCredentials()); // allow credentials

app
    .UseFastEndpoints()
    .UseSwaggerGen();
app.Run();
