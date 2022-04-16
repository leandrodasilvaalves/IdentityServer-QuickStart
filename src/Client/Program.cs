using IdentityModel.Client;
using System.Text.Json;

var handler = new HttpClientHandler
{
     //only dev environment
    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
};

var client = new HttpClient(handler);
var disco = await client.GetDiscoveryDocumentAsync("https://localhost:5001");
if (disco.IsError)
{
    Console.WriteLine(disco.Error);
    return;
}

var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
{
    Address = disco.TokenEndpoint,
    ClientId = "client",
    ClientSecret = "secret",
    Scope = "api1",
});

if (tokenResponse.IsError)
{
    Console.WriteLine(tokenResponse.Error);
    return;
}
Console.WriteLine(tokenResponse.Json);

var apiClient = new HttpClient(handler);
apiClient.SetBearerToken(tokenResponse.AccessToken);
var response = await apiClient.GetAsync("https://localhost:6001/identity");

if (!response.IsSuccessStatusCode)
{
    Console.WriteLine(response.StatusCode);
}
else
{
    var content = await response.Content.ReadAsStringAsync();
    using var json = JsonDocument.Parse(content);
    var result = JsonSerializer.Serialize(json, new JsonSerializerOptions { WriteIndented = true });
    Console.WriteLine(result);
}