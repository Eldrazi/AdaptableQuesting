using System;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Terradventure.NPCs
{
    public class GNPC : GlobalNPC
    {
        public override bool PreAI(NPC npc)
        {
            if (npc.type == NPCID.Merchant)
            {
                if (NPC.AnyNPCs(NPCID.Nurse))
                {
                    NPC nurse = Array.Find(Main.npc, (x => x.type == NPCID.Nurse));
                    if (nurse.homeTileX != -1 && nurse.homeTileY != -1 &&
                        npc.homeTileX != nurse.homeTileX && npc.homeTileY != nurse.homeTileY)
                    {
                        npc.homeTileX = nurse.homeTileX;
                        npc.homeTileY = nurse.homeTileY;
                    }
                    else if (npc.homeTileX != -1 && npc.homeTileY != -1)
                    {
                        nurse.homeTileX = npc.homeTileX;
                        nurse.homeTileY = npc.homeTileY;
                    }
                }
            }
            return true;
        }
    }
}
