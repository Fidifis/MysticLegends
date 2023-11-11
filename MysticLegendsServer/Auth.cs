﻿using Microsoft.EntityFrameworkCore;
using MysticLegendsServer.Models;
using MysticLegendsShared.Models;
using System.Security.Cryptography;
using System.Text;

namespace MysticLegendsServer;

public class Auth: IDisposable
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

        var character = await dbContext.Characters
            .Where(character => character.CharacterName == characterName)
            .Include(character => character.UsernameNavigation)
            .ThenInclude(user => user.AccessToken)
            .SingleAsync();

        var dbAccessToken = character.UsernameNavigation.AccessToken;
        if (dbAccessToken is null || dbAccessToken.Expiration < DateTime.Now)
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

        var user = await dbContext.Users
            .Where(user => user.Username == username)
            .Include(user => user.AccessToken)
            .SingleAsync();

        var dbAccessToken = user.AccessToken;
        if (dbAccessToken is null || dbAccessToken.Expiration < DateTime.Now)
            return false;

        var trueToken = dbAccessToken.AccessToken1;

        return trueToken == accessToken;
    }

    public async Task<string?> IssueRefreshToken(string username, string password)
    {
        ObjectDisposedException.ThrowIf(disposed, this);

        var userTask = dbContext.Users.SingleAsync(user => user.Username == username);
        var passwordHash = GetPasswordHash(password);
        var user = await userTask;

        if (user.PasswordHash != passwordHash)
            return null;

        var refreshToken = GenerateToken(random, RefreshTokenLength);
        dbContext.Add(new RefreshToken()
        {
            Username = username,
            RefreshToken1 = refreshToken,
            Expiration = DateTime.Now.AddDays(RefreshTokenExpirationDays)
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
        if (dbRefreshToken is null || dbRefreshToken.Expiration < DateTime.Now)
            return null;

        var accessToken = GenerateToken(random, AccessTokenLength);

        dbRefreshToken.UsernameNavigation.AccessToken = new AccessToken()
        {
            Username = dbRefreshToken.Username,
            AccessToken1 = accessToken,
            Expiration = DateTime.Now.AddMinutes(AccessTokenExpirationMinutes)
        };
        await dbContext.SaveChangesAsync();

        return accessToken;
    }

    public static string GenerateToken(RandomNumberGenerator randomInstance, int length)
    {
        var bytes = new byte[length];
        randomInstance.GetBytes(bytes);
        var chars = bytes.Select(b => alphaNumericChars[b % alphaNumericChars.Length]).ToArray();
        return new string(chars);
    }

    public string GetPasswordHash(string plaintext) => Convert.ToHexString(md5.ComputeHash(Encoding.UTF8.GetBytes(plaintext))).ToLower();

    public void Dispose()
    {
        if (disposed)
            return;

        random.Dispose();
        md5.Dispose();

        disposed = true;
    }
}
