using System;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AdaptableQuesting.Entities
{
    public enum QuestAvailability
    {
        None = 0,
        ActiveQuest_NotComplete = 1,
        ActiveQuest_Complete = 2,
        InactiveQuest = 3
    }

    public class QuestNPCInfo : NPCInfo
    {
        public QuestAvailability questAvailable;
    }
}
