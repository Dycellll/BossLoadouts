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
        public static ModKeybind LoadoutUp;
        public static ModKeybind LoadoutDown;
        public static ModKeybind ReloadCurrentLoadout;

        public override void Load()
        {
            ToggleLoadoutsUI = KeybindLoader.RegisterKeybind(Mod, "Toggle Loadouts UI", "L");
            LoadoutUp = KeybindLoader.RegisterKeybind(Mod, "Loadout Up", "O");
            LoadoutDown = KeybindLoader.RegisterKeybind(Mod, "Loadout Down", "P");
            ReloadCurrentLoadout = KeybindLoader.RegisterKeybind(Mod, "Reload Current Loadout", "K");
        }
        public override void Unload()
        {
            ToggleLoadoutsUI = null;
            LoadoutUp = null;
            LoadoutDown = null;
            ReloadCurrentLoadout = null;
        }
    }
}