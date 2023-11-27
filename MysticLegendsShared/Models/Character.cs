using System;
using System.Collections.Generic;

namespace MysticLegendsShared.Models;

public partial class Character
{
    public string CharacterName { get; set; } = null!;

    public string CityName { get; set; } = null!;

    public string Username { get; set; } = null!;

    public int CharacterClass { get; set; }

    public int Level { get; set; }

    public int CurrencyGold { get; set; }

    public int Xp { get; set; }

    public virtual ICollection<AcceptedQuest> AcceptedQuests { get; set; } = new List<AcceptedQuest>();

    public virtual CharacterInventory? CharacterInventory { get; set; }

    public virtual ICollection<CityInventory> CityInventories { get; set; } = new List<CityInventory>();

    public virtual City CityNameNavigation { get; set; } = null!;

    public virtual ICollection<InventoryItem> InventoryItems { get; set; } = new List<InventoryItem>();

    public virtual Travel? Travel { get; set; }

    public virtual User UsernameNavigation { get; set; } = null!;
}
