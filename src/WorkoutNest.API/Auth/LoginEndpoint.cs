using System.Security.Authentication;
using System.Security.Claims;
using FastEndpoints;
using MongoDB.Driver;
using WorkoutNest.Infrastructure.Mongo;
using WorkoutNest.Infrastructure.Mongo.Entities;

namespace WorkoutNest.API.Auth;

internal class LoginEndpoint : Endpoint<LoginRequest, LoginResponse>
{
    private readonly IJwtToken _jwtToken;
    private readonly string _mongoDbConnectionString;
    private readonly string _mongoDb;

    public LoginEndpoint(IConfiguration configuration, IJwtToken jwtToken)
    {
        _jwtToken = jwtToken;
        _mongoDbConnectionString = configuration["MongoDbConnectionString"];
        _mongoDb = configuration["MongoDb"];
    }
    public override void Configure()
    {
        Post("/auth/login");
        AllowAnonymous();
    }
    
    public override async Task HandleAsync(LoginRequest r, CancellationToken c)
    {

        var client = new MongoClient(_mongoDbConnectionString);
        var db = client.GetDatabase(_mongoDb);
        var users = db.GetCollection<User>(Collections.UsersCollection);
        
        var user = await (await users.FindAsync(x => x.Username == r.Username && x.Password == r.Password, cancellationToken: c))
            .SingleOrDefaultAsync(cancellationToken: c);
        
        if (user == null)
        {
            throw new AuthenticationException("Please provide correct username nad password.");
        }

        var tokenG = _jwtToken.GenerateToken(Guid.NewGuid().ToString(), new []
        {
            new Claim("user_id", user.Id),
            new Claim("roles", "workout-nest.user")
        });
        
        await SendAsync(new() {AccessToken = tokenG} , cancellation: c);
    }
}


public class LoginRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
}

public class LoginResponse{
    public string AccessToken {get; set; }

}