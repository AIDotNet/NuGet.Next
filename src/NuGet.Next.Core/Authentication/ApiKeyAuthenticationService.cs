using NuGet.Next.Options;

namespace NuGet.Next.Core;

public class ApiKeyAuthenticationService : IAuthenticationService
{
    private readonly string _apiKey;

    public ApiKeyAuthenticationService(NuGetNextOptions options)
    {
        if (options == null) throw new ArgumentNullException(nameof(options));

        // _apiKey = string.IsNullOrEmpty(options.ApiKey) ? null : options.ApiKey;
    }

    public Task<bool> AuthenticateAsync(string apiKey, CancellationToken cancellationToken)
    {
        return Task.FromResult(Authenticate(apiKey));
    }

    private bool Authenticate(string apiKey)
    {
        // No authentication is necessary if there is no required API key.
        if (_apiKey == null) return true;

        return _apiKey == apiKey;
    }
}