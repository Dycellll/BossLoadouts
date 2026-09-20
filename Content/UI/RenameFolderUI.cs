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
using BossLoadouts.Content;
using System;

namespace BossLoadouts.Content.UI
{
    class RenameFolderUI : UIState
    {
        public UIPanel panel;
        public UIHeader titleHeader;
        private UITextInputField folderNameInput;
        public UITextPanel<string> renameButton;
        public UITextPanel<string> cancelButton;
        public Folder folderToRename;

        public override void OnInitialize()
        {
            panel = new UIPanel
            {
                HAlign = 0.5f,
                VAlign = 0.5f,
                Width = { Pixels = 400 },
                Height = { Pixels = 400 },
            };
            Append(panel);

            titleHeader = new UIHeader("Rename Folder")
            {
                HAlign = 0.5f,
                VAlign = 0.05f,
            };
            panel.Append(titleHeader);

            folderNameInput = new UITextInputField();
            folderNameInput.HAlign = 0.5f;
            folderNameInput.VAlign = 0.4f;
            folderNameInput.Width.Set(300f, 0f);
            folderNameInput.Height.Set(75f, 0f);
            folderNameInput.PlaceholderText = folderToRename.Name;
            panel.Append(folderNameInput);

            renameButton = new UITextPanel<string>("Rename Folder")
            {
                HAlign = 0.5f,
                VAlign = 0.7f,
            };
            renameButton.WithFadedMouseOver();
            renameButton.OnLeftClick += (evt, element) =>
            {
                if (string.IsNullOrWhiteSpace(folderNameInput.Text))
                {
                    Main.NewText("Folder name cannot be empty!", 255, 0, 0);
                    return;
                }

                string oldName = folderToRename.Name;
                FoldersManager.Folders.Remove(folderToRename);
                folderToRename.Name = folderNameInput.Text;
                FoldersManager.Folders.Add(folderToRename);

                var myPlayer = Main.LocalPlayer.GetModPlayer<MyPlayer>();
                if (myPlayer.CurrentFolderName != null && myPlayer.CurrentFolderName.Equals(oldName, StringComparison.OrdinalIgnoreCase))
                    myPlayer.CurrentFolderName = folderToRename.Name;

                SaveLoadSystem.SaveGlobalData();
                var loadoutsSystem = ModContent.GetInstance<BossLoadoutsSystem>();
                loadoutsSystem.LoadoutsUI.RefreshFolders();
                loadoutsSystem.ShowUI("loadouts");
            };
            panel.Append(renameButton);

            cancelButton = new UITextPanel<string>("Cancel")
            {
                HAlign = 0.5f,
                VAlign = 0.9f,
            };
            cancelButton.WithFadedMouseOver();
            cancelButton.OnLeftClick += (evt, element) =>
            {
                var loadoutsSystem = ModContent.GetInstance<BossLoadoutsSystem>();
                loadoutsSystem.ShowUI("loadouts");
            };
            panel.Append(cancelButton);
        }

        public void SetFolder(Folder folder)
        {
            folderToRename = folder;
            folderNameInput.Text = folder.Name;
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
