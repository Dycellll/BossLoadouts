using System;
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

class ConfirmDeleteLoadoutUI : UIState
{
    private UIPanel panel;
    private UIText message;
    private UITextPanel<string> yesButton;
    private UITextPanel<string> noButton;
    public Loadout loadoutToDelete;
    public Folder currentFolder;

    public ConfirmDeleteLoadoutUI(Loadout loadout, Folder CurrentFolder)
    {
        loadoutToDelete = loadout;
        currentFolder = CurrentFolder;

        panel = new UIPanel
        {
            HAlign = 0.5f,
            VAlign = 0.5f,
            Width = { Pixels = 300 },
            Height = { Pixels = 150 }
        };
        Append(panel);

        message = new UIText("Are you sure you want to \ndelete this loadout?")
        {
            HAlign = 0.5f,
            VAlign = 0.1f,
        };
        panel.Append(message);

        yesButton = new UITextPanel<string>("Yes")
        {
            Width = { Pixels = 100 },
            Height = { Pixels = 40 },
            HAlign = 0.15f,
            VAlign = 0.7f,
        };
        yesButton.WithFadedMouseOver();
        yesButton.OnLeftClick += (evt, element) => DeleteLoadout();
        panel.Append(yesButton);

        noButton = new UITextPanel<string>("No")
        {
            Width = { Pixels = 100 },
            Height = { Pixels = 40 },
            HAlign = 0.85f,
            VAlign = 0.7f,
        };
        noButton.WithFadedMouseOver();
        noButton.OnLeftClick += (evt, element) =>
        {
            var loadoutsSystem = ModContent.GetInstance<BossLoadoutsSystem>();
            loadoutsSystem.ShowUI("folderloadouts");
        };
        panel.Append(noButton);
        currentFolder = CurrentFolder;
    }

    private void DeleteLoadout()
    {
        if (loadoutToDelete == null)
        {
            Main.NewText("Error: loadoutToDelete is null!", Color.Red);
            return;
        }

        bool removed = currentFolder.Loadouts.Remove(loadoutToDelete);
        if (removed)
        {
            Main.NewText($"Loadout '{loadoutToDelete.Name}' deleted successfully!", 0, 255, 0);
        }
        else
        {
            Main.NewText($"Failed to delete loadout '{loadoutToDelete.Name}'. It may not exist.", 255, 0, 0);
        }

        SaveLoadSystem.SaveGlobalData();
        var loadoutsSystem = ModContent.GetInstance<BossLoadoutsSystem>();
        loadoutsSystem.ShowUI("folderloadouts");
        loadoutsSystem.FolderLoadoutsUI.RefreshLoadouts();
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
