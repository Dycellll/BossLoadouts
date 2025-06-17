using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BossLoadouts.Systems;
using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;

namespace BossLoadouts.Content
{
    class MyPlayer : ModPlayer
    {
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (KeybindsSystem.ToggleLoadoutsUI.JustPressed)
            {
                var loadoutsSystem = ModContent.GetInstance<BossLoadoutsSystem>();
                if (loadoutsSystem._loadoutsInterface.CurrentState == null)
                {
                    loadoutsSystem.ShowUI();
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
