using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;
using BossLoadouts.Common.Systems;
using Terraria.ModLoader.UI;
using Terraria;
using BossLoadouts.Systems; // Your Folder, Loadout classes

namespace BossLoadouts.Content.UI
{
    public class FolderLoadoutsUI : UIState
    {
        private UIPanel mainPanel;
        private UIText folderTitle;
        private UIList loadoutsList;
        private UIScrollbar scrollbar;

        private UITextPanel<string> backButton;
        private UITextPanel<string> addLoadoutButton;

        private Folder currentFolder;

        public override void OnInitialize()
        {
            // Main container panel
            mainPanel = new UIPanel();
            mainPanel.Width.Set(500f, 0f);
            mainPanel.Height.Set(400f, 0f);
            mainPanel.HAlign = 0.5f;
            mainPanel.VAlign = 0.5f;
            Append(mainPanel);

            // Folder title
            folderTitle = new UIText("Folder Name");
            folderTitle.HAlign = 0.5f;
            folderTitle.Top.Set(10f, 0f);
            mainPanel.Append(folderTitle);

            // Add loadout button
            addLoadoutButton = new UITextPanel<string>("Add Loadout");
            addLoadoutButton.Width.Set(120f, 0f);
            addLoadoutButton.Height.Set(40f, 0f);
            addLoadoutButton.Top.Set(40f, 0f);
            addLoadoutButton.Left.Set(20f, 0f);
            addLoadoutButton.WithFadedMouseOver();
            addLoadoutButton.OnLeftClick += (evt, element) =>
            {
                AddNewLoadout();
            };
            mainPanel.Append(addLoadoutButton);

            // Back button
            backButton = new UITextPanel<string>("Back");
            backButton.Width.Set(80f, 0f);
            backButton.Height.Set(40f, 0f);
            backButton.Top.Set(40f, 0f);
            backButton.Left.Set(-100f, 1f);
            backButton.WithFadedMouseOver();
            backButton.OnLeftClick += (evt, element) =>
            {
                var loadoutsSystem = ModContent.GetInstance<BossLoadoutsSystem>();
                loadoutsSystem.ShowUI("loadouts"); // Go back to folders UI
            };
            mainPanel.Append(backButton);

            // Scrollbar for loadouts list
            scrollbar = new UIScrollbar();
            scrollbar.SetView(100f, 1000f);
            scrollbar.Height.Set(-75f, 1f);
            scrollbar.Top.Set(75f, 0f);
            scrollbar.Left.Set(-20f, 1f);
            mainPanel.Append(scrollbar);

            // Loadouts list
            loadoutsList = new UIList();
            loadoutsList.Width.Set(-40f, 1f);
            loadoutsList.Height.Set(-80f, 1f);
            loadoutsList.Top.Set(75f, 0f);
            loadoutsList.ListPadding = 5f;
            loadoutsList.SetScrollbar(scrollbar);
            mainPanel.Append(loadoutsList);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (currentFolder == null)
            {
                return;
            }

            // Optionally add logic for keyboard/gamepad input here
        }

        public void SetFolder(Folder folder)
        {
            currentFolder = folder;
            folderTitle.SetText($"Loadouts in \"{folder.Name}\"");
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
            panel.Height.Set(50f, 0f);
            panel.BackgroundColor = new Color(30, 30, 60);

            // Loadout name text
            var loadoutName = new UIText(loadout.Name)
            {
                Left = { Pixels = 10 },
                Top = { Pixels = 15 },
                Width = { Pixels = 200 }
            };
            panel.Append(loadoutName);

            // Rename button
            var renameButton = new UITextPanel<string>("Rename")
            {
                Width = { Pixels = 80 },
                Height = { Pixels = 30 },
                Left = { Pixels = 220 },
                Top = { Pixels = 10 }
            };
            renameButton.WithFadedMouseOver();
            renameButton.OnLeftClick += (evt, element) =>
            {
                var loadoutsSystem = ModContent.GetInstance<BossLoadoutsSystem>();
                loadoutsSystem.ShowUI("renameloadout");
                loadoutsSystem.RenameLoadoutUI.SetLoadout(loadout);
            };
            panel.Append(renameButton);

            // Delete button
            var deleteButton = new UITextPanel<string>("Delete")
            {
                Width = { Pixels = 80 },
                Height = { Pixels = 30 },
                Left = { Pixels = 310 },
                Top = { Pixels = 10 }
            };
            deleteButton.WithFadedMouseOver();
            deleteButton.OnLeftClick += (evt, element) =>
            {
                currentFolder.Loadouts.Remove(loadout);
                RefreshLoadouts();
            };
            panel.Append(deleteButton);

            // Open button (optional, to equip/load the loadout)
            var openButton = new UITextPanel<string>("Open")
            {
                Width = { Pixels = 80 },
                Height = { Pixels = 30 },
                Left = { Pixels = 400 },
                Top = { Pixels = 10 }
            };
            openButton.WithFadedMouseOver();
            openButton.OnLeftClick += (evt, element) =>
            {
                // TODO: Implement logic to activate the loadout
                Main.NewText($"Opening loadout: {loadout.Name}");
            };
            panel.Append(openButton);

            return panel;
        }

        private void AddNewLoadout()
        {
            if (currentFolder == null)
                return;

            string newLoadoutName = $"New Loadout {currentFolder.Loadouts.Count + 1}";
            var newLoadout = new Loadout { Name = newLoadoutName };
            currentFolder.Loadouts.Add(newLoadout);
            RefreshLoadouts();
        }
    }
}
