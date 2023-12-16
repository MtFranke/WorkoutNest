using FastEndpoints;
using MongoDB.Driver;
using WorkoutNest.Infrastructure.Mongo.Collections;

namespace WorkoutNest.API.Auth;

public class RegisterEndpoint : Endpoint<RegisterRequest, RegisterResponse>
{
    private string mongoDbConnectionString;
    public RegisterEndpoint(IConfiguration configuration)
    {
        mongoDbConnectionString = configuration["MongoDbConnectionString"];
    }
    
    public override void Configure()
    {
        Post("/auth/register");
        AllowAnonymous();
    }
    
    public override async Task HandleAsync(RegisterRequest r, CancellationToken c)
    {
        var id = Guid.NewGuid();
        var client = new MongoClient(mongoDbConnectionString);
        var db = client.GetDatabase("workoutnest");
        var users = db.GetCollection<User>("users");
        users.InsertOne(new User()
        {
            Id = id.ToString(),
            Password = r.Password,
            Email = r.Email,
            Username = r.Username,
        });

        await SendAsync(new() { Id = id }, cancellation: c);
    }
}

public class RegisterRequest
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}

public class RegisterResponse
{
    public Guid Id { get; set; }
}