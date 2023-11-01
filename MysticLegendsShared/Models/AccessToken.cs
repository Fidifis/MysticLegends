using System;
using System.Collections.Generic;

namespace MysticLegendsShared.Models;

public partial class AccessToken
{
    public string Username { get; set; } = null!;

    public string AccessToken1 { get; set; } = null!;

    public DateTime Expiration { get; set; }

    public virtual User UsernameNavigation { get; set; } = null!;
}
