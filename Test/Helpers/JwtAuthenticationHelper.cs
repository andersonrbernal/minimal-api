using System.Text;
using System.Text.Json;
using MinimalApi.Domain.DTOs;
using MinimalApi.Domain.ModelViews;

namespace Test.Domain.Helpers;

public sealed class JwtAuthenticationHelper
{
    private const string MediaTypeJsonApplication = "application/json";

    public static async Task<string> GetJwtToken(
        string endpoint,
        LoginDTO loginDTO,
        string mediaType,
        JsonSerializerOptions options
    )
    {
        var stringfiedAdmin = JsonSerializer.Serialize(loginDTO);
        var content = new StringContent(stringfiedAdmin, Encoding.UTF8, mediaType);
        var request = await Setup.client.PostAsync(endpoint, content);
        var result = await request.Content.ReadAsStringAsync();

        var loggedAdmin = JsonSerializer.Deserialize<SignedInAdministratorModelView>(result, options);

        if (loggedAdmin == null) throw new Exception("Couldn't not authenticate administrator.");

        return loggedAdmin.Token;
    }
}