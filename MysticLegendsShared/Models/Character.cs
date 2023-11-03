﻿using System;
using System.Collections.Generic;

namespace MysticLegendsShared.Models;

public partial class Character
{
    public string CharacterName { get; set; } = null!;

    public string Username { get; set; } = null!;

    public int CharacterClass { get; set; }

    public int Level { get; set; }

    public int CurrencyGold { get; set; }

    public virtual ICollection<AcceptedQuest> AcceptedQuests { get; set; } = new List<AcceptedQuest>();

    public virtual CharacterInventory? CharacterInventory { get; set; }

    public virtual ICollection<CityInventory> CityInventories { get; set; } = new List<CityInventory>();

    public virtual ICollection<InventoryItem> InventoryItems { get; set; } = new List<InventoryItem>();

    public virtual User UsernameNavigation { get; set; } = null!;
}