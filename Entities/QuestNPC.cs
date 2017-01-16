using System;
using System.Linq;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using AdaptableQuesting.Quests;

namespace AdaptableQuesting.Entities
{
    public class QuestNPC : GlobalNPC
    {
        public override void NPCLoot(NPC npc)
        {
            QuestPlayer p = Main.player[Main.myPlayer].GetModPlayer<QuestPlayer>(mod);
            if (!p.player.dead)
            {
                for (int i = 0; i < p.currentQuests.Length; ++i)
                {
                    if (p.currentQuests[i] != null)
                    {
                        QuestStage part = p.currentQuests[i].CurrentStageObject;
                        foreach(QuestPartElement element in part.killsNeeded)
                        {
                            if (element.currentAmount < element.amount)
                            {
                                if (element.type == npc.netID)
                                {
                                    element.currentAmount++;
                                }
                                else if (element.types != null && element.type == 0 && 
                                    Array.Exists(element.types, x => x == npc.netID))
                                {
                                    element.currentAmount++;
                                }
                            }
                        }
                    }
                }
            }            
        }

        public override void GetChat(NPC npc, ref string chat)
        {
            AdaptableQuesting.chatQuestUI.tempDrawQuests = QuestManager.GetAllAvailableQuestsForNPC(npc);
        }

        public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Color drawColor)
        {
            if (npc.active && npc.townNPC)
            {
                QuestNPCInfo info = npc.GetModInfo<QuestNPCInfo>(mod);
                if (info.questAvailable == QuestAvailability.None) return;
                Texture2D texture = mod.GetTexture("Visuals/" + info.questAvailable);
                Vector2 drawPos = new Vector2(npc.Center.X - texture.Width / 2, npc.position.Y - texture.Height - 10) - Main.screenPosition;
                spriteBatch.Draw(texture, drawPos, Color.White);
            }
        }
    }
}
