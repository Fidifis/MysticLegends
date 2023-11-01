using System;
using System.Collections.Generic;

namespace MysticLegendsShared.Models;

public partial class AcceptedQuest
{
    public string CharacterName { get; set; } = null!;

    public int QuestId { get; set; }

    public int QuestState { get; set; }

    public virtual Character CharacterNameNavigation { get; set; } = null!;

    public virtual Quest Quest { get; set; } = null!;
}
