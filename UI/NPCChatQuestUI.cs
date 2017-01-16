using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameInput;

using AdaptableQuesting.Quests;
using AdaptableQuesting.Entities;

namespace AdaptableQuesting.UI
{
    public class NPCChatQuestUI
    {
        public List<Quest> tempDrawQuests;
        public Quest selectedQuest;

        public int selectedQuestIndex;

        public void Draw(SpriteBatch spriteBatch, Mod mod)
        {
            if (Main.player[Main.myPlayer].talkNPC < 0)
            {
                if (tempDrawQuests != null) tempDrawQuests = null;
                if (selectedQuest != null) selectedQuest = null;
                return;
            }
            
            int vanillaChatHeight;
            string[] array = Utils.WordwrapString(Main.npcChatText, Main.fontMouseText, 460, 10, out vanillaChatHeight);
            vanillaChatHeight += 3;

            int questUIHeight = tempDrawQuests.Count == 0 ? 1 : tempDrawQuests.Count;
            if (selectedQuest != null)
            {
                Conversation conv = null;
                if (!selectedQuest.isActive)
                    conv = selectedQuest.introConversation;
                else
                    conv = selectedQuest.questParts[selectedQuest.currentQuestPart].conversations[Main.npc[Main.player[Main.myPlayer].talkNPC].type];
                   
                questUIHeight = conv.conversationParts[conv.currentConvPart].conversationButtons.Length + 1;
            }

            Color color = new Color(200, 200, 200, 200);
            Texture2D questChatBack = mod.GetTexture("Visuals/Quest_Chat_Back");

            Main.spriteBatch.Draw(questChatBack,
                new Vector2((Main.screenWidth / 2 - Main.chatBackTexture.Width / 2), 100 + vanillaChatHeight * 30 - 8),
                new Rectangle?(new Rectangle(0, 0, questChatBack.Width, questUIHeight * 30)),
                color, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(questChatBack,
                new Vector2((Main.screenWidth / 2 - Main.chatBackTexture.Width / 2), 100 +
                    (vanillaChatHeight * 30) + ((questUIHeight * 30) - 8)),
                new Rectangle?(new Rectangle(0, Main.chatBackTexture.Height - 30, questChatBack.Width, 30)), 
                color, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);

            Rectangle rectangle = new Rectangle(Main.screenWidth / 2 - Main.chatBackTexture.Width / 2,
                100 + (vanillaChatHeight * 30) - 8, questChatBack.Width, (questUIHeight + 1) * 30);

            if (selectedQuest == null)
            {
                color = new Microsoft.Xna.Framework.Color(Main.mouseTextColor, (int)(Main.mouseTextColor / 1.1F),
                    Main.mouseTextColor / 2, Main.mouseTextColor);
                selectedQuestIndex = -1;

                for (int i = 0; i < tempDrawQuests.Count; ++i)
                {
                    string drawString = tempDrawQuests[i].name;
                    if (drawString.Length > 20)
                    {
                        drawString = drawString.Substring(0, 20) + "...";
                    }

                    Vector2 drawStringSize = Main.fontMouseText.MeasureString(drawString);
                    Vector2 vector3 = drawStringSize * 0.5f;

                    int drawXPos = 180 + (Main.screenWidth - 800) / 2;
                    int drawYPos = 100 + (vanillaChatHeight + i) * 30 + 15;

                    float scale = 0.9f;

                    if (Main.mouseX > drawXPos && Main.mouseX < drawXPos + drawStringSize.X &&
                        Main.mouseY > drawYPos && Main.mouseY < drawYPos + drawStringSize.Y && !PlayerInput.IgnoreMouseInterface)
                    {
                        Main.player[Main.myPlayer].mouseInterface = true;
                        Main.player[Main.myPlayer].releaseUseItem = false;
                        scale = 1.1f;

                        selectedQuestIndex = i;
                    }

                    Utils.DrawBorderStringFourWay(Main.spriteBatch, Main.fontMouseText, drawString,
                        drawXPos + vector3.X, drawYPos + vector3.Y, color, Microsoft.Xna.Framework.Color.Black, vector3, scale);
                }

                if (!PlayerInput.IgnoreMouseInterface)
                {
                    if (rectangle.Contains(new Point(Main.mouseX, Main.mouseY)))
                    {
                        Main.player[Main.myPlayer].mouseInterface = true;

                        if (selectedQuestIndex >= 0 && Main.mouseLeft && Main.mouseLeftRelease)
                        {
                            Main.mouseLeftRelease = false;
                            Main.player[Main.myPlayer].releaseUseItem = false;

                            selectedQuest = tempDrawQuests[selectedQuestIndex];

                            Conversation conv;
                            if (!selectedQuest.isActive)
                            {
                                conv = selectedQuest.introConversation;
                            }
                            else
                            {
                                conv = selectedQuest.CurrentPart.conversations[Main.npc[Main.player[Main.myPlayer].talkNPC].type];
                                if (!selectedQuest.CurrentQuestPartFinished())
                                {
                                    Main.npcChatText = selectedQuest.CurrentPart.partUnfinishedText;
                                    selectedQuest = null;
                                    return;
                                }
                            }

                            conv.Reset();
                            Main.npcChatText = conv.CurrentPart.text;
                        }
                    }
                }
            }
            else
            {
                color = new Microsoft.Xna.Framework.Color(Main.mouseTextColor, (int)(Main.mouseTextColor / 1.1F),
                    Main.mouseTextColor / 2, Main.mouseTextColor);
                try
                {
                    Conversation conv = null;
                    if (!selectedQuest.isActive)
                        conv = selectedQuest.introConversation;
                    else
                        conv = selectedQuest.questParts[selectedQuest.currentQuestPart].conversations[Main.npc[Main.player[Main.myPlayer].talkNPC].type];
                    int selectedButton = -1;

                    for (int i = 0; i < conv.CurrentPart.conversationButtons.Length; ++i)
                    {
                        string drawString = conv.CurrentPart.conversationButtons[i].displayName;

                        Vector2 drawStringSize = Main.fontMouseText.MeasureString(drawString);
                        Vector2 vector3 = drawStringSize * 0.5f;

                        int drawXPos = 180 + (Main.screenWidth - 800) / 2;
                        int drawYPos = 100 + (vanillaChatHeight + i) * 30 + 15;

                        float scale = 0.9f;

                        if (Main.mouseX > drawXPos && Main.mouseX < drawXPos + drawStringSize.X &&
                            Main.mouseY > drawYPos && Main.mouseY < drawYPos + drawStringSize.Y && !PlayerInput.IgnoreMouseInterface)
                        {
                            Main.player[Main.myPlayer].mouseInterface = true;
                            Main.player[Main.myPlayer].releaseUseItem = false;
                            scale = 1.1f;

                            selectedButton = i;
                        }

                        Utils.DrawBorderStringFourWay(Main.spriteBatch, Main.fontMouseText, drawString,
                            drawXPos + vector3.X, drawYPos + vector3.Y, color, Color.Black, vector3, scale);
                    }

                    #region Draw Back Button
                    Vector2 backStringSize = Main.fontMouseText.MeasureString("Back");
                    Vector2 backHalfSize = backStringSize * 0.5f;

                    int backDrawXPos = 180 + (Main.screenWidth - 800) / 2;
                    int backDrawYPos = 100 + (vanillaChatHeight * 30) + (questUIHeight * 30) - 8;
                    float backScale = 0.8f;

                    if (Main.mouseX > backDrawXPos && Main.mouseX < backDrawXPos + backStringSize.X &&
                        Main.mouseY > backDrawYPos && Main.mouseY < backDrawYPos + backStringSize.Y && !PlayerInput.IgnoreMouseInterface)
                    {
                        Main.player[Main.myPlayer].mouseInterface = true;
                        Main.player[Main.myPlayer].releaseUseItem = false;
                        backScale = 1f;

                        selectedButton = -2;
                    }

                    Utils.DrawBorderStringFourWay(Main.spriteBatch, Main.fontMouseText, "Back",
                        backDrawXPos + backHalfSize.X, backDrawYPos + backHalfSize.Y, color, Color.Black, backHalfSize, backScale);
                    #endregion

                    if (!PlayerInput.IgnoreMouseInterface)
                    {
                        if (rectangle.Contains(new Point(Main.mouseX, Main.mouseY)))
                        {
                            Main.player[Main.myPlayer].mouseInterface = true;

                            if (selectedButton >= 0 && Main.mouseLeft && Main.mouseLeftRelease)
                            {
                                Main.mouseLeftRelease = false;
                                Main.player[Main.myPlayer].releaseUseItem = false;

                                string[] actions = conv.CurrentPart.conversationButtons[selectedButton].actions.Split(';');
                                for (int i = 0; i < actions.Length; ++i)
                                {
                                    string action = actions[i].ToLower();

                                    if (action == "convup")
                                    {
                                        conv.currentConvPart++;
                                        Main.npcChatText = conv.CurrentPart.text;
                                    }
                                    else if (action == "closeconv")
                                    {
                                        Main.CloseNPCChatOrSign();
                                    }
                                    else if (action == "addquest")
                                    {
                                        QuestPlayer qp = Main.player[Main.myPlayer].GetModPlayer<QuestPlayer>(mod);
                                        qp.AddQuest(selectedQuest);
                                    }
                                    else if (action == "completequest")
                                    {
                                        selectedQuest.CompleteQuest();
                                    }
                                    else if (action == "questpartup")
                                    {
                                        selectedQuest.currentQuestPart++;
                                        /*conv = selectedQuest.questParts[selectedQuest.currentQuestPart].conversations[Main.npc[Main.player[Main.myPlayer].talkNPC].type];
                                        Main.npcChatText = conv.CurrentPart.text;*/
                                    }
                                    else if (action == "removeitems")
                                    {
                                        if (selectedQuest.CurrentPart.itemsNeeded.Count > 0)
                                        {
                                            foreach (QuestPartElement element in selectedQuest.CurrentPart.itemsNeeded)
                                            {
                                                int amountToRemove = element.amount;

                                                for (int it = 0; it < 58; it++)
                                                {
                                                    if (element.type == Main.player[Main.myPlayer].inventory[it].type)
                                                    {
                                                        if (Main.player[Main.myPlayer].inventory[it].stack >= amountToRemove)
                                                            Main.player[Main.myPlayer].inventory[it].stack -= amountToRemove;
                                                        else
                                                        {
                                                            int tempStackAmount = Main.player[Main.myPlayer].inventory[it].stack;
                                                            Main.player[Main.myPlayer].inventory[it].stack -= tempStackAmount;
                                                            amountToRemove -= tempStackAmount;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else if (action == "additem")
                                    {
                                        string[] splitAction = action.Split(':');

                                        int type = int.Parse(splitAction[1]);
                                        int amount = 1; if (splitAction.Length >= 3) amount = int.Parse(splitAction[2]);

                                        Main.player[Main.myPlayer].QuickSpawnItem(type, amount);
                                    }
                                }
                            }
                            else if (selectedButton == -2 && Main.mouseLeft && Main.mouseLeftRelease)
                            {
                                Main.mouseLeftRelease = false;
                                Main.player[Main.myPlayer].releaseUseItem = false;

                                selectedQuest = null;
                                Main.npcChatText = Main.npc[Main.player[Main.myPlayer].talkNPC].GetChat();
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Main.NewText(e.Message);
                }
            }
        }
    }
}
