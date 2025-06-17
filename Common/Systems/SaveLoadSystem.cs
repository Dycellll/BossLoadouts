using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BossLoadouts.Content;
using BossLoadouts.Content.UI;
using BossLoadouts.Systems;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;

namespace BossLoadouts.Common.Systems
{
    public class Folder : UIPanel
    {
        public string Name { get; set; }
        public List<Loadout> Loadouts { get; set; }
        public Folder(string name)
        {
            Name = name;
            Loadouts = new List<Loadout>();
        }

        public UITextPanel<string> OpenButton;
        public UITextPanel<string> RenameButton;
        public UITextPanel<string> DeleteButton;

        public UIPanel panel;
        public override void OnInitialize()
        {
            panel = new UIPanel
            {
                Width = { Pixels = 300 },
                Height = { Pixels = 50 },
                BackgroundColor = new Microsoft.Xna.Framework.Color(73, 94, 171),
            };
            Append(panel);
            var folderName = new UIText(Name)
            {
                Left = { Pixels = 10 },
                Top = { Pixels = 15 },
            };
            panel.Append(folderName);
            OpenButton = new UITextPanel<string>("Open")
            {
                Width = { Pixels = 80 },
                Height = { Pixels = 30 },
                Left = { Pixels = 200 },
                Top = { Pixels = 10 },
            };
            OpenButton.WithFadedMouseOver();
            OpenButton.OnLeftClick += (evt, element) =>
            {
                var loadoutsSystem = ModContent.GetInstance<BossLoadoutsSystem>();
                loadoutsSystem.ShowLoadoutsUIForFolder(this);
            };

            panel.Append(OpenButton);
            RenameButton = new UITextPanel<string>("Rename")
            {
                Width = { Pixels = 80 },
                Height = { Pixels = 30 },
                Left = { Pixels = 110 },
                Top = { Pixels = 10 },
            };
            RenameButton.WithFadedMouseOver();
            RenameButton.OnLeftClick += (evt, element) =>
            {
                var loadoutsSystem = ModContent.GetInstance<BossLoadoutsSystem>();
                var renameUI = loadoutsSystem.RenameFolderUI;
                renameUI.SetFolder(this);
                loadoutsSystem.ShowUI("renamefolder");
            };

            panel.Append(RenameButton);
            DeleteButton = new UITextPanel<string>("Delete")
            {
                Width = { Pixels = 80 },
                Height = { Pixels = 30 },
                Left = { Pixels = 20 },
                Top = { Pixels = 10 },
            };
            DeleteButton.WithFadedMouseOver();
            DeleteButton.OnLeftClick += (evt, element) =>
            {
                var loadoutsSystem = ModContent.GetInstance<BossLoadoutsSystem>();
                var confirmUI = loadoutsSystem.ConfirmDeleteUI;
                confirmUI.SetFolder(this);
                loadoutsSystem.ShowUI("confirmdelete");
            };

            panel.Append(DeleteButton);
        }
    }
    static class FoldersManager
    {
        public static List<Folder> Folders = new List<Folder>();
    }
    class SaveLoadSystem : ModSystem
    {
    }
}
