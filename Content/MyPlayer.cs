using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BossLoadouts.Common.Systems;
using BossLoadouts.Systems;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Microsoft.Xna.Framework.Input;

namespace BossLoadouts.Content
{
    class MyPlayer : ModPlayer
    {
        public string CurrentFolderName;
        public string CurrentLoadoutName;
        public int NegativeLifeCrystals;

        public override void SaveData(TagCompound tag)
        {
            if (!string.IsNullOrEmpty(CurrentFolderName))
                tag["CurrentFolderName"] = CurrentFolderName;

            if (!string.IsNullOrEmpty(CurrentLoadoutName))
                tag["CurrentLoadoutName"] = CurrentLoadoutName;

            if (NegativeLifeCrystals > 0)
                tag["NegativeLifeCrystals"] = NegativeLifeCrystals;
        }

        public override void LoadData(TagCompound tag)
        {
            CurrentFolderName = tag.ContainsKey("CurrentFolderName") ? tag.GetString("CurrentFolderName") : null;
            CurrentLoadoutName = tag.ContainsKey("CurrentLoadoutName") ? tag.GetString("CurrentLoadoutName") : null;
            NegativeLifeCrystals = tag.ContainsKey("NegativeLifeCrystals")
                ? Math.Clamp(tag.GetInt("NegativeLifeCrystals"), 0, 4)
                : 0;
        }

        public override void ModifyMaxStats(out StatModifier health, out StatModifier mana)
        {
            base.ModifyMaxStats(out health, out mana);

            if (NegativeLifeCrystals > 0)
                health.Flat -= NegativeLifeCrystals * 20;
        }

        public void SetCurrentLoadout(Folder folder, Loadout loadout)
        {
            CurrentFolderName = folder?.Name;
            CurrentLoadoutName = loadout?.Name;
        }

        public bool TryGetCurrentLoadout(out Folder folder, out Loadout loadout, out int index)
        {
            folder = null;
            loadout = null;
            index = -1;

            if (string.IsNullOrEmpty(CurrentFolderName) || string.IsNullOrEmpty(CurrentLoadoutName))
                return false;

            folder = FoldersManager.GetFolderByName(CurrentFolderName);
            if (folder == null)
                return false;

            index = folder.Loadouts.FindIndex(l => l.Name.Equals(CurrentLoadoutName, StringComparison.OrdinalIgnoreCase));
            if (index < 0)
            {
                folder = null;
                return false;
            }

            loadout = folder.Loadouts[index];
            return true;
        }

        private void EquipAndTrack(Folder folder, Loadout loadout)
        {
            loadout.LoadGear(Player);
            SetCurrentLoadout(folder, loadout);
            SoundEngine.PlaySound(SoundID.MenuTick);
        }

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
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

            if (KeybindsSystem.ReloadCurrentLoadout.JustPressed)
            {
                if (TryGetCurrentLoadout(out Folder folder, out Loadout loadout, out _))
                {
                    EquipAndTrack(folder, loadout);
                    Main.NewText($"Reloaded loadout '{loadout.Name}'.", 0, 255, 0);
                }
                else
                {
                    Main.NewText("No loadout is currently equipped.", 255, 0, 0);
                }
            }

            if (KeybindsSystem.LoadoutUp.JustPressed)
            {
                if (TryGetCurrentLoadout(out Folder folder, out _, out int index))
                {
                    if (index > 0)
                    {
                        var target = folder.Loadouts[index - 1];
                        EquipAndTrack(folder, target);
                        Main.NewText($"Switched to loadout '{target.Name}'.", 0, 255, 0);
                    }
                }
                else
                {
                    Main.NewText("No loadout is currently equipped.", 255, 0, 0);
                }
            }

            if (KeybindsSystem.LoadoutDown.JustPressed)
            {
                if (TryGetCurrentLoadout(out Folder folder, out _, out int index))
                {
                    if (index >= 0 && index < folder.Loadouts.Count - 1)
                    {
                        var target = folder.Loadouts[index + 1];
                        EquipAndTrack(folder, target);
                        Main.NewText($"Switched to loadout '{target.Name}'.", 0, 255, 0);
                    }
                }
                else
                {
                    Main.NewText("No loadout is currently equipped.", 255, 0, 0);
                }
            }
        }
    }
}