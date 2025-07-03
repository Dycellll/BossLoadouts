using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BossLoadouts.Common.Systems;
using BossLoadouts.Systems;
using CalamityMod;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Input;

namespace BossLoadouts.Content
{
    class MyPlayer : ModPlayer
    {
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (Main.keyState.IsKeyDown(Keys.Escape))
            {
                var loadoutsSystem = ModContent.GetInstance<BossLoadoutsSystem>();
                if(loadoutsSystem._loadoutsInterface.CurrentState != null)
                {
                    loadoutsSystem.HideUI();
                }
            }
            if (KeybindsSystem.ToggleLoadoutsUI.JustPressed)
            {
                var loadoutsSystem = ModContent.GetInstance<BossLoadoutsSystem>();
                if (loadoutsSystem._loadoutsInterface.CurrentState == null)
                {
                    loadoutsSystem.ShowUI("loadouts");
                    Main.playerInventory = false;
                }
                else
                {
                    loadoutsSystem.HideUI();
                }
            }
        }
    }
}
