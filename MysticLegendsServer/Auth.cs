using Microsoft.EntityFrameworkCore;
using MysticLegendsServer.Models;
using MysticLegendsShared.Models;
using MysticLegendsShared.Utilities;
using System.Security.Cryptography;
using System.Text;

namespace MysticLegendsServer;

public sealed class Auth: IDisposable
{
    private static readonly char[] alphaNumericChars = Enumerable.Range('0', '9' - '0').Union(Enumerable.Range('A', 'Z' - 'A')).Union(Enumerable.Range('a', 'z' - 'a')).Select(i => (char)i).ToArray();

    public int RefreshTokenExpirationDays { get; set; } = 90;
    public int AccessTokenExpirationMinutes { get; set; } = 60;
    public int RefreshTokenLength { get; set; } = 256;
    public int AccessTokenLength { get; set; } = 256;

    private bool disposed = false;

    private readonly Xdigf001Context dbContext;
    private readonly MD5 md5 = MD5.Create();
    private readonly RandomNumberGenerator random = RandomNumberGenerator.Create();

    public Auth(Xdigf001Context dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<bool> ValidateAsync(IHeaderDictionary headers, string characterName)
    {
        if (!headers.ContainsKey("access-token"))
            return false;

        var accessToken = headers["access-token"].FirstOrDefault();

        if (accessToken is null)
            return false;

        return await ValidateAsync(accessToken, characterName);
    }

    public async Task<bool> ValidateAsync(string accessToken, string characterName)
    {
        ObjectDisposedException.ThrowIf(disposed, this);

        var dbAccessToken = await dbContext.Characters
            .Where(character => character.CharacterName == characterName)
            .Include(character => character.UsernameNavigation)
            .ThenInclude(user => user.AccessToken)
            .Select(character => character.UsernameNavigation.AccessToken)
            .SingleAsync();

        //var dbAccessToken = character.UsernameNavigation.AccessToken;
        if (dbAccessToken is null || dbAccessToken.Expiration < Time.Current)
            return false;

        var trueToken = dbAccessToken.AccessToken1;

        return trueToken == accessToken;
    }

    public async Task<bool> ValidateUserAsync(IHeaderDictionary headers, string username)
    {
        if (!headers.ContainsKey("access-token"))
            return false;

        var accessToken = headers["access-token"].FirstOrDefault();

        if (accessToken is null)
            return false;

        return await ValidateUserAsync(accessToken, username);
    }

    public async Task<bool> ValidateUserAsync(string accessToken, string username)
    {
        ObjectDisposedException.ThrowIf(disposed, this);

        var dbAccessToken = await dbContext.Users
            .Where(user => user.Username == username)
            .Include(user => user.AccessToken)
            .Select(user => user.AccessToken)
            .SingleAsync();

        //var dbAccessToken = user.AccessToken;
        if (dbAccessToken is null || dbAccessToken.Expiration < Time.Current)
            return false;

        var trueToken = dbAccessToken.AccessToken1;

        return trueToken == accessToken;
    }

    public async Task<User?> RegisterUser(string username, string password)
    {
        ObjectDisposedException.ThrowIf(disposed, this);

        // TODO: check minumal password length >=7
        username = username.Trim();
        if (username == "" || password == "")
            return null;

        foreach (var letter in username)
        {
            if (!alphaNumericChars.Contains(letter))
                return null;
        }

        var user = new User()
        {
            Username = username,
            PasswordHash = GetPasswordHash(password),
        };

        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();
        return user;
    }

    public async Task<string?> IssueRefreshToken(string username, string password)
    {
        var user = await dbContext.Users.SingleAsync(user => user.Username == username);
        return await IssueRefreshToken(username, password, user);
    }

    public async Task<string?> IssueRefreshToken(string username, string password, User user)
    {
        ObjectDisposedException.ThrowIf(disposed, this);

        var passwordHash = GetPasswordHash(password);

        if (user.PasswordHash != passwordHash)
            return null;

        var refreshToken = GenerateToken(random, RefreshTokenLength);
        dbContext.Add(new RefreshToken()
        {
            Username = username,
            RefreshToken1 = refreshToken,
            Expiration = Time.Current.AddDays(RefreshTokenExpirationDays)
        });
        await dbContext.SaveChangesAsync();

        return refreshToken;
    }

    public async Task<string?> IssueAccessToken(string refreshToken)
    {
        ObjectDisposedException.ThrowIf(disposed, this);

        var dbRefreshToken = await dbContext.RefreshTokens
            .Include(token => token.UsernameNavigation)
            .ThenInclude(user => user.AccessToken)
            .SingleOrDefaultAsync(token => token.RefreshToken1 == refreshToken);

        // TODO: cleanup expired tokens
        if (dbRefreshToken is null || dbRefreshToken.Expiration < Time.Current)
            return null;

        var accessToken = GenerateToken(random, AccessTokenLength);

        dbRefreshToken.UsernameNavigation.AccessToken = new AccessToken()
        {
            Username = dbRefreshToken.Username,
            AccessToken1 = accessToken,
            Expiration = Time.Current.AddMinutes(AccessTokenExpirationMinutes)
        };
        await dbContext.SaveChangesAsync();

        return accessToken;
    }

    public async Task<bool> InvalidateRefreshToken(string refreshToken)
    {
        ObjectDisposedException.ThrowIf(disposed, this);

        var tokenRecord = await dbContext.RefreshTokens.SingleOrDefaultAsync(token => token.RefreshToken1 == refreshToken);

        if (tokenRecord is null)
        {
            return false;
        }

        dbContext.RefreshTokens.Remove(tokenRecord);
        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> InvalidateAccessToken(string accessToken)
    {
        ObjectDisposedException.ThrowIf(disposed, this);

        var tokenRecord = await dbContext.AccessTokens.SingleOrDefaultAsync(token => token.AccessToken1 == accessToken);

        if (tokenRecord is null)
        {
            return false;
        }

        dbContext.AccessTokens.Remove(tokenRecord);
        await dbContext.SaveChangesAsync();
        return true;
    }

    public static string GenerateToken(RandomNumberGenerator randomInstance, int length)
    {
        var bytes = new byte[length];
        randomInstance.GetBytes(bytes);
        var chars = bytes.Select(b => alphaNumericChars[b % alphaNumericChars.Length]).ToArray();
        return new string(chars);
    }

    private string GetPasswordHash(string plaintext) => Convert.ToHexString(md5.ComputeHash(Encoding.UTF8.GetBytes(plaintext))).ToLower();

    public void Dispose()
    {
        if (disposed)
            return;

        random.Dispose();
        md5.Dispose();

        disposed = true;
    }
}
