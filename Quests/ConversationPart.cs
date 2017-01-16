using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace AdaptableQuesting.Quests
{
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
}
