using System.Security.Authentication;
using FastEndpoints;
using MongoDB.Driver;
using WorkoutNest.Infrastructure.Mongo;
using WorkoutNest.Infrastructure.Mongo.Entities;

namespace WorkoutNest.API.Auth;

public class LoginEndpoint : Endpoint<LoginRequest, LoginResponse>
{
    private string mongoDbConnectionString;

    public LoginEndpoint(IConfiguration configuration)
    {
        mongoDbConnectionString = configuration["MongoDbConnectionString"];
    }
    public override void Configure()
    {
        Post("/auth/login");
        AllowAnonymous();
    }
    
    public override async Task HandleAsync(LoginRequest r, CancellationToken c)
    {

        var client = new MongoClient(mongoDbConnectionString);
        var db = client.GetDatabase("workoutnest");
        var users = db.GetCollection<User>(Collections.UsersCollection);

        var user = await (await users.FindAsync(x => x.Username == r.Username && x.Password == r.Password, cancellationToken: c))
            .SingleOrDefaultAsync(cancellationToken: c);

        if (user == null)
        {
            throw new AuthenticationException("Please provide correct username nad password.");
        }
        
        
        await SendAsync(new() { } , cancellation: c);
    }
}


public class LoginRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
}

public class LoginResponse{
    
}