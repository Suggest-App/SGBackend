using System.Diagnostics.CodeAnalysis;
using SGBackend.Connector.Spotify;
using SGBackend.Entities;

namespace SGBackend.Provider;

public class AccessTokenProvider
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly Dictionary<Guid, AccessToken> _tokenCache = new();

    public AccessTokenProvider(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public bool TryGetTokenFromCache(Guid userId, [MaybeNullWhen(false)] out AccessToken accessToken)
    {
        var cachedTokenExists = _tokenCache.TryGetValue(userId, out accessToken);
        if (cachedTokenExists && DateTime.Now < accessToken.Fetched.Add(accessToken.ExpiresIn))
        {
            // token exists and is valid
            return true;
        }
        return false;
    }
    
    public void InsertTokenIntoCache(Guid userId, AccessToken accessToken)
    {
        _tokenCache[userId] = accessToken;
    }

    public async Task<string?> GetAccessToken(User user)
    {
        if (user.SpotifyRefreshToken == null) return null;
        if (_tokenCache.TryGetValue(user.Id, out var accessToken))
            // check if token is valid
            if (DateTime.Now < accessToken.Fetched.Add(accessToken.ExpiresIn))
                return accessToken.Token;
        // TODO: handle refresh token expired

        using (var scope = _scopeFactory.CreateScope())
        {
            var spotifyConnector = scope.ServiceProvider.GetRequiredService<SpotifyConnector>();
            // token is invalid / doesnt exist yet
            var tokenResponse = await spotifyConnector.GetAccessTokenUsingRefreshToken(user.SpotifyRefreshToken);

            if (tokenResponse == null) return null;

            _tokenCache[user.Id] = new AccessToken
            {
                Fetched = DateTime.Now,
                ExpiresIn = TimeSpan.FromSeconds(tokenResponse.expires_in),
                Token = tokenResponse.access_token
            };

            return tokenResponse.access_token;
        }
    }
}

public class AccessToken
{
    public string Token { get; set; }

    public TimeSpan ExpiresIn { get; set; }

    public DateTime Fetched { get; set; }
}