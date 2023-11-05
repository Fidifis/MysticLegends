using System;
using System.Collections.Generic;

namespace MysticLegendsShared.Models;

public partial class City
{
    public string CityName { get; set; } = null!;

    public virtual ICollection<Character> Characters { get; set; } = new List<Character>();

    public virtual ICollection<CityInventory> CityInventories { get; set; } = new List<CityInventory>();

    public virtual ICollection<Npc> Npcs { get; set; } = new List<Npc>();
}
