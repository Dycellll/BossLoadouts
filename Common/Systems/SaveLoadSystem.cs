using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using BossLoadouts.Systems;
using CalamityMod;
using CalamityMod.Buffs.Alcohol;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace BossLoadouts.Common.Systems
{
    public class Folder
    {
        public string Name { get; set; }
        public List<Loadout> Loadouts { get; set; } = new List<Loadout>();
        public Folder(string name)
        {
            Name = name;
        }
    }
    static class FoldersManager
    {
        public static List<Folder> Folders = new List<Folder>();
    }
    class SaveLoadSystem : ModSystem
    {
        public static string BasePath => Path.Combine(ModLoader.ModPath, "BossLoadouts");

        public static void SaveGlobalData()
        {
            try
            {
                Directory.CreateDirectory(BasePath);

                // Save current folders and loadouts
                foreach (var folder in FoldersManager.Folders)
                {
                    string folderPath = Path.Combine(BasePath, SanitizeFileName(folder.Name));
                    Directory.CreateDirectory(folderPath);

                    // Clear old loadout files
                    foreach (var oldFile in Directory.GetFiles(folderPath, "*.dat"))
                        File.Delete(oldFile);

                    string loadoutsOrderPath = Path.Combine(folderPath, "loadouts.json");
                    File.WriteAllText(loadoutsOrderPath, JsonSerializer.Serialize(folder.Loadouts.Select(l => l.Name)));

                    // Save each loadout
                    foreach (var loadout in folder.Loadouts)
                    {
                        string loadoutFilePath = Path.Combine(folderPath, SanitizeFileName(loadout.Name) + ".dat");

                        var loadoutTag = new TagCompound
                        {
                            ["Name"] = loadout.Name,
                            ["ConsumedLifeFruit"] = loadout.ConsumedLifeFruit,
                            ["Powerups"] = loadout.Powerups,
                            ["DownedBosses"] = loadout.downedBosses.Select(b => b ? (byte)1 : (byte)0).ToArray(),
                            ["Inventory"] = loadout.Inventory.Select(ItemIO.Save).ToList(),
                            ["Armor"] = loadout.Armor.Select(ItemIO.Save).ToList(),
                            ["Hook"] = ItemIO.Save(loadout.Hook),
                            ["Mount"] = ItemIO.Save(loadout.Mount),
                            ["Ammo"] = loadout.Ammo.Select(ItemIO.Save).ToList()
                        };

                        TagIO.ToFile(loadoutTag, loadoutFilePath);
                    }
                }

                // Save folder order
                string orderPath = Path.Combine(BasePath, "folders.json");
                File.WriteAllText(orderPath, JsonSerializer.Serialize(FoldersManager.Folders.Select(f => f.Name)));

                // --- Delete unused folders ---
                var currentFolderNames = new HashSet<string>(FoldersManager.Folders.Select(f => SanitizeFileName(f.Name)));
                foreach (var dir in Directory.GetDirectories(BasePath))
                {
                    string folderName = Path.GetFileName(dir);
                    if (folderName == "folders.json") continue; // Just in case

                    if (!currentFolderNames.Contains(folderName))
                    {
                        try
                        {
                            Directory.Delete(dir, true); // true = recursive delete
                        }
                        catch (Exception ex)
                        {
                            ModContent.GetInstance<BossLoadouts>().Logger.Warn($"Failed to delete unused folder '{folderName}': {ex}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ModContent.GetInstance<BossLoadouts>().Logger.Error("Failed to save global data: " + ex);
            }
        }

        public static void LoadGlobalData()
        {
            try
            {
                if (!Directory.Exists(BasePath))
                    return;

                FoldersManager.Folders.Clear();

                string orderPath = Path.Combine(BasePath, "folders.json");
                List<string> orderedFolderNames = new();

                if (File.Exists(orderPath))
                {
                    orderedFolderNames = JsonSerializer.Deserialize<List<string>>(File.ReadAllText(orderPath));
                }

                // Get all folders by name for lookup
                var folderDirs = Directory.GetDirectories(BasePath)
                                          .Where(d => Path.GetFileName(d) != "folders.json") // Just in case
                                          .ToDictionary(Path.GetFileName, d => d);

                // Load in saved order
                foreach (var name in orderedFolderNames)
                {
                    if (!folderDirs.TryGetValue(name, out string folderDir))
                        continue;

                    var folder = LoadFolder(folderDir);
                    if (folder != null)
                        FoldersManager.Folders.Add(folder);

                    folderDirs.Remove(name); // Remove from dictionary to avoid duplicate loading
                }

                // Load any new folders not present in the saved order
                foreach (var leftoverDir in folderDirs.Values)
                {
                    var folder = LoadFolder(leftoverDir);
                    if (folder != null)
                        FoldersManager.Folders.Add(folder);
                }
            }
            catch (Exception ex)
            {
                ModContent.GetInstance<BossLoadouts>().Logger.Error("Failed to load global data: " + ex);
            }
        }

        private static Folder LoadFolder(string folderDir)
        {
            try
            {
                string folderName = Path.GetFileName(folderDir);
                var folder = new Folder(folderName);

                // Load order file if exists
                string loadoutsOrderPath = Path.Combine(folderDir, "loadouts.json");
                List<string> loadoutNamesOrder = null;
                if (File.Exists(loadoutsOrderPath))
                {
                    loadoutNamesOrder = JsonSerializer.Deserialize<List<string>>(File.ReadAllText(loadoutsOrderPath));
                }

                // Get all loadout files mapped by sanitized name for lookup
                var loadoutFiles = Directory.GetFiles(folderDir, "*.dat")
                                           .ToDictionary(f => Path.GetFileNameWithoutExtension(f), f => f);

                if (loadoutNamesOrder != null)
                {
                    // Load loadouts in the saved order
                    foreach (var loadoutName in loadoutNamesOrder)
                    {
                        string sanitizedName = SanitizeFileName(loadoutName);
                        if (!loadoutFiles.TryGetValue(sanitizedName, out string loadoutFile))
                            continue;

                        var loadoutTag = TagIO.FromFile(loadoutFile);
                        var loadout = CreateLoadoutFromTag(loadoutTag);
                        folder.Loadouts.Add(loadout);

                        loadoutFiles.Remove(sanitizedName);
                    }
                }

                // Load any new loadouts not in the order file (append at the end)
                foreach (var loadoutFile in loadoutFiles.Values)
                {
                    var loadoutTag = TagIO.FromFile(loadoutFile);
                    var loadout = CreateLoadoutFromTag(loadoutTag);
                    folder.Loadouts.Add(loadout);
                }

                return folder;
            }
            catch (Exception ex)
            {
                ModContent.GetInstance<BossLoadouts>().Logger.Error("Failed to load folder: " + ex);
                return null;
            }
        }

        private static Loadout CreateLoadoutFromTag(TagCompound loadoutTag)
        {
            byte[] downedBytes = loadoutTag.GetByteArray("DownedBosses");
            return new Loadout
            {
                Name = loadoutTag.GetString("Name"),
                ConsumedLifeFruit = loadoutTag.GetInt("ConsumedLifeFruit"),
                Powerups = loadoutTag.GetIntArray("Powerups"),
                downedBosses = downedBytes.Select(b => b == 1).ToArray(),
                Inventory = loadoutTag.GetList<TagCompound>("Inventory").Select(ItemIO.Load).ToArray(),
                Armor = loadoutTag.GetList<TagCompound>("Armor").Select(ItemIO.Load).ToArray(),
                Hook = ItemIO.Load(loadoutTag.Get<TagCompound>("Hook")),
                Mount = ItemIO.Load(loadoutTag.Get<TagCompound>("Mount")),
                Ammo = loadoutTag.GetList<TagCompound>("Ammo").Select(ItemIO.Load).ToArray()
            };
        }
        private static string SanitizeFileName(string name)
        {
            foreach (var c in Path.GetInvalidFileNameChars())
                name = name.Replace(c, '_');
            return name;
        }

        public override void OnWorldLoad()
        {
            LoadGlobalData();
            BossLoadoutsSystem loadoutsSystem = ModContent.GetInstance<BossLoadoutsSystem>();
            loadoutsSystem.HideUI();
        }
        public override void OnWorldUnload()
        {
            SaveGlobalData();
            BossLoadoutsSystem loadoutsSystem = ModContent.GetInstance<BossLoadoutsSystem>();
            loadoutsSystem.HideUI();
        }
    }

    class SaveLoadPlayer : ModPlayer
    {
        private int savedShroomLevel = 0;
        private bool reTrippy = false;
        public override void SaveData(TagCompound tag)
        {
            var modPlayer = Player.Calamity();
            if (!modPlayer.trippy)
            {
                tag["shroomedLevel"] = 0;
            }
            else
            {
                tag["shroomedLevel"] = modPlayer.trippyLevel;
            }
        }

        public override void LoadData(TagCompound tag)
        {
            savedShroomLevel = 0;
            reTrippy = false;
            if (tag.ContainsKey("shroomedLevel"))
            {
                int level = tag.GetInt("shroomedLevel");
                var modPlayer = Player.Calamity();

                if(level > 0)
                {
                    modPlayer.trippyLevel = level;
                    modPlayer.trippy = level > 0;
                }
                else
                {
                    modPlayer.trippyLevel = 0;
                    modPlayer.trippy = false;
                    if (Player.HasBuff<Trippy>())
                    {
                        Player.ClearBuff(ModContent.BuffType<Trippy>());
                    }
                }
            }
            else if (tag.ContainsKey("shroomed"))
            {
                int level = tag.GetInt("shroomed");
                var modPlayer = Player.Calamity();
                if (level > 0)
                {
                    modPlayer.trippyLevel = level;
                    modPlayer.trippy = level > 0;
                }
                else
                {
                    modPlayer.trippyLevel = 0;
                    modPlayer.trippy = false;
                    if (Player.HasBuff<Trippy>())
                    {
                        Player.ClearBuff(ModContent.BuffType<Trippy>());
                    }
                }
            }
        }

        public override void OnEnterWorld()
        {
            var modPlayer = Player.Calamity();

            if (modPlayer.trippyLevel > 0)
            {
                Player.AddBuff(ModContent.BuffType<Trippy>(), int.MaxValue);
                modPlayer.trippy = true;
            }
            else
            {
                if (Player.HasBuff<Trippy>())
                {
                    Player.ClearBuff(ModContent.BuffType<Trippy>());
                }
                modPlayer.trippy = false;
            }
        }

        public override void PostUpdateBuffs()
        {
            var modPlayer = Player.Calamity();
            BossLoadoutsSystem loadoutsSystem = ModContent.GetInstance<BossLoadoutsSystem>();

            if (loadoutsSystem.BuffsEditorUI == null || loadoutsSystem.BuffsEditorUI.shroomedToggleButton == null)
                return;

            if (modPlayer.trippyLevel > 0 && (loadoutsSystem.BuffsEditorUI.shroomedToggleButton.BackgroundColor == Color.Green || loadoutsSystem.BuffsEditorUI.shroomedToggleButton.BackgroundColor == Color.DarkGreen))
            {
                if (!Player.HasBuff<Trippy>())
                {
                    Player.AddBuff(ModContent.BuffType<Trippy>(), 60);
                }
                modPlayer.trippy = true;
            }
            else
            {
                if (Player.HasBuff<Trippy>())
                {
                    Player.ClearBuff(ModContent.BuffType<Trippy>());
                }
                modPlayer.trippy = false;
            }
        }

        public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genDust, ref PlayerDeathReason damageSource)
        {
            savedShroomLevel = Player.Calamity().trippyLevel;
            if(savedShroomLevel != 0)
            {
                reTrippy = true;
            }
            else
            {
                reTrippy = false;
            }
            return base.PreKill(damage, hitDirection, pvp, ref playSound, ref genDust, ref damageSource);
        }

        public override void OnRespawn()
        {
            var modPlayer = Player.Calamity();

            modPlayer.trippyLevel = savedShroomLevel;

            if (reTrippy)
            {
                Player.AddBuff(ModContent.BuffType<Trippy>(), int.MaxValue);
                modPlayer.trippy = true;
            }
        }
    }
}
