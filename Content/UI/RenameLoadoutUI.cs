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

namespace BossLoadouts.Content.UI
{
    public class RenameLoadoutUI : UIState
    {
        private UIPanel panel;
        private UIHeader titleHeader;
        private UITextInputField loadoutNameInput;
        private UITextPanel<string> renameButton;
        private UITextPanel<string> cancelButton;

        private Loadout loadoutToRename;

        public void SetLoadout(Loadout loadout)
        {
            loadoutToRename = loadout;
            if (loadoutToRename != null)
            {
                loadoutNameInput.Text = loadoutToRename.Name;
            }
        }

        public override void OnInitialize()
        {
            panel = new UIPanel
            {
                Width = { Pixels = 400 },
                Height = { Pixels = 200 },
                HAlign = 0.5f,
                VAlign = 0.5f,
            };
            Append(panel);

            titleHeader = new UIHeader("Rename Loadout")
            {
                HAlign = 0.5f,
                VAlign = 0.1f,
            };
            panel.Append(titleHeader);

            loadoutNameInput = new UITextInputField
            {
                Width = { Pixels = 300 },
                Height = { Pixels = 50 },
                HAlign = 0.5f,
                VAlign = 0.4f,
                PlaceholderText = "New Loadout Name",
            };
            panel.Append(loadoutNameInput);

            renameButton = new UITextPanel<string>("Rename")
            {
                Width = { Pixels = 120 },
                Height = { Pixels = 40 },
                HAlign = 0.3f,
                VAlign = 0.75f,
            };
            renameButton.WithFadedMouseOver();
            renameButton.OnLeftClick += RenameButton_OnLeftClick;
            panel.Append(renameButton);

            cancelButton = new UITextPanel<string>("Cancel")
            {
                Width = { Pixels = 120 },
                Height = { Pixels = 40 },
                HAlign = 0.7f,
                VAlign = 0.75f,
            };
            cancelButton.WithFadedMouseOver();
            cancelButton.OnLeftClick += CancelButton_OnLeftClick;
            panel.Append(cancelButton);
        }

        private void RenameButton_OnLeftClick(UIMouseEvent evt, UIElement listeningElement)
        {
            string newName = loadoutNameInput.Text.Trim();
            if (string.IsNullOrEmpty(newName))
            {
                Main.NewText("Loadout name cannot be empty!", 255, 0, 0);
                return;
            }

            if (loadoutToRename != null)
            {
                loadoutToRename.Name = newName;
                Main.NewText($"Loadout renamed to \"{newName}\"", 0, 255, 0);
            }

            // Show the main Loadouts UI again after renaming
            var loadoutsSystem = ModContent.GetInstance<BossLoadoutsSystem>();
            loadoutsSystem.LoadoutsUI.RefreshFolders();  // refresh to show new name if needed
            loadoutsSystem.ShowUI("loadouts");
        }

        private void CancelButton_OnLeftClick(UIMouseEvent evt, UIElement listeningElement)
        {
            var loadoutsSystem = ModContent.GetInstance<BossLoadoutsSystem>();
            loadoutsSystem.ShowUI("loadouts");
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
