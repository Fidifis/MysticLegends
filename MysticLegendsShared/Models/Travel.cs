using System;
using System.Collections.Generic;

namespace MysticLegendsShared.Models;

public partial class Travel
{
    public string CharacterName { get; set; } = null!;

    public string? AreaName { get; set; }

    public DateTime Arrival { get; set; }

    public virtual Area? AreaNameNavigation { get; set; }

    public virtual Character CharacterNameNavigation { get; set; } = null!;
}
