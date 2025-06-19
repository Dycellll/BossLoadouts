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
    class CreateFolderUI : UIState
    {
        public UIPanel panel;
        public UIHeader titleHeader;
        private UITextInputField folderNameInput;
        public UITextPanel<string> createButton;
        public UITextPanel<string> cancelButton;
        public override void OnInitialize()
        {
            panel = new UIPanel
            {
                HAlign = 0.5f,
                VAlign = 0.5f,
                Width = { Pixels = 500 },
                Height = { Pixels = 500 },
            };
            panel.BackgroundColor.A = 100;
            Append(panel);

            titleHeader = new UIHeader("Create New Folder")
            {
                HAlign = 0.5f,
                VAlign = 0.05f,
            };
            panel.Append(titleHeader);

            folderNameInput = new UITextInputField();
            folderNameInput.HAlign = 0.5f;
            folderNameInput.VAlign = 0.35f;
            folderNameInput.Width.Set(300f, 0.5f);
            folderNameInput.Height.Set(75f, 0f);
            folderNameInput.PlaceholderText = "My Folder";
            panel.Append(folderNameInput);

            createButton = new UITextPanel<string>("Create Folder")
            {
                HAlign = 0.5f,
                VAlign = 0.85f,
            };
            createButton.WithFadedMouseOver();
            createButton.OnLeftClick += (evt, element) =>
            {
                if(folderNameInput.Text.Length == 0)
                {
                    Main.NewText("Folder name cannot be empty!", 255, 0, 0);
                    return;
                }
                Folder newFolder = new Folder(folderNameInput.Text);
                folderNameInput.Text = "";
                FoldersManager.Folders.Add(newFolder);
                SaveLoadSystem.SaveGlobalData();
                BossLoadoutsSystem loadoutsSystem = ModContent.GetInstance<BossLoadoutsSystem>();
                loadoutsSystem.LoadoutsUI.RefreshFolders();
                loadoutsSystem.ShowUI("loadouts");
            };
            panel.Append(createButton);

            cancelButton = new UITextPanel<string>("Cancel")
            {
                HAlign = 0.5f,
                VAlign = 0.95f,
            };
            cancelButton.WithFadedMouseOver();
            cancelButton.OnLeftClick += (evt, element) =>
            {
                BossLoadoutsSystem loadoutsSystem = ModContent.GetInstance<BossLoadoutsSystem>();
                loadoutsSystem.ShowUI("loadouts");
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
