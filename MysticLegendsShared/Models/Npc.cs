﻿using System;
using System.Collections.Generic;

namespace MysticLegendsShared.Models;

public partial class Npc
{
    public string NpcName { get; set; } = null!;

    public string CityName { get; set; } = null!;

    public int NpcType { get; set; }

    public int CurrencyGold { get; set; }

    public virtual City CityNameNavigation { get; set; } = null!;

    public virtual ICollection<NpcItem> NpcItems { get; set; } = new List<NpcItem>();

    public virtual ICollection<Quest> Quests { get; set; } = new List<Quest>();
}
