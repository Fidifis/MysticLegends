using System;
using System.Collections.Generic;

namespace MysticLegendsShared.Models;

public partial class Quest
{
    public int QuestId { get; set; }

    public string NpcName { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public bool IsRepeable { get; set; }

    public bool IsOffered { get; set; }

    public virtual ICollection<AcceptedQuest> AcceptedQuests { get; set; } = new List<AcceptedQuest>();

    public virtual Npc NpcNameNavigation { get; set; } = null!;

    public virtual QuestRequirement? QuestRequirement { get; set; }

    public virtual QuestReward? QuestReward { get; set; }
}
