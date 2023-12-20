using FastEndpoints;
using MongoDB.Driver;
using WorkoutNest.Infrastructure.Mongo;
using WorkoutNest.Infrastructure.Mongo.Entities;

namespace WorkoutNest.API.Auth;

internal class RegisterEndpoint : Endpoint<RegisterRequest, RegisterResponse>
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
        
        var client = new MongoClient(mongoDbConnectionString);
        var db = client.GetDatabase("workoutnest");
        var users = db.GetCollection<User>(Collections.UsersCollection);

        var userWithEmail = await (await users.FindAsync(x => x.Email == r.Email, cancellationToken: c))
            .SingleOrDefaultAsync(cancellationToken: c);

        if (userWithEmail != null)
        {
            throw new Exception($"Email '{userWithEmail.Email}' already exist in workout-nest, please provide different one");
        }
        
        var user = await (await users.FindAsync(x => x.Username == r.Username, cancellationToken: c))
            .SingleOrDefaultAsync(cancellationToken: c);
        
        if (user != null)
        {
            throw new Exception($"This username '{user.Username}' is taken, try with different one");
        }
        
        var id = Guid.NewGuid();
        await users.InsertOneAsync(new User()
        {
            Id = id.ToString(),
            Password = r.Password,
            Email = r.Email,
            Username = r.Username,
        }, cancellationToken: c);

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