using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace WorkoutNest.API;

public interface IJwtToken
{
    string GenerateToken(string subject, Claim[] claims);
}

public class JwtTokenGenerator: IJwtToken
{
    private readonly string _issuer;
    private readonly string _audience;
    private readonly string _secretKey;
    private readonly int _expirationInMinutes;

    public JwtTokenGenerator(IConfiguration configuration)
    {
     
        _issuer = configuration["JwtToken:Issuer"];
        _audience = configuration["JwtToken:Audience"];
        _secretKey = configuration["JwtToken:SecretKey"];
        _expirationInMinutes = int.Parse(configuration["JwtToken:ExpirationInMinutes"]);
    }

    public string GenerateToken(string subject, Claim[] claims)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: new[] { new Claim(JwtRegisteredClaimNames.Sub, subject) }.Union(claims),
            expires: DateTime.UtcNow.AddMinutes(_expirationInMinutes),
            signingCredentials: credentials
        );

        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.WriteToken(token);
    }
}