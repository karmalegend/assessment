using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace ApI.Middleware;

public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
{
    private static readonly List<ApiClient> ApiClients = new()
    {
        new ApiClient
        {
            PartnerId = "partner-1",
            ApiKeys = new[] {"4ef18f62-c77a-447e-a3b6-c4bf2e5027d0"},
            UserId = "3fa85f64-5717-4562-b3fc-2c963f66afa6"
        }
    };

    public ApiKeyAuthenticationHandler(IOptionsMonitor<ApiKeyAuthenticationOptions> options, ILoggerFactory logger,
        UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var apiClient = GetApiClient();
        if (apiClient == null)
        {
            Logger.LogWarning("An API request was received without the x-api-key header");

            return Task.FromResult(AuthenticateResult.Fail("Invalid Api Client"));
        }

        Logger.BeginScope("{PartnerId}", apiClient.PartnerId);

        var claims = new[]
        {
            new Claim(CustomClaimNames.PartnerId, apiClient.PartnerId),
            new Claim(CustomClaimNames.UserId, apiClient.UserId)
        };
        var identity = new ClaimsIdentity(claims, ApiKeyAuthenticationOptions.DefaultScheme);
        var identities = new List<ClaimsIdentity> {identity};
        var principal = new ClaimsPrincipal(identities);
        var ticket = new AuthenticationTicket(principal, ApiKeyAuthenticationOptions.DefaultScheme);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }

    private ApiClient? GetApiClient()
    {
        if (Request.Headers.TryGetValue(ApiKeyAuthenticationOptions.HeaderName, out var apiKey) && apiKey.Count == 1)
            return ApiClients.Find(x => x.ApiKeys.Contains(apiKey[0]));

        return null;
    }

    private sealed class ApiClient
    {
        public string[] ApiKeys { get; set; }

        public string PartnerId { get; set; }
        public string UserId { get; set; }
    }
}
