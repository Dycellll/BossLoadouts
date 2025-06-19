using BossLoadouts.Common.Systems;
using BossLoadouts.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.UI;
using System.Diagnostics;
using System.IO;
using System;

namespace BossLoadouts.UI
{
    public class UIFolderEntry : UIPanel
    {
        public UIText FolderName;
        public UITextPanel<string> OpenButton;
        public UITextPanel<string> RenameButton;
        public UITextPanel<string> DeleteButton;
        public UITextPanel<string> ShowInExplorerButton;
        public UITextPanel<string> MoveUpButton;
        public UITextPanel<string> MoveDownButton;

        public UIFolderEntry(Folder folder)
        {
            Width.Set(0f, 1f);
            SetPadding(10f);
            BackgroundColor = new Color(30, 30, 60);

            FolderName = new UIText(folder.Name)
            {
                HAlign = 0.5f,
                Top = new StyleDimension(5f, 0f)
            };
            Append(FolderName);

            float spacing = 8f;
            float buttonHeight = 40f;
            float buttonPadding = 8f;
            float startTop = 35f;
            float currentLeft = 0f;

            var buttons = new (string, Action, bool)[]
            {
                ("Open", () =>
                {
                    ModContent.GetInstance<BossLoadoutsSystem>().ShowLoadoutsUIForFolder(folder);
                }, false),
                ("Rename", () =>
                {
                    var system = ModContent.GetInstance<BossLoadoutsSystem>();
                    system.RenameFolderUI = new Content.UI.RenameFolderUI { folderToRename = folder };
                    system.ShowUI("renamefolder");
                }, false),
                ("Delete", () =>
                {
                    ModContent.GetInstance<BossLoadoutsSystem>()._loadoutsInterface?.SetState(new ConfirmDeleteUI(folder));
                }, false),
                ("Explorer", () =>
                {
                    string path = Path.Combine(ModLoader.ModPath, "BossLoadouts", folder.Name);
                    if (Directory.Exists(path))
                    {
                        Process.Start(new ProcessStartInfo { FileName = path, UseShellExecute = true });
                    }
                    else
                    {
                        Main.NewText("Folder path not found.", Color.Red);
                    }
                }, false),
                ("↑", () =>
                {
                    int index = FoldersManager.Folders.IndexOf(folder);
                    if (index > 0)
                    {
                        (FoldersManager.Folders[index - 1], FoldersManager.Folders[index]) =
                        (FoldersManager.Folders[index], FoldersManager.Folders[index - 1]);
                        SaveLoadSystem.SaveGlobalData();
                        ModContent.GetInstance<BossLoadoutsSystem>().LoadoutsUI.RefreshFolders();
                    }
                }, true),
                ("↓", () =>
                {
                    int index = FoldersManager.Folders.IndexOf(folder);
                    if (index < FoldersManager.Folders.Count - 1)
                    {
                        (FoldersManager.Folders[index + 1], FoldersManager.Folders[index]) =
                        (FoldersManager.Folders[index], FoldersManager.Folders[index + 1]);
                        SaveLoadSystem.SaveGlobalData();
                        ModContent.GetInstance<BossLoadoutsSystem>().LoadoutsUI.RefreshFolders();
                    }
                }, true)
            };

            // Create a horizontal container for the buttons
            UIElement buttonRow = new UIElement
            {
                Top = new StyleDimension(startTop, 0f),
                Left = new StyleDimension(0f, 0f),
                Width = new StyleDimension(0f, 1f),
                Height = new StyleDimension(buttonHeight, 0f)
            };
            Append(buttonRow);

            // Add all buttons to the horizontal row
            foreach (var (text, action, isSquare) in buttons)
            {
                var button = new UITextPanel<string>(text, 0.8f, large: false)
                {
                    PaddingTop = buttonPadding,
                    PaddingBottom = buttonPadding,
                    PaddingLeft = buttonPadding,
                    PaddingRight = buttonPadding,
                    Height = { Pixels = buttonHeight },
                    Width = { Pixels = isSquare ? buttonHeight : CalculateButtonWidth(text) }
                };

                button.WithFadedMouseOver();
                button.OnLeftClick += (evt, element) => action();

                button.Left.Set(currentLeft, 0f);
                button.Top.Set(0f, 0f);
                buttonRow.Append(button);

                currentLeft += button.Width.Pixels + spacing;
            }

            // Set the panel height to accommodate the buttons
            Height.Set(startTop + buttonHeight + 10f, 0f); // 10f for bottom padding
        }

        private float CalculateButtonWidth(string text)
        {
            // Calculate approximate width based on text length
            return text.Length * 8f + 20f; // Adjust these values as needed
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            if (ContainsPoint(Main.MouseScreen))
            {
                Main.LocalPlayer.mouseInterface = true;
                PlayerInput.LockVanillaMouseScroll("uiMSL");
            }
        }
    }
}