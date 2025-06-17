using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace BossLoadouts.Content
{
    class KeybindsSystem : ModSystem
    {
        public static ModKeybind ToggleLoadoutsUI;
        public override void Load()
        {
            ToggleLoadoutsUI = KeybindLoader.RegisterKeybind(Mod, "Toggle Loadouts UI", "L");
        }
        public override void Unload()
        {
            ToggleLoadoutsUI = null;
        }
    }
}
