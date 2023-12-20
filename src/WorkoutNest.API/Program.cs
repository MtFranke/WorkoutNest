using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using WorkoutNest.API;


var builder = WebApplication.CreateBuilder(args);

//Add support to logging with SERILOG
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

builder.Services
    .AddAuthorization()
    .AddCors()
    .AddFastEndpoints()
    .SwaggerDocument();

builder.Services.AddSingleton<IJwtToken, JwtTokenGenerator>();
builder.Services.AddApplicationInsightsTelemetry();

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer =  builder.Configuration["JwtToken:Issuer"], // Update with your actual issuer
            ValidateAudience = false,
            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JwtToken:SecretKey"])),

        };
        
    });
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGet("test", [Authorize]() =>
{
    // Access the claims for the current user
    
});

app.UseCors(x => x
    .AllowAnyMethod()
    .AllowAnyHeader()
    .SetIsOriginAllowed(origin => true) // allow any origin
    //.WithOrigins("https://localhost:44351")); // Allow only this origin can also have multiple origins separated with comma
    .AllowCredentials()); // allow credentials
//Add support to logging request with SERILOG
app.UseSerilogRequestLogging();
app
    .UseFastEndpoints()
    .UseSwaggerGen();
app.UseAuthentication();
app.UseAuthorization();
app.Run();
