using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameInput;
using Terraria.ModLoader.IO;

using AdaptableQuesting.Quests;
using AdaptableQuesting.UI;

namespace AdaptableQuesting.Entities
{
    public class QuestPlayer : ModPlayer
    {
        public Quest[] currentQuests;
        public string completedQuests = "";

        private int updateQuestCooldown;

        public override void OnEnterWorld(Player player)
        {
            if (this.currentQuests == null)
                this.currentQuests = new Quest[AdaptableQuesting.MaxCurrentQuestsCount];
        }

        public override void PreUpdate()
        {
            if (updateQuestCooldown++ >= 300)
            {
                QuestManager.UpdateAvailableQuests();
                updateQuestCooldown = 0;
            }
        }

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            // If we press the hotkey, toggle the GUI on or off
            if (AdaptableQuesting.toggleQuestLogUIKey.JustPressed)
            {
                // Update the GUI before showing it
                if (!QuestLogUI.visible)
                {
                    ((AdaptableQuesting)AdaptableQuesting.instance).questLogGUI.Update(Main._drawInterfaceGameTime);
                    ((AdaptableQuesting)AdaptableQuesting.instance).questLogGUI.RecalculateQuests(this);
                }
                QuestLogUI.visible = !QuestLogUI.visible;
            }
        }

        public int AddQuest(Quest quest)
        {
            for (int i = 0; i < currentQuests.Length; ++i)
            {
                if(currentQuests[i] == null)
                {
                    currentQuests[i] = quest;
                    currentQuests[i].isActive = true;
                    return i;
                }
            }

            return -1;
        }
        public void RemoveQuest(Quest quest)
        {
            for (int i = 0; i < currentQuests.Length; ++i)
            {
                if (currentQuests[i] == quest)
                {
                    currentQuests[i] = null;
                    break;
                }
            }
        }

        public bool CompletedQuest(string id, string modName)
        {
            return completedQuests.Contains(id + ":" + modName);
        }

        public override TagCompound Save()
        {
            if (currentQuests == null) currentQuests = new Quest[AdaptableQuesting.MaxCurrentQuestsCount];

            TagCompound tc = new TagCompound();

            for (int i = 0; i < currentQuests.Length; ++i)
            {
                if (currentQuests[i] == null || string.IsNullOrEmpty(currentQuests[i].id))
                {
                    tc.Add("quest" + i, "empty");
                    continue;
                }
                string toWrite = currentQuests[i].id + "~" + currentQuests[i].modName + "~" + currentQuests[i].CurrentStage;
                foreach (QuestPartElement element in currentQuests[i].CurrentStageObject.killsNeeded)
                {
                    toWrite += "~" + element.type + ";" + element.currentAmount;
                }
                tc.Add("quest" + i, toWrite);
            }

            tc.Add("completedQuests", completedQuests);
            
            return tc;        
        }
        public override void Load(TagCompound tag)
        {
            this.currentQuests = new Quest[AdaptableQuesting.MaxCurrentQuestsCount];

            for (int i = 0; i < this.currentQuests.Length; ++i)
            {
                string loadedQuest = tag.GetString("quest" + i);

                if (loadedQuest == "empty")
                {
                    this.currentQuests[i] = null;
                    continue;
                }

                string[] data = loadedQuest.Split('~');
                this.currentQuests[i] = QuestManager.GetQuest(data[0], data[1]);

                // Failsave to check if the loaded quest is actually available.
                // If not, set the quest as an invalid quest to make sure it's not processed, but is still saved properly.
                if (this.currentQuests[i] == null)
                {
                    this.currentQuests[i] = new Quest(data[0], "", data[1], "");
                    this.currentQuests[i].invalid = true;
                }
                else
                {
                    this.currentQuests[i].isActive = true;
                }
                this.currentQuests[i].CurrentStage = int.Parse(data[2]);
                for (int n = 3; n < data.Length; ++n)
                {
                    string[] typeAndAmount = data[n].Split(';');
                    this.currentQuests[i].CurrentStageObject.killsNeeded.ElementAt(n - 3).currentAmount = int.Parse(typeAndAmount[1]);
                }
            }

            this.completedQuests = tag.GetString("completedQuests");
        }
    }
}
