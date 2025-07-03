using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BossLoadouts.Common.Systems;
using BossLoadouts.Systems;
using BossLoadouts.UI;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.UI;

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
                VAlign = 0.35f,
                Width = { Percent = 0.9f },
                Height = { Percent = 0.35f },
                ListPadding = 5f,
                ManualSortMethod = (e) => { }
            };
            panel.Append(foldersList);

            var foldersScrollbar = new UIScrollbar
            {
                HAlign = 0.95f,
                VAlign = 0.35f,
                Height = { Percent = 0.35f },
            };
            panel.Append(foldersScrollbar);
            foldersList.SetScrollbar(foldersScrollbar);

            NewFolderButton = new UITextPanel<string>("Create New Folder")
            {
                HAlign = 0.5f,
                VAlign = 0.65f,
            };
            NewFolderButton.WithFadedMouseOver();
            NewFolderButton.OnLeftClick += (evt, str) =>
            {
                CreateNewFolder();
            };
            panel.Append(NewFolderButton);

            var permanentBuffsButton = new UITextPanel<string>("Permanent Buffs Editor")
            {
                HAlign = 0.5f,
                VAlign = 0.75f,
            };
            permanentBuffsButton.WithFadedMouseOver();
            permanentBuffsButton.OnLeftClick += (evt, str) =>
            {
                var loadoutsSystem = ModContent.GetInstance<BossLoadoutsSystem>();
                loadoutsSystem.ShowUI("buffseditor");
            };
            panel.Append(permanentBuffsButton);

            var downedBossesButton = new UITextPanel<string>("Downed Bosses Editor")
            {
                HAlign = 0.5f,
                VAlign = 0.85f,
            };
            downedBossesButton.WithFadedMouseOver();
            downedBossesButton.OnLeftClick += (evt, str) =>
            {
                var loadoutsSystem = ModContent.GetInstance<BossLoadoutsSystem>();
                loadoutsSystem.ShowUI("downededitor");
            };
            panel.Append(downedBossesButton);

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
            foreach (var folder in FoldersManager.Folders)
            {
                var folderEntry = new UIFolderEntry(folder);
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
