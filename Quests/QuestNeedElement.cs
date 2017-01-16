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
}
