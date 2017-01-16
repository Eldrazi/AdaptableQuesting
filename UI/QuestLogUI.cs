using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria;
using Terraria.UI;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.UI.Elements;

using AdaptableQuesting.Quests;
using AdaptableQuesting.Entities;

namespace AdaptableQuesting.UI
{
    internal class OnDrawSelfArgs : EventArgs
    {
        public SpriteBatch spriteBatch { get; private set; }

        public OnDrawSelfArgs(SpriteBatch sB)
        {
            spriteBatch = sB;
        }
    }

    internal class QuestLogUI : UIState
    {
        internal UIPanel questLogPanel;
        internal UIPanel questContainerPanel;
        internal UIPanel questDataPanel;

        internal UIMessageBox descriptionText;

        internal UIList questContainer;
        internal FixedUIScrollbar questScrollbar;

        internal static bool visible = false;

        internal const float questLogPanelWidth = 540;
        internal const float questLogPanelHeight = 320;

        public override void OnInitialize()
        {
            questLogPanel = new UIPanel();
            questLogPanel.SetPadding(10);
            questLogPanel.Width.Set(questLogPanelWidth, 0);
            questLogPanel.Height.Set(questLogPanelHeight, 0);
            questLogPanel.Left.Set(Main.screenWidth / 2f - questLogPanel.Width.Pixels / 2f, 0f);
            questLogPanel.Top.Set(Main.screenHeight / 2f - questLogPanel.Height.Pixels / 2f, 0f);
            questLogPanel.BackgroundColor = new Color(73, 94, 171, 75);

            base.Append(questLogPanel);

            questContainerPanel = new UIPanel();
            questContainerPanel.SetPadding(0);
            questContainerPanel.Width.Set((questLogPanelWidth / 2f) - 10, 0f);
            questContainerPanel.Height.Set(questLogPanelHeight, 0f);
            questContainerPanel.Left.Set(0, 0f);
            questContainerPanel.Top.Set(0, 0f);
            questContainerPanel.BackgroundColor = new Color(73, 94, 171, 75);
            questLogPanel.Append(questContainerPanel);
            
            questContainer = new UIList();
            questContainer.OverflowHidden = true;
            questContainer.SetPadding(5);
            questContainer.Width.Set((questLogPanelWidth / 2f) - 10, 0f);
            questContainer.Height.Set(questLogPanelHeight, 0f);
            questContainerPanel.Append(questContainer);

            questScrollbar = new FixedUIScrollbar();
            questScrollbar.Width.Set(25f, 0f);
            questScrollbar.Height.Set(questLogPanelHeight - 42, 0f);
            questScrollbar.Left.Set((questLogPanelWidth / 2f) - 42, 0f);
            questScrollbar.Top.Set(6, 0);
            questContainer.Append(questScrollbar);
            questContainer.SetScrollbar(questScrollbar);

            // Quest Data Panel
            questDataPanel = new UIPanel();
            questDataPanel.SetPadding(10);
            questDataPanel.Width.Set((questLogPanelWidth / 2f) - 20, 0f);
            questDataPanel.Height.Set(questLogPanelHeight, 0f);
            questDataPanel.Left.Set((questLogPanelWidth / 2f), 0f);
            questDataPanel.Top.Set(0, 0f);
            questDataPanel.BackgroundColor = new Color(0, 0, 0, 0);
            questLogPanel.Append(questDataPanel);

            descriptionText = new UIMessageBox("");
            descriptionText.scale = 0.75F;
            descriptionText.Width.Set((questLogPanelWidth / 2f) - 20, 0f);
            descriptionText.Height.Set(questLogPanelHeight, 0f);
            descriptionText.Left.Set(0, 0f);
            descriptionText.Top.Set(0, 0f);
            descriptionText.MarginLeft = 0;
            questDataPanel.Append(descriptionText);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Vector2 mousePosition = new Vector2((float)Main.mouseX, (float)Main.mouseY);

            if (questLogPanel.ContainsPoint(mousePosition))
            {
                Main.LocalPlayer.mouseInterface = true;
            }
        }

        /// <summary>
        /// Called whenever the player opens the QuestLogUI screen.
        /// </summary>
        internal void RecalculateQuests(QuestPlayer player)
        {
            this.questContainer.Clear();

            for (int i = AdaptableQuesting.MaxCurrentQuestsCount - 1; i >= 0; --i)
            {
                if (player.currentQuests[i] != null && !string.IsNullOrEmpty(player.currentQuests[i].id) && !player.currentQuests[i].invalid)
                {
                    QuestUIPanel questUIPanel = new QuestUIPanel();
                    questUIPanel.name = player.currentQuests[i].name;
                    questUIPanel.description = player.currentQuests[i].description;
                    questUIPanel.questID = i;
                    questUIPanel.Initialize();
                    questUIPanel.OnClick += (s, e) =>
                    {
                        string textToSet = "";
                        for (int q = 0; q <= player.currentQuests[questUIPanel.questID].CurrentStage; ++q)
                        {
                            if (!string.IsNullOrEmpty(player.currentQuests[questUIPanel.questID].Stages[q].description))
                            {
                                textToSet += (q == player.currentQuests[questUIPanel.questID].CurrentStage ? "> " : "< ") + player.currentQuests[questUIPanel.questID].Stages[q].description;
                                if (q == player.currentQuests[questUIPanel.questID].CurrentStage)
                                {
                                    foreach (QuestPartElement qpe in player.currentQuests[questUIPanel.questID].Stages[q].killsNeeded)
                                    {
                                        string name;
                                        if (qpe.type != 0) name = Lang.npcName(qpe.type);
                                        else name = qpe.typesName;
                                        textToSet += ": " + qpe.currentAmount + "/" + qpe.amount;
                                    }
                                    /*foreach (QuestPartElement qpe in player.currentQuests[questUIPanel.questID].questParts[q].itemsNeeded)
                                    {
                                        textToSet += " " + qpe.amount + " " + Main.itemName[qpe.type];
                                    }*/
                                }
                                textToSet += "\n";
                            }
                        }

                        descriptionText.SetText(questUIPanel.description + "\n\n" + textToSet);
                    };
                    this.questContainer.Add(questUIPanel);
                }
            }
        }
    }

    internal class QuestUIPanel : UIPanel
    {
        internal UIText nameUIText;

        internal string name = "";
        internal string description = "";

        internal int questID;

        internal const float panelWidth = 224;
        internal const float panelHeight = 50;

        public QuestUIPanel()
        {
            base.Width.Set(panelWidth, 0f);
            base.Height.Set(panelHeight, 0f);
        }

        public override void OnInitialize()
        {
            nameUIText = new UIText(name);
            base.Append(nameUIText);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			base.DrawSelf(spriteBatch);
			if (base.IsMouseHovering)
			{
				Main.hoverItemName = "Click for more information.";
			}
		}
    }

    internal class FixedUIScrollbar : UIScrollbar
    {
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            UserInterface temp = UserInterface.ActiveInstance;
            UserInterface.ActiveInstance = AdaptableQuesting.questLogInterface;
            base.DrawSelf(spriteBatch);
            UserInterface.ActiveInstance = temp;
        }

        public override void MouseDown(UIMouseEvent evt)
        {
            UserInterface temp = UserInterface.ActiveInstance;
            UserInterface.ActiveInstance = AdaptableQuesting.questLogInterface;
            base.MouseDown(evt);
            UserInterface.ActiveInstance = temp;
        }
    }
}
