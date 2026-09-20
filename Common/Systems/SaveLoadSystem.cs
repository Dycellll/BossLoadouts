using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using BossLoadouts.Systems;
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
        public float ScrollPosition { get; set; } = 0f;
        public Folder(string name)
        {
            Name = name;
        }
    }
    static class FoldersManager
    {
        public static List<Folder> Folders = new List<Folder>();
        public static Folder GetFolderByName(string name)
        {
            return Folders.FirstOrDefault(f => f.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }
        public static int GetFolderIndexByName(string name)
        {
            return Folders.FindIndex(f => f.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }
    }
    class SaveLoadSystem : ModSystem
    {
        public static string BasePath => Path.Combine(ModLoader.ModPath, "BossLoadouts");

        public static void SaveGlobalData()
        {
            try
            {
                Directory.CreateDirectory(BasePath);

                foreach (var folder in FoldersManager.Folders)
                {
                    string folderPath = Path.Combine(BasePath, SanitizeFileName(folder.Name));
                    Directory.CreateDirectory(folderPath);

                    foreach (var oldFile in Directory.GetFiles(folderPath, "*.dat"))
                        File.Delete(oldFile);

                    string loadoutsOrderPath = Path.Combine(folderPath, "loadouts.json");
                    File.WriteAllText(loadoutsOrderPath, JsonSerializer.Serialize(folder.Loadouts.Select(l => l.Name)));

                    foreach (var loadout in folder.Loadouts)
                    {
                        string loadoutFilePath = Path.Combine(folderPath, SanitizeFileName(loadout.Name) + ".dat");

                        var loadoutTag = new TagCompound
                        {
                            ["Name"] = loadout.Name,
                            ["ConsumedLifeCrystals"] = loadout.ConsumedLifeCrystals,
                            ["ConsumedLifeFruit"] = loadout.ConsumedLifeFruit,
                            ["ConsumedManaCrystals"] = loadout.ConsumedManaCrystals,
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

                string orderPath = Path.Combine(BasePath, "folders.json");
                File.WriteAllText(orderPath, JsonSerializer.Serialize(FoldersManager.Folders.Select(f => f.Name)));

                var currentFolderNames = new HashSet<string>(FoldersManager.Folders.Select(f => SanitizeFileName(f.Name)));
                foreach (var dir in Directory.GetDirectories(BasePath))
                {
                    string folderName = Path.GetFileName(dir);
                    if (folderName == "folders.json") continue;

                    if (!currentFolderNames.Contains(folderName))
                    {
                        try
                        {
                            Directory.Delete(dir, true);
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

                var folderDirs = Directory.GetDirectories(BasePath)
                                          .Where(d => Path.GetFileName(d) != "folders.json")
                                          .ToDictionary(Path.GetFileName, d => d);

                foreach (var name in orderedFolderNames)
                {
                    if (!folderDirs.TryGetValue(name, out string folderDir))
                        continue;

                    var folder = LoadFolder(folderDir);
                    if (folder != null)
                        FoldersManager.Folders.Add(folder);

                    folderDirs.Remove(name);
                }

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

                string loadoutsOrderPath = Path.Combine(folderDir, "loadouts.json");
                List<string> loadoutNamesOrder = null;
                if (File.Exists(loadoutsOrderPath))
                {
                    loadoutNamesOrder = JsonSerializer.Deserialize<List<string>>(File.ReadAllText(loadoutsOrderPath));
                }

                var loadoutFiles = Directory.GetFiles(folderDir, "*.dat")
                                           .ToDictionary(f => Path.GetFileNameWithoutExtension(f), f => f);

                if (loadoutNamesOrder != null)
                {
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

                ConsumedLifeCrystals = loadoutTag.ContainsKey("ConsumedLifeCrystals")
                    ? loadoutTag.GetInt("ConsumedLifeCrystals")
                    : Player.LifeCrystalMax,
                ConsumedManaCrystals = loadoutTag.ContainsKey("ConsumedManaCrystals")
                    ? loadoutTag.GetInt("ConsumedManaCrystals")
                    : Player.ManaCrystalMax,

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
}