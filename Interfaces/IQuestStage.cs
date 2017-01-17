using System;

using AdaptableQuesting.Quests;

namespace AdaptableQuesting.Interfaces
{
	public interface IQuestStage
	{
		void AddItemNeeded(int type, int amount);
		void AddKillNeeded(int netID, int amount, string typesName = "", params int[] types);
		void AddConversation(int npcType, Conversation conversation);
	}
}
