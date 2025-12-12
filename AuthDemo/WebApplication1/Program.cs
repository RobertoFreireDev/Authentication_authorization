using System.Net.Http.Headers;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient("ExternalApi", client =>
{
    var externalBaseUrl = builder.Configuration["ExternalApi:BaseUrl"]
        ?? throw new Exception("Missing ExternalApi:BaseUrl");
    client.BaseAddress = new Uri(externalBaseUrl);
})
.ConfigureHttpClient((sp, client) =>
{
    var externalUser = builder.Configuration["ExternalApi:User"]
        ?? throw new Exception("Missing ExternalApi:User");
    var externalPassword = builder.Configuration["ExternalApi:Password"]
        ?? throw new Exception("Missing ExternalApi:Password");
    var credentials = $"{externalUser}:{externalPassword}";
    var base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(credentials));

    client.DefaultRequestHeaders.Authorization =
        new AuthenticationHeaderValue("Basic", base64);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/weatherforecast", async (IHttpClientFactory httpClientFactory) =>
{
    var client = httpClientFactory.CreateClient("ExternalApi");

    var response = await client.GetAsync("weatherforecast");
    response.EnsureSuccessStatusCode();

    return await response.Content.ReadAsStringAsync();
})
.WithName("GetWeatherForecast");

app.Run();

