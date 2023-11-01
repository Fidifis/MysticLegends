using System;
using System.Collections.Generic;

namespace MysticLegendsShared.Models;

public partial class User
{
    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public virtual AccessToken? AccessToken { get; set; }

    public virtual ICollection<Character> Characters { get; set; } = new List<Character>();

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
