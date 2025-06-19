using BossLoadouts.Common.Systems;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.UI;
using Terraria;
using Terraria.ModLoader.UI;
using Terraria.ModLoader;
using BossLoadouts.Systems;
using BossLoadouts.Content.UI;
using Microsoft.Xna.Framework;

public class FolderLoadoutsUI : UIState
{
    private UIPanel mainPanel;
    private UIText folderTitle;
    private UIList loadoutsList;
    private UIScrollbar scrollbar;

    private UITextPanel<string> backButton;
    private UITextPanel<string> addLoadoutButton;

    private Folder currentFolder;

    public FolderLoadoutsUI(Folder folder)
    {
        if (folder == null)
        {
            Main.NewText("Error: Tried to open a folder, but it was null!", Color.Red);
            return;
        }

        if (folder.Loadouts == null)
        {
            folder.Loadouts = new List<Loadout>();
        }

        currentFolder = folder;

        mainPanel = new UIPanel();
        mainPanel.Width.Set(500f, 0f);   // Match BossLoadoutsUI panel width
        mainPanel.Height.Set(500f, 0f);  // Match BossLoadoutsUI panel height
        mainPanel.HAlign = 0.5f;
        mainPanel.VAlign = 0.5f;
        Append(mainPanel);

        folderTitle = new UIText($"Loadouts in \"{folder.Name}\"");
        folderTitle.HAlign = 0.5f;
        folderTitle.Top.Set(10f, 0f);
        mainPanel.Append(folderTitle);

        addLoadoutButton = new UITextPanel<string>("Add Loadout");
        addLoadoutButton.Width.Set(120f, 0f);
        addLoadoutButton.Height.Set(40f, 0f);  // Match UIFolderEntry button height
        addLoadoutButton.Top.Set(40f, 0f);
        addLoadoutButton.Left.Set(20f, 0f);
        addLoadoutButton.WithFadedMouseOver();
        addLoadoutButton.OnLeftClick += (evt, element) =>
        {
            AddNewLoadout();
        };
        mainPanel.Append(addLoadoutButton);

        backButton = new UITextPanel<string>("Back");
        backButton.Width.Set(80f, 0f);
        backButton.Height.Set(40f, 0f);  // Match UIFolderEntry button height
        backButton.Top.Set(40f, 0f);
        backButton.Left.Set(-100f, 1f);
        backButton.WithFadedMouseOver();
        backButton.OnLeftClick += (evt, element) =>
        {
            var loadoutsSystem = ModContent.GetInstance<BossLoadoutsSystem>();
            loadoutsSystem.ShowUI("loadouts");
        };
        mainPanel.Append(backButton);

        scrollbar = new UIScrollbar();
        scrollbar.HAlign = 0.95f;
        scrollbar.Top.Set(90f, 0f);
        scrollbar.Height.Set(-90, 1f);   // Match BossLoadoutsUI scrollbar height 45%
        mainPanel.Append(scrollbar);

        loadoutsList = new UIList();
        loadoutsList.Width.Set(0, 0.9f);  // Leave room for scrollbar & padding
        loadoutsList.Height.Set(-90f, 1f);
        loadoutsList.Top.Set(90f, 0f);
        loadoutsList.ListPadding = 8f;  // Match spacing in UIFolderEntry
        loadoutsList.ManualSortMethod = (e) => { };
        loadoutsList.SetScrollbar(scrollbar);
        mainPanel.Append(loadoutsList);

        RefreshLoadouts();
    }

    public void RefreshLoadouts()
    {
        loadoutsList.Clear();

        if (currentFolder == null)
            return;

        foreach (var loadout in currentFolder.Loadouts)
        {
            var loadoutEntry = CreateLoadoutEntry(loadout);
            loadoutsList.Add(loadoutEntry);
        }
    }

    private UIPanel CreateLoadoutEntry(Loadout loadout)
    {
        var panel = new UIPanel();
        panel.Width.Set(0, 1f);
        panel.Height.Set(85f, 0f);
        panel.SetPadding(10f);
        panel.BackgroundColor = new Color(30, 30, 60);

        var loadoutName = new UIText(loadout.Name)
        {
            HAlign = 0.5f,
            Top = new StyleDimension(5f, 0f),
            TextColor = Color.White
        };
        panel.Append(loadoutName);

        float spacing = 8f;
        float buttonHeight = 40f;  // Match UIFolderEntry button height
        float buttonPadding = 8f;  // Match UIFolderEntry padding
        float startTop = 35f;      // Match UIFolderEntry startTop
        float currentLeft = 0f;

        var buttonInfos = new (string Text, Action Action, bool IsSquare)[]
        {
            ("Equip", () => loadout.LoadGear(Main.player[Main.myPlayer]), false),
            ("Rename", () =>
            {
                var system = ModContent.GetInstance<BossLoadoutsSystem>();
                system.RenameLoadoutUI = new RenameLoadoutUI();
                system.RenameLoadoutUI.loadoutToRename = loadout;
                system.ShowUI("renameloadout");
            }, false),
            ("Update", () =>
            {
                loadout.SaveGear(Main.player[Main.myPlayer]);
                SaveLoadSystem.SaveGlobalData();
                Main.NewText($"Successfully updated loadout {loadout.Name}.", Color.Green);
            }, false),
            ("Delete", () =>
            {
                var system = ModContent.GetInstance<BossLoadoutsSystem>();
                system._loadoutsInterface?.SetState(new ConfirmDeleteLoadoutUI(loadout, currentFolder));
            }, false),
            ("↑", () => MoveLoadout(loadout, -1), true),
            ("↓", () => MoveLoadout(loadout, 1), true)
        };

        UIElement buttonRow = new UIElement
        {
            Top = new StyleDimension(startTop, 0f),
            Left = new StyleDimension(0f, 0f),
            Width = new StyleDimension(0f, 1f),
            Height = new StyleDimension(buttonHeight, 0f)
        };
        panel.Append(buttonRow);

        foreach (var (text, action, isSquare) in buttonInfos)
        {
            float width = isSquare ? buttonHeight : CalculateButtonWidth(text);

            var button = new UITextPanel<string>(text, 0.8f, large: false)
            {
                PaddingTop = buttonPadding,
                PaddingBottom = buttonPadding,
                PaddingLeft = buttonPadding,
                PaddingRight = buttonPadding,
                Height = { Pixels = buttonHeight },
                Width = { Pixels = width }
            };

            button.WithFadedMouseOver();
            button.OnLeftClick += (evt, element) => action();

            button.Left.Set(currentLeft, 0f);
            button.Top.Set(0f, 0f);
            buttonRow.Append(button);

            currentLeft += button.Width.Pixels + spacing;
        }

        return panel;
    }

    private float CalculateButtonWidth(string text)
    {
        // Approx width similar to UIFolderEntry
        return text.Length * 8f + 20f;
    }

    private void MoveLoadout(Loadout loadout, int direction)
    {
        int index = currentFolder.Loadouts.IndexOf(loadout);
        int newIndex = index + direction;

        if (newIndex >= 0 && newIndex < currentFolder.Loadouts.Count)
        {
            // Swap positions
            var temp = currentFolder.Loadouts[newIndex];
            currentFolder.Loadouts[newIndex] = loadout;
            currentFolder.Loadouts[index] = temp;

            SaveLoadSystem.SaveGlobalData();
            RefreshLoadouts();
        }
    }

    private void AddNewLoadout()
    {
        if (currentFolder == null)
            return;

        string newLoadoutName = $"New Loadout {currentFolder.Loadouts.Count + 1}";
        var newLoadout = new Loadout { Name = newLoadoutName };
        newLoadout.SaveGear(Main.player[Main.myPlayer]);
        currentFolder.Loadouts.Add(newLoadout);
        SaveLoadSystem.SaveGlobalData();
        RefreshLoadouts();
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);
        if (mainPanel.ContainsPoint(Main.MouseScreen))
        {
            Main.LocalPlayer.mouseInterface = true;
            PlayerInput.LockVanillaMouseScroll("uiMSL");
        }
    }
}
