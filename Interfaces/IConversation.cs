using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
