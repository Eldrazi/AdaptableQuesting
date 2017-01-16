using System;
using System.Collections.Generic;

namespace AdaptableQuesting.Quests
{
    public class QuestNeedElement : List<QuestPartElement>
    {
        /// <summary>
        /// The list is arranged by type, for easier indexing.
        /// </summary>
        /// <param name="type">The type of the item/npc that is trying to be accessed.</param>
        /// <returns>The QuestPartElement associated with the given type.</returns>
        public QuestPartElement this[int type]
        {
            get
            {
                return (this.Find(x => x.type == type));
            }
            set
            {
                this[type] = value;
            }
        }
    }

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
