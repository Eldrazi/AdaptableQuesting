using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AdaptableQuesting.Quests;


namespace AdaptableQuesting.Interfaces
{
	public interface IQuest
	{
		QuestStage[] Stages { get; set; }
		int CurrentStage { get; set; }
		//void NextStage();
		void CompleteQuest();
		//void CancelQuest();
		void StartQuest();
		void Reset();
	}
}
