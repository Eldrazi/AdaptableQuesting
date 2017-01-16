using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace AdaptableQuesting.Quests
{
	public class QuestPartElement
	{
		public int type { get; set; }
		public int amount { get; set; }
		public int currentAmount { get; set; }

		public string typesName { get; set; }
		public int[] types { get; set; }

		public QuestPartElement(int type, int amount, string typesName = "", params int[] types)
		{
			this.type = type;
			this.amount = amount;
			this.currentAmount = 0;

			this.typesName = typesName;
			this.types = types;
		}
	}
}
