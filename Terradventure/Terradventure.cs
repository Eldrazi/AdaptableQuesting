using System;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using AdaptableQuesting;
using AdaptableQuesting.Quests;
using AdaptableQuesting.Entities;

namespace Terradventure
{
    public class Terradventure : Mod
    {
        public int[] nightspawn = new int[] { NPCID.Zombie, NPCID.BaldZombie, NPCID.BigBaldZombie, NPCID.BigFemaleZombie, NPCID.BigPincushionZombie,
            NPCID.BigSlimedZombie, NPCID.BigTwiggyZombie, NPCID.BigZombie, NPCID.FemaleZombie, NPCID.PincushionZombie, NPCID.SlimedZombie,
            NPCID.SmallBaldZombie, NPCID.SmallFemaleZombie, NPCID.SmallPincushionZombie, NPCID.SmallSlimedZombie, NPCID.SmallTwiggyZombie, NPCID.SmallZombie, 
            NPCID.TwiggyZombie, NPCID.ArmedZombie, NPCID.SwampZombie, NPCID.SmallSwampZombie, NPCID.BigSwampZombie, NPCID.DemonEye, NPCID.DemonEye2,
            NPCID.CataractEye, NPCID.CataractEye2, NPCID.SleepyEye, NPCID.SleepyEye2, NPCID.DialatedEye, NPCID.DialatedEye2, NPCID.GreenEye, NPCID.GreenEye2,
            NPCID.PurpleEye, NPCID.PurpleEye2 };

        public Terradventure()
        {
	        Properties = new ModProperties()
	        {                
		        Autoload = true,
		        AutoloadGores = true,
		        AutoloadSounds = true
	        };
        }

        public override void Load()
        {
            QuestManager.RemoveAllQuests(this.Name);

            #region Guide Quests

            // Questline

            Quest quest = new Quest("Main1", "An Introduction", this.Name, "Help the Guide");

            Conversation conv = new Conversation();
            conv.conversationParts.Add(new ConversationPart("Why hello there! It's been a while since I've seen a friendly face around here.", new ConversationButton[] { new ConversationButton("What?", "convup"), new ConversationButton("Uh...?", "convup") }));
            conv.conversationParts.Add(new ConversationPart("Little fuzzy, are we? Well, that will probably pass in about a minute or so.", new ConversationButton[] { new ConversationButton("Where am I?", "convup") }));
            conv.conversationParts.Add(new ConversationPart("If only I knew my friend, if only I knew. Best thing we can do is make the most of it, don't you agree?", new ConversationButton[] { new ConversationButton("I guess", "convup") }));
            conv.conversationParts.Add(new ConversationPart("Great! Then you wouldn't mind getting some work done... Would you? How about you start by making a Workbench from the wood of some of the trees around here?", new ConversationButton[] { new ConversationButton("Accept", "addquest;closeconv") }));
            quest.introConversation = conv;
            quest.introNPC = NPCID.Guide;

            int partIndex = quest.AddPart(new QuestPart("Craft a Workbench"));
            conv = new Conversation();
            conv.conversationParts.Add(new ConversationPart("Wow, you did it! That's great, now we can get some stuff done!", new ConversationButton[] { new ConversationButton("What now?", "convup") }));
            conv.conversationParts.Add(new ConversationPart("Hmm, I guess something to fend those pesky slimes off. How about a Wooden Sword for starters?", new ConversationButton[] { new ConversationButton("Accept", "questpartup;closeconv") }));
            quest.questParts[partIndex].AddConversation(NPCID.Guide, conv);
            quest.questParts[partIndex].partUnfinishedText = "Don't take too long crafting that Workbench now. Trust me, you want to be done before nightfall!";
            quest.questParts[partIndex].AddItemNeeded(ItemID.WorkBench, 1);

            partIndex = quest.AddPart(new QuestPart("Craft a Wooden Sword"));
            conv = new Conversation();
            conv.conversationParts.Add(new ConversationPart("Now you got yourself a sword! You could try testing it out...", new ConversationButton[] { new ConversationButton("'Testing it out'?", "convup") }));
            conv.conversationParts.Add(new ConversationPart("Yes, beat up some of those Blue Slimes! I think 5 will do.", new ConversationButton[] { new ConversationButton("Accept", "questpartup;closeconv") }));
            quest.questParts[partIndex].AddConversation(NPCID.Guide, conv);
            quest.questParts[partIndex].partUnfinishedText = "Show me your craftsmanship! I'm eager to see your sword.";
            quest.questParts[partIndex].AddItemNeeded(ItemID.WoodenSword, 1);

            partIndex = quest.AddPart(new QuestPart("Kill 5 Blue Slimes"));
            conv = new Conversation();
            conv.conversationParts.Add(new ConversationPart("That was gruesome, nice job. Now it seems night is almost here. How about using some of that Gel the Slimes dropped to craft some Torches?", new ConversationButton[] { new ConversationButton("Accept", "questpartup;closeconv") }));
            quest.questParts[partIndex].AddConversation(NPCID.Guide, conv);
            quest.questParts[partIndex].partUnfinishedText = "Have you finished killing those slimes yet?";
            quest.questParts[partIndex].AddKillNeeded(NPCID.BlueSlime, 5);

            partIndex = quest.AddPart(new QuestPart("Craft 20 Torches"));
            conv = new Conversation();
            conv.conversationParts.Add(new ConversationPart("Allright, now we can at least light the place up! Thanks for your help so far. Here, something for your trouble.", new ConversationButton[] { new ConversationButton("Turn in", "completequest;closeconv") }));
            quest.questParts[partIndex].AddConversation(NPCID.Guide, conv);
            quest.questParts[partIndex].partUnfinishedText = "I don't mean to hurry you, but we do need some light anytime soon.";
            quest.questParts[partIndex].AddItemNeeded(ItemID.Torch, 20);

            quest.questCompleted = delegate(Player p)
            {
                p.QuickSpawnItem(ItemID.Spear);
                p.QuickSpawnItem(ItemID.LesserHealingPotion, 5);
                return true;
            };

            quest.AddQuest();


            quest = new Quest("Main2", "Nightspawn", this.Name, "Fend of the Nightspawn");

            quest.requirement = delegate(Player p)
            {
                return !Main.dayTime && QuestManager.CompletedQuest("Main1", this.Name);
            };
            conv = new Conversation();
            conv.conversationParts.Add(new ConversationPart("By Cthulu, what are these monsters roaming around? I've never seen anything like them!", new ConversationButton[] { new ConversationButton("What to do?", "convup") }));
            conv.conversationParts.Add(new ConversationPart("What to do?! Kill them, that's what to do!", new ConversationButton[] { new ConversationButton("Accept", "addquest;closeconv") }));
            quest.introConversation = conv;
            quest.introNPC = NPCID.Guide;

            partIndex = quest.AddPart(new QuestPart("Kill the Nightspawn"));
            conv = new Conversation();
            conv.conversationParts.Add(new ConversationPart("Whew, that should make things settle down a little bit... Thank you, I don't know what I'd have done without you.", new ConversationButton[] { new ConversationButton("Turn in", "completequest;closeconv") }));
            quest.questParts[partIndex].AddConversation(NPCID.Guide, conv);
            quest.questParts[partIndex].partUnfinishedText = "Kill them! Don't let any one of them live!";
            quest.questParts[partIndex].AddKillNeeded(0, 20, "Nightspawn", nightspawn);

            quest.questCompleted = delegate(Player p)
            {
                p.QuickSpawnItem(ItemID.GrapplingHook);
                return true;
            };

            quest.AddQuest();


            quest = new Quest("Main3", "The Merchant", this.Name, "Find out who the Merchant is");

            quest.requirement = delegate(Player p)
            {
                return NPC.AnyNPCs(NPCID.Merchant) && QuestManager.CompletedQuest("Main2", this.Name);
            };

            conv = new Conversation();
            conv.conversationParts.Add(new ConversationPart("Pssst, hey... Have you seen that new guy hanging around here?", new ConversationButton[] { new ConversationButton("Yes", "convup") }));
            conv.conversationParts.Add(new ConversationPart("Hmm, so I'm not imagining this. I've heard him talking a bit, calling himself 'The Merchant'. I don't trust him, though. Do you think you could talk to him, see what he knows?", new ConversationButton[] { new ConversationButton("Accept", "addquest;closeconv") }));
            quest.introConversation = conv;
            quest.introNPC = NPCID.Guide;

            partIndex = quest.AddPart(new QuestPart("Talk to the Merchant"));
            conv = new Conversation();
            conv.conversationParts.Add(new ConversationPart("Ah, you want to know more about me, do you?", new ConversationButton[] { new ConversationButton("Yes", "convup") }));
            conv.conversationParts.Add(new ConversationPart("There's not much I can tell you, I'm afraid. I'm a Merchant by trade, it's what I do and what I did all my life.", new ConversationButton[] { new ConversationButton("Continue", "convup") }));
            conv.conversationParts.Add(new ConversationPart("Then one day, I was on my way to a fair and 'poof', here I was. I've got as much of a clue as to how I got here as you do, believe me.", new ConversationButton[] { new ConversationButton("Continue", "convup") }));
            conv.conversationParts.Add(new ConversationPart("So that's it... Like I said, there's not much to tell. Anyway, it seems my supplies came along with me too this... Place. If you need something, I'm setting up shop so come and find me.", new ConversationButton[] { new ConversationButton("Accept", "questpartup;closeconv") }));
            quest.questParts[partIndex].AddConversation(NPCID.Merchant, conv);

            partIndex = quest.AddPart(new QuestPart("Go back to the Guide"));
            conv = new Conversation();
            conv.conversationParts.Add(new ConversationPart("And... What did you find out?", new ConversationButton[] { new ConversationButton("Tell findings", "convup") }));
            conv.conversationParts.Add(new ConversationPart("I see, so he himself is a Merchant, maybe that's what he was mumbling about! Anyway, if what you told me is true, I think he's not that bad after all.", new ConversationButton[] { new ConversationButton("What now?", "convup") }));
            conv.conversationParts.Add(new ConversationPart("Now we explore some more, see more of this place. Here, I found this while you were gone. It should help you get around faster.", new ConversationButton[] { new ConversationButton("Turn in", "completequest;closeconv") }));
            quest.questParts[partIndex].AddConversation(NPCID.Guide, conv);

            quest.questCompleted = delegate(Player p)
            {
                p.QuickSpawnItem(ItemID.Aglet);
                return true;
                
            };

            quest.AddQuest();

            #endregion

            #region Merchant Quests

            // Quesline

            quest = new Quest("Merch1", "Who's that girl?", this.Name, "The Merchant is eyeballing the Nurse");
            quest.requirement = delegate(Player p)
            {
                return NPC.AnyNPCs(NPCID.Nurse);
            };

            conv = new Conversation();
            conv.conversationParts.Add(new ConversationPart("...", new ConversationButton[] { new ConversationButton("Hello", "convup") }));
            conv.conversationParts.Add(new ConversationPart("...", new ConversationButton[] { new ConversationButton("Hello?", "convup") }));
            conv.conversationParts.Add(new ConversationPart("...", new ConversationButton[] { new ConversationButton("HELLO?!", "convup") }));
            conv.conversationParts.Add(new ConversationPart("Oh, I'm sorry, I was just dazing off. That Nurse is really something, huh? Say, do you think you could... You know talk to her for me?", new ConversationButton[] { new ConversationButton("Accept", "addquest;closeconv") }));
            quest.introConversation = conv;
            quest.introNPC = NPCID.Merchant;

            partIndex = quest.AddPart(new QuestPart("Talk to the Nurse"));
            conv = new Conversation();
            conv.conversationParts.Add(new ConversationPart("Well hello you! What can I do for you love?", new ConversationButton[] { new ConversationButton("Who are you?", "convup") }));
            conv.conversationParts.Add(new ConversationPart("Who I am? Well a Nurse of course!", new ConversationButton[] { new ConversationButton("Continue", "convup") }));
            conv.conversationParts.Add(new ConversationPart("In my hometown, I was one of the few nurses capable of surgery, so I was quite loved.", new ConversationButton[] { new ConversationButton("Continue", "convup") }));
            conv.conversationParts.Add(new ConversationPart("One day, however, a woman brought in her sick baby boy whom I was unable to cure.", new ConversationButton[] { new ConversationButton("Continue", "convup") }));
            conv.conversationParts.Add(new ConversationPart("I got trashed after that incident and nobody came and saw me for their illnesses anymore. That's when I decided to run away...", new ConversationButton[] { new ConversationButton("Continue", "convup") }));
            conv.conversationParts.Add(new ConversationPart("That's when I suddenly got here! I think I might come to like this place... Lots to heal, lots to cure!", new ConversationButton[] { new ConversationButton("Bye", "questpartup;closeconv") }));
            quest.questParts[partIndex].AddConversation(NPCID.Nurse, conv);
            
            partIndex = quest.AddPart(new QuestPart("Talk to the Merchant"));
            conv = new Conversation();
            conv.conversationParts.Add(new ConversationPart("And, what did you find out?", new ConversationButton[] { new ConversationButton("Tell Merchant the story", "convup") }));
            conv.conversationParts.Add(new ConversationPart("Oh my, that's such a sad story! She must be feeling sad if not alone. We should try to cheer her up!", new ConversationButton[] { new ConversationButton("Agree", "convup") }));
            conv.conversationParts.Add(new ConversationPart("Lets see, what could make her feel better... Maybe we could get her some flowers? How about you bring me 10 Dayblooms?", new ConversationButton[] { new ConversationButton("Accept", "questpartup;closeconv") }));
            quest.questParts[partIndex].AddConversation(NPCID.Merchant, conv);

            partIndex = quest.AddPart(new QuestPart("Get the Merchant 10 Dayblooms"));
            conv = new Conversation();
            conv.conversationParts.Add(new ConversationPart("Ah, you got the flowers! Allright, give me a minute...", new ConversationButton[] { new ConversationButton("Wait...", "convup") }));
            conv.conversationParts.Add(new ConversationPart("Aaand tadaa! A nice bouquet of Dayblooms. If anything should cheer her up, this will! Could you bring this over to her?", new ConversationButton[] { new ConversationButton("Accept", "questpartup;closeconv") }));
            quest.questParts[partIndex].AddConversation(NPCID.Merchant, conv);
            quest.questParts[partIndex].partUnfinishedText = "Have you been able to collect those flowers yet?";
            quest.questParts[partIndex].AddItemNeeded(ItemID.Daybloom, 10);

            quest.AddQuest();


            quest = new Quest("Merch2", "Flowers for the Nurse", this.Name, "The Merchant wants some flowers to give to the nurse");
            quest.requirement = delegate(Player p)
            {
                return NPC.AnyNPCs(NPCID.Nurse) && QuestManager.CompletedQuest("Merch1", this.Name);
            };

            conv = new Conversation();
            conv.conversationParts.Add(new ConversationPart("Hey there " + Main.player[Main.myPlayer].name + "! Say, could you give me a hand?", new ConversationButton[] { new ConversationButton("Sure", "convup") }));
            conv.conversationParts.Add(new ConversationPart("Well, the thing is... I've had my eye on the Nurse for a little while now. The Guide said I had to get her some flowers. Think you could get me some?", new ConversationButton[] { new ConversationButton("Accept", "addquest;closeconv") }));
            quest.introConversation = conv;

            partIndex = quest.AddPart(new QuestPart("Get the Merchant 5 Dayblooms"));
            conv = new Conversation();
            conv.conversationParts.Add(new ConversationPart("What's that? You have the flowers?! Thank you!", new ConversationButton[] { new ConversationButton("No problem", "removeitems;completequest;closeconv") }));
            quest.questParts[partIndex].AddConversation(NPCID.Merchant, conv);
            quest.questParts[partIndex].partUnfinishedText = "Have you gotten me those flowers yet?";
            quest.questParts[partIndex].AddItemNeeded(ItemID.Daybloom, 5);

            quest.AddQuest();

            #endregion
        }
    }
}
