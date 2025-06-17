using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BossLoadouts.Common.Systems;
using BossLoadouts.Systems;
using BossLoadouts.UI;
using CalamityMod.World.Planets;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.UI;
using Folder = BossLoadouts.Common.Systems.Folder;

namespace BossLoadouts.Content.UI
{
    class BossLoadoutsUI : UIState
    {
        public UIPanel panel;
        public UIHeader titleHeader;
        public UIList foldersList;
        public UITextPanel<string> NewFolderButton;
        public UITextPanel<string> closeButton;
        public override void OnInitialize()
        {
            panel = new UIPanel
            {
                HAlign = 0.5f,
                VAlign = 0.5f,
                Width = { Pixels = 500 },
                Height = { Pixels = 500 },
            };
            Append(panel);

            titleHeader = new UIHeader("Boss Loadouts")
            {
                HAlign = 0.5f,
                VAlign = 0.05f,
            };
            panel.Append(titleHeader);

            foldersList = new UIList
            {
                HAlign = 0.05f,
                VAlign = 0.3f,
                Width = { Percent = 0.9f },
                Height = { Percent = 0.5f },
                ListPadding = 5f,
            };
            panel.Append(foldersList);

            var foldersScrollbar = new UIScrollbar
            {
                HAlign = 0.95f,
                VAlign = 0.3f,
                Height = { Percent = 0.5f },
            };
            panel.Append(foldersScrollbar);
            foldersList.SetScrollbar(foldersScrollbar);

            NewFolderButton = new UITextPanel<string>("Create New Folder")
            {
                HAlign = 0.5f,
                VAlign = 0.85f,
            };
            NewFolderButton.WithFadedMouseOver();
            NewFolderButton.OnLeftClick += (evt, str) =>
            {
                CreateNewFolder();
            };
            panel.Append(NewFolderButton);

            closeButton = new UITextPanel<string>("Close")
            {
                HAlign = 0.5f,
                VAlign = 0.95f,
            };
            closeButton.WithFadedMouseOver();
            closeButton.OnLeftClick += (evt, str) =>
            {
                var loadoutsSystem = ModContent.GetInstance<BossLoadoutsSystem>();
                loadoutsSystem.HideUI();
                Main.playerInventory = false;
            };
            panel.Append(closeButton);

            RefreshFolders();
        }

        private void CreateNewFolder()
        {
            BossLoadoutsSystem loadoutsSystem = ModContent.GetInstance<BossLoadoutsSystem>();
            loadoutsSystem.ShowUI("createfolder");
            RefreshFolders();
        }

        public void RefreshFolders()
        {
            foldersList.Clear();
            Main.NewText($"Folders count: {FoldersManager.Folders.Count}");
            foreach (var folder in FoldersManager.Folders)
            {
                var folderEntry = new UIFolderEntry(folder.Name);
                foldersList.Add(folderEntry);
            }
        }


        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            if (panel.ContainsPoint(Main.MouseScreen))
            {
                Main.LocalPlayer.mouseInterface = true;
                PlayerInput.LockVanillaMouseScroll("uiMSL");
            }
        }
    }
}
