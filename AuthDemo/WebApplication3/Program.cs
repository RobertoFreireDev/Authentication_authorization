using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

var key = new SymmetricSecurityKey(
        Encoding.UTF8.GetBytes(config["Jwt:Key"]!)
    );

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = key,
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

app.Use(async (context, next) => {
    var auth = context.Request.Headers["Authorization"].ToString();
    Console.WriteLine($"RAW Authorization header: [{auth}]");
    await next();
});

app.UseAuthentication();
app.UseAuthorization();

var usersOnDatabase = new Dictionary<string,string>()
{
    { "testuser", "testpassword" }, // for simplicity, no hash password
    { "testuser2", "testpassword2" }
};

string GenerateJwt(string username, IConfiguration config)
{
    var tokenHandler = new JwtSecurityTokenHandler();
    var tokenDescriptor = new SecurityTokenDescriptor
    {
        Subject = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.Name, username),
        ]),
        Expires = DateTime.UtcNow.AddHours(2),
        SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
    };
    var token = tokenHandler.CreateToken(tokenDescriptor);

    return tokenHandler.WriteToken(token);
}

app.MapPost("/login", (LoginRequest login, IConfiguration config) =>
{
    if (string.IsNullOrWhiteSpace(login.Username) || string.IsNullOrWhiteSpace(login.Password))
    {
        return Results.Unauthorized();
    }

    if (!usersOnDatabase.TryGetValue(login.Username, out var storedPassword) ||
    !string.Equals(storedPassword, login.Password, StringComparison.Ordinal))
    {
        return Results.Unauthorized();
    }

    var jwt = GenerateJwt(login.Username, config);

    return Results.Ok(new { token = jwt });
});

app.MapGet("/secure", (ClaimsPrincipal user) =>
{
    return Results.Ok(new { message = $"Hello {user.Identity?.Name}, you are authenticated!" });
}).RequireAuthorization();

app.Run();

public record LoginRequest(string Username, string Password);
