using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
