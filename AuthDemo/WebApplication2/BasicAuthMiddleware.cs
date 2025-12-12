using System.Text;

namespace WebApplication2;

public class BasicAuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string _user;
    private readonly string _password;

    public BasicAuthMiddleware(RequestDelegate next, IConfiguration config)
    {
        _next = next;

        _user = config["Credentials:User"]
            ?? throw new Exception("Missing Credentials:User in configuration");

        _password = config["Credentials:Password"]
            ?? throw new Exception("Missing Credentials:Password in configuration");
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue("Authorization", out var authHeader))
        {
            context.Response.Headers["WWW-Authenticate"] = "Basic";
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        if (!authHeader.ToString().StartsWith("Basic "))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        var encodedCredentials = authHeader.ToString().Substring("Basic ".Length).Trim();
        var decodedBytes = Convert.FromBase64String(encodedCredentials);
        var decodedCredentials = Encoding.UTF8.GetString(decodedBytes);

        var parts = decodedCredentials.Split(':', 2);
        if (parts.Length != 2)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        var user = parts[0];
        var pwd = parts[1];

        if (user != _user || pwd != _password)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        await _next(context);
    }
}