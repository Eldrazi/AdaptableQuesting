using System;
using System.Linq;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using AdaptableQuesting.Entities;

namespace AdaptableQuesting.Quests
{
    public delegate bool QuestDelegate(Player p);

    public class Quest : IQuest
    {
        /// <summary>
        /// Only true when a quest is invalid. Happens when a mod is still active in the players' quest list,
        /// but the mod belonging to the quest is unloaded.
        /// </summary>
        public bool invalid = false;
        public bool isActive;

        public string id;
        public string name;
        public string modName;
        public string description;

        public bool repeatable;

        public QuestStage[] Stages { get; set; }
        public int CurrentStage { get; set; }

        /// <summary>
        /// A delegate that is called when the quest has been completed.
        /// Using a delegate, the creator will also be able to make the quests more customized 
        /// (give exp if a level system is implemented, etc).
        /// </summary>
        public QuestDelegate questCompleted;
        public QuestDelegate requirement;

        public int introNPC;
        public Conversation introConversation;

        public QuestStage CurrentStageObject
        {
            get { return this.Stages[this.CurrentStage]; }
        }

        public Quest(string id, string name, string modName, string description)
        {
            this.invalid = false;
            this.isActive = false;

            this.id = id;
            this.name = name;
            this.modName = modName;
            this.description = description;

            this.repeatable = false;

            this.Stages = new QuestStage[0];

            this.questCompleted = null;
        }
        public Quest Copy()
        {
            return this.MemberwiseClone() as Quest;
        }

        public void StartQuest()
        {

        }

        public void AddQuest()
        {
            QuestManager.AddQuest(this);
        }
        public int AddStage(QuestStage questPart)
        {
            int newPartIndex = Stages.Length;
            QuestStage[] tempParts = new QuestStage[newPartIndex + 1];

            for (int i = 0; i < Stages.Length; ++i)
            {
                tempParts[i] = Stages[i];
            }

            tempParts[newPartIndex] = questPart;
            this.Stages = tempParts;

            return newPartIndex;
        }

        public void CompleteQuest()
        {
            // Call the 'questCompleted' delegate.
            if(questCompleted != null)
                questCompleted(Main.player[Main.myPlayer]);

            QuestPlayer qp = Main.player[Main.myPlayer].GetModPlayer<QuestPlayer>(AdaptableQuesting.instance);
            if(!CompletedQuest())
            {
                if (!string.IsNullOrEmpty(qp.completedQuests))
                    qp.completedQuests += ";";
                qp.completedQuests += this.id + ":" + this.modName;
            }
            this.Reset();
            qp.RemoveQuest(this);
        }

        public bool CompletedQuest()
        {
            QuestPlayer qp = Main.player[Main.myPlayer].GetModPlayer<QuestPlayer>(AdaptableQuesting.instance);
            return qp.completedQuests.Contains(this.id + ":" + this.modName);
        }

        public bool CurrentQuestPartFinished()
        {
            foreach (QuestPartElement element in this.CurrentStageObject.killsNeeded)
            {
                if (element.currentAmount != element.amount)
                {
                    return false;
                }
            }
            foreach (QuestPartElement element in this.CurrentStageObject.itemsNeeded)
            {
                element.currentAmount = 0;

                for (int i = 0; i < 58; i++)
                {
                    if(element.type == Main.player[Main.myPlayer].inventory[i].type)
                    {
                        element.currentAmount += Main.player[Main.myPlayer].inventory[i].stack;
                    }
                }

                if(element.currentAmount < element.amount)
                    return false;
            }

            if (this.CurrentStageObject.partUnfinishedCheck != null)
                return this.CurrentStageObject.partUnfinishedCheck(Main.player[Main.myPlayer]);

            return true;
        }

        public void Reset()
        {
            this.CurrentStage = 0;
            this.isActive = false;

            for (int i = 0; i < Stages.Length; ++i)
            {
                foreach (QuestPartElement element in Stages[i].killsNeeded)
                {
                    element.currentAmount = 0;
                }
            }
        }
    }

    public class QuestStage
    {
        public string description;

        public Dictionary<int, Conversation> conversations; // List of all conversations, keys are NPC types.
        
        /// <summary>
        /// Holds all the items needed to complete the quest part.
        /// [0] is the ID of the item, [1] is the amount needed.
        /// </summary>
        public QuestNeedElement itemsNeeded;
        /// <summary>
        /// Holds all the kills that are needed to complete the quest part.
        /// [0] is the ID of the NPCs, [1] is the amount of kills needed.
        /// </summary>
        public QuestNeedElement killsNeeded;

        public string partUnfinishedText;
        public QuestDelegate partUnfinishedCheck;

        public QuestStage(string description)
        {
            this.description = description;

            conversations = new Dictionary<int, Conversation>();

            itemsNeeded = new QuestNeedElement();
            killsNeeded = new QuestNeedElement();
        }

        public void AddItemNeeded(int type, int amount)
        {
            itemsNeeded.Add(new QuestPartElement(type, amount));
        }
        public void AddKillNeeded(int netID, int amount, string typesName = "", params int[] types)
        {
            killsNeeded.Add(new QuestPartElement(netID, amount, typesName, types));
        }

        public void AddConversation(int npcType, Conversation conversation)
        {
            conversations.Add(npcType, conversation);
        }
    }
    public class Conversation
    {
        public int currentConvPart = 0;
        
        public List<ConversationPart> conversationParts;

        public ConversationPart CurrentPart
        {
            get { return this.conversationParts[this.currentConvPart]; }
        }

        public Conversation()
        {
            conversationParts = new List<ConversationPart>();
        }

        public void Reset()
        {
            currentConvPart = 0;
        }

        public void AddConversationPart(string content, ConversationButton[] conversationButtons)
        {
            ConversationPart convPart = new ConversationPart(content, conversationButtons);
            conversationParts.Add(convPart);
        }
    }
    public class ConversationPart
    {
        public string text;

        public ConversationButton[] conversationButtons;

        public ConversationPart(string text, ConversationButton[] conversationButtons)
        {
            this.text = text;
            this.conversationButtons = conversationButtons;
        }
    }
    public class ConversationButton
    {
        public string displayName;
        public string actions;

        public ConversationButton(string displayName, string actions)
        {
            this.displayName = displayName;
            this.actions = actions;
        }
    }

    public interface IQuest
    {        
        QuestStage[] Stages { get; set; }
        int CurrentStage { get; set; }
        //void NextStage();
        void CompleteQuest();
        //void CancelQuest();
        void StartQuest();
        void Reset();
    }
}
