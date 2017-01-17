using System;
using System.Collections.Generic;

using AdaptableQuesting.Quests;

namespace AdaptableQuesting.Interfaces
{
	public interface IConversation
	{
		int currentConvPart { get; set; }
		List<ConversationPart> conversationParts { get; set; }
		ConversationPart CurrentPart { get; set; }

		void Reset();
		void AddConversationPart(string content, ConversationButton[] conversationButtons);
	}
}
