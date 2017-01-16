using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace AdaptableQuesting.Quests
{
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
}
