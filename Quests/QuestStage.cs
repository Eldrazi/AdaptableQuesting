using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AdaptableQuesting.Interfaces;


namespace AdaptableQuesting.Quests
{
	public class QuestStage : IQuestStage
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

		// Provide a base implementation of the IQuestStage here
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

		public object Clone()
		{
			return this.MemberwiseClone();
		}
	}
}
