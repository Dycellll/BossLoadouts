using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;
using BossLoadouts.Common.Systems;
using BossLoadouts.Systems;
using BossLoadouts.UI;
using Terraria.ModLoader.UI;
using Terraria.GameInput;
using System.IO;
using BossLoadouts.Content;
using System;

namespace BossLoadouts.Content.UI
{
    public class RenameLoadoutUI : UIState
    {
        private UIPanel panel;
        private UIHeader titleHeader;
        private UITextInputField loadoutNameInput;
        private UITextPanel<string> renameButton;
        private UITextPanel<string> cancelButton;

        public Loadout loadoutToRename;
        public Folder folderContext;

        public void SetLoadout(Loadout loadout)
        {
            loadoutToRename = loadout;
            loadoutNameInput.Text = loadout.Name;
        }

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

            titleHeader = new UIHeader("Rename Loadout")
            {
                HAlign = 0.5f,
                VAlign = 0.05f,
            };
            panel.Append(titleHeader);

            loadoutNameInput = new UITextInputField();
            loadoutNameInput.HAlign = 0.5f;
            loadoutNameInput.VAlign = 0.4f;
            loadoutNameInput.Width.Set(300f, 0f);
            loadoutNameInput.Height.Set(75f, 0f);
            loadoutNameInput.PlaceholderText = loadoutToRename.Name;
            panel.Append(loadoutNameInput);

            renameButton = new UITextPanel<string>("Rename Loadout")
            {
                HAlign = 0.5f,
                VAlign = 0.7f,
            };
            renameButton.WithFadedMouseOver();
            renameButton.OnLeftClick += (evt, element) =>
            {
                string newName = loadoutNameInput.Text.Trim();
                if (string.IsNullOrEmpty(newName))
                {
                    Main.NewText("Loadout name cannot be empty!", 255, 0, 0);
                    return;
                }
                if (loadoutToRename != null)
                {
                    string oldName = loadoutToRename.Name;
                    loadoutToRename.Name = newName;

                    var myPlayer = Main.LocalPlayer.GetModPlayer<MyPlayer>();
                    if (folderContext != null
                        && myPlayer.CurrentFolderName != null && myPlayer.CurrentFolderName.Equals(folderContext.Name, StringComparison.OrdinalIgnoreCase)
                        && myPlayer.CurrentLoadoutName != null && myPlayer.CurrentLoadoutName.Equals(oldName, StringComparison.OrdinalIgnoreCase))
                    {
                        myPlayer.CurrentLoadoutName = newName;
                    }
                }
                SaveLoadSystem.SaveGlobalData();
                var loadoutsSystem = ModContent.GetInstance<BossLoadoutsSystem>();
                loadoutsSystem.FolderLoadoutsUI.RefreshLoadouts();
                loadoutsSystem.ShowUI("folderloadouts");
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
                loadoutsSystem.ShowUI("folderloadouts");
            };
            panel.Append(cancelButton);
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
