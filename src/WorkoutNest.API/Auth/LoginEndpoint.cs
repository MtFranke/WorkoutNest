using FastEndpoints;

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

        await SendAsync(new() { }, cancellation: c);
    }
}


public class LoginRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
}

public class LoginResponse{
    
}