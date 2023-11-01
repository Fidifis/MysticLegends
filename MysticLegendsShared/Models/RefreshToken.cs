using System;
using System.Collections.Generic;

namespace MysticLegendsShared.Models;

public partial class RefreshToken
{
    public int RecordId { get; set; }

    public string Username { get; set; } = null!;

    public string RefreshToken1 { get; set; } = null!;

    public DateTime Expiration { get; set; }

    public virtual User UsernameNavigation { get; set; } = null!;
}
