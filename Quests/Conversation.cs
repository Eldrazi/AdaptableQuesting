using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AdaptableQuesting.Interfaces;


namespace AdaptableQuesting.Quests
{
	public class Conversation : IConversation, ICloneable
	{
		public int currentConvPart { get; set; }
		public List<ConversationPart> conversationParts { get; set; }

		public ConversationPart CurrentPart
		{
			get { return this.conversationParts[this.currentConvPart]; }
			set { }
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

		public object Clone()
		{
			return this.MemberwiseClone();
		}
	}
}
