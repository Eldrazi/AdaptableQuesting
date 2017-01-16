using System;
using System.Linq;
using System.Collections.Generic;

using Microsoft.Xna.Framework.Graphics;

using Terraria;
using Terraria.UI;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;

using AdaptableQuesting.UI;
using AdaptableQuesting.Quests;
using AdaptableQuesting.Entities;

namespace AdaptableQuesting
{
    public class AdaptableQuesting : Mod
    {
        public static Mod instance;

        public const int MaxCurrentQuestsCount = 20;

        public static NPCChatQuestUI chatQuestUI;

        internal QuestLogUI questLogGUI; // Quest Log Graphical UI
        internal static UserInterface questLogInterface;
        internal static ModHotKey toggleQuestLogUIKey;

        public AdaptableQuesting()
        {
	        Properties = new ModProperties()
	        {                
		        Autoload = true,
		        AutoloadGores = true,
		        AutoloadSounds = true
	        };
        }

        public override void Load()
        {
            instance = this;

            chatQuestUI = new NPCChatQuestUI();

            toggleQuestLogUIKey = RegisterHotKey("Toggle Quest Log GUI", "Q");

            if (!Main.dedServ)
            {
                // Setup UI if not running from a server
                questLogGUI = new QuestLogUI();
                questLogGUI.Activate();

                questLogInterface = new UserInterface();
                questLogInterface.SetState(questLogGUI);
            }
        }

        public override void PostSetupContent()
        {

        }

        public override void ModifyInterfaceLayers(List<MethodSequenceListItem> layers)
        {
            // Insert our new shop GUI layer so it updates in this layer
            int mouseTextLayerIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextLayerIndex != -1)
            {
                layers.Insert(mouseTextLayerIndex, new MethodSequenceListItem(
                        "Quest Log",
                        delegate
                        {
                            if (QuestLogUI.visible)
                            {
                                questLogInterface.Update(Main._drawInterfaceGameTime);
                                questLogGUI.Draw(Main.spriteBatch);
                            }
                            return true;
                        },
                        null)
                );
            }
        }

        public override void PostDrawInterface(SpriteBatch spriteBatch)
        {
            chatQuestUI.Draw(spriteBatch, this);
        }

        public override void ChatInput(string text, ref bool broadcast)
        {
            if (text.StartsWith("/"))
            {
                string command = text.Substring(1);

                // Clears all invalid quests in the players' currentQuest array.
                if (command.ToLower() == "clearinvalidquests")
                {
                    QuestPlayer qp = Main.player[Main.myPlayer].GetModPlayer<QuestPlayer>(this);
                    for (int i = 0; i < qp.currentQuests.Length; ++i)
                    {
                        if (qp.currentQuests[i].invalid)
                            qp.currentQuests[i] = null;
                    }
                }
                if (command.ToLower() == "clearactivequests")
                {
                    QuestPlayer qp = Main.player[Main.myPlayer].GetModPlayer<QuestPlayer>(this);

                    try
                    {
                        for (int i = 0; i < qp.currentQuests.Length; ++i)
                        {
                            if (qp.currentQuests[i] != null)
                            {
                                qp.currentQuests[i].isActive = false;
                                qp.currentQuests[i] = null;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Main.NewText(e.Message);
                    }
                }
                if (command.ToLower() == "clearcompletedquests")
                {
                    QuestPlayer qp = Main.player[Main.myPlayer].GetModPlayer<QuestPlayer>(this);
                    qp.completedQuests = "";
                }
            }
        }
    }

    public static class QuestManager
    {
        public static List<Quest> quests
        {
            get
            {
                if (_quests == null)
                    _quests = new List<Quest>();
                return _quests; 
            }
            set { _quests = value; }
        }
        public static List<Quest> _quests;

        public static void AddQuest(Quest quest)
        {
            for (int i = 0; i < quests.Count; ++i)
            {
                if (quests[i].id == quest.id && quests[i].modName == quest.modName)
                    return;
            }

            quests.Add(quest);
        }
        public static Quest GetQuest(string id, string modName)
        {
            for (int i = 0; i < quests.Count; ++i)
            {
                if (quests[i].id == id && quests[i].modName == modName)
                {
                    return quests[i].Copy();
                }
            }           
            return null;
        }

        public static List<Quest> GetAllAvailableQuestsForNPC(NPC npc)
        {
            // Create a temporary Quest list to return.
            List<Quest> q = new List<Quest>();

            // Get the local QuestPlayer
            QuestPlayer qp = Main.player[Main.myPlayer].GetModPlayer<QuestPlayer>(AdaptableQuesting.instance);
            // Loop through the local QuestPlayers' currentQuest array to check if there are any valid quests
            // for the given NPC. If so, add the quest to 'q'.
            for (int i = 0; i < qp.currentQuests.Length; ++i)
            {                
                Quest quest = qp.currentQuests[i];
                if (quest != null && !quest.invalid)
                {
                    if (quest.questParts[quest.currentQuestPart].conversations.ContainsKey(npc.type))
                    {
                        q.Add(quest);
                    }
                }
            }

            // After all the already active quests have been processed, 
            for (int i = 0; i < quests.Count; ++i)
            {
                Quest quest = quests[i];
                if (quest.introNPC == npc.type && ((!quest.repeatable && !quest.CompletedQuest()) || quest.repeatable) &&
                    (quest.requirement == null ? true : quest.requirement(qp.player)) && 
                    !q.Exists(x => (x.id == quest.id && x.modName == quest.modName)))
                {
                    q.Add(quest);
                }
            }

            return q;
        }

        /// <summary>
        /// Call this method when unloading a mod to remove all quests associated with this mod.
        /// </summary>
        /// <param name="modName"></param>
        public static void RemoveAllQuests(string modName)
        {
            for (int i = 0; i < quests.Count; ++i)
            {
                if (quests[i] != null && quests[i].modName == modName)
                {
                    quests.RemoveAt(i);
                    i--;
                }
            }
        }

        public static void UpdateAvailableQuests()
        {
            for (int n = 0; n < 200; ++n)
            {
                if (!Main.npc[n].active || !Main.npc[n].townNPC) continue;

                Main.npc[n].GetModInfo<QuestNPCInfo>(AdaptableQuesting.instance).questAvailable = QuestAvailability.None;

                QuestPlayer qp = Main.player[Main.myPlayer].GetModPlayer<QuestPlayer>(AdaptableQuesting.instance);
                // Loop through all the local players' active quests first.
                for (int i = 0; i < qp.currentQuests.Length; ++i)
                {
                    if (qp.currentQuests[i] == null) continue;

                    Quest quest = qp.currentQuests[i];
                    // If the given NPC is present in the currentQuestPart conversations list.
                    if (quest.questParts[quest.currentQuestPart].conversations.ContainsKey(Main.npc[n].type))
                    {
                        // If the given QuestPart is ready to be finished, we want to set it so and break out of the loop.
                        if (quest.CurrentQuestPartFinished())
                        {
                            Main.npc[n].GetModInfo<QuestNPCInfo>(AdaptableQuesting.instance).questAvailable = QuestAvailability.ActiveQuest_Complete;
                            break;
                        }
                        else // If not, we do want to set ActiveQuest_NotComplete, but we still want to keep looking to see if another QuestPart IS finished.
                        {
                            Main.npc[n].GetModInfo<QuestNPCInfo>(AdaptableQuesting.instance).questAvailable = QuestAvailability.ActiveQuest_NotComplete;
                        }
                    }
                }

                // If the town NPC does not have a quest which is ready to be completed, loop through the optional quests.
                if (Main.npc[n].GetModInfo<QuestNPCInfo>(AdaptableQuesting.instance).questAvailable != QuestAvailability.ActiveQuest_Complete)
                {
                    for (int i = 0; i < quests.Count; ++i)
                    {
                        if (quests[i] == null) continue;

                        Quest quest = quests[i];

                        if (((!quest.repeatable && !quest.CompletedQuest()) || quest.repeatable) &&
                            (quest.requirement == null ? true : quest.requirement(qp.player)) &&
                            quest.introNPC == Main.npc[n].type &&
                            !Array.Exists(qp.currentQuests, x => x != null && x.id == quest.id && x.modName == quest.modName))
                        {
                            Main.npc[n].GetModInfo<QuestNPCInfo>(AdaptableQuesting.instance).questAvailable = QuestAvailability.InactiveQuest;
                            break;
                        }
                    }
                }
            }
        }

        public static bool CompletedQuest(string id, string modName)
        {
            return Main.player[Main.myPlayer].GetModPlayer<QuestPlayer>(AdaptableQuesting.instance).CompletedQuest(id, modName);
        }
    }
}
