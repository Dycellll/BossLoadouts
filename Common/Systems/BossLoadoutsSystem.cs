using System.Collections.Generic;
using BossLoadouts.Common.Systems;
using BossLoadouts.Content.UI;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace BossLoadouts.Systems
{
    public class BossLoadoutsSystem : ModSystem
    {
        internal BossLoadoutsUI LoadoutsUI;
        internal CreateFolderUI CreateFolderUI;
        internal RenameFolderUI RenameFolderUI;
        public FolderLoadoutsUI FolderLoadoutsUI;
        public RenameLoadoutUI RenameLoadoutUI;
        public BossDownedEditorUI DownedEditorUI;
        public PermanentBuffsEditorUI BuffsEditorUI;
        public UserInterface _loadoutsInterface;

        public override void Load()
        {
            if (!Main.dedServ)
            {
                LoadoutsUI = new BossLoadoutsUI();
                CreateFolderUI = new CreateFolderUI();
                RenameFolderUI = new RenameFolderUI();
                RenameLoadoutUI = new RenameLoadoutUI();
                DownedEditorUI = new BossDownedEditorUI();
                BuffsEditorUI = new PermanentBuffsEditorUI();
                _loadoutsInterface = new UserInterface();
            }
            Main.blockInput = false;
        }

        public void ShowUI(string type)
        {
            if (type.ToLower() == "loadouts")
            {
                float scroll = LoadoutsUI?.foldersList?.ViewPosition ?? 0f;

                _loadoutsInterface?.SetState(LoadoutsUI);
                LoadoutsUI.RefreshFolders();

                LoadoutsUI.foldersList.ViewPosition = scroll;
            }
            else if (type.ToLower() == "renamefolder")
            {
                _loadoutsInterface?.SetState(RenameFolderUI);
            }
            else if (type.ToLower() == "renameloadout")
            {
                _loadoutsInterface?.SetState(RenameLoadoutUI);
            }
            else if (type.ToLower() == "createfolder")
            {
                _loadoutsInterface?.SetState(CreateFolderUI);
            }
            else if (type.ToLower() == "folderloadouts")
            {
                _loadoutsInterface?.SetState(FolderLoadoutsUI);
            }
            else if (type.ToLower() == "downededitor")
            {
                _loadoutsInterface?.SetState(DownedEditorUI);
            }
            else if (type.ToLower() == "buffseditor")
            {
                _loadoutsInterface?.SetState(BuffsEditorUI);
            }
        }

        public void ShowLoadoutsUIForFolder(Folder folder)
        {
            FolderLoadoutsUI = new FolderLoadoutsUI(folder);
            _loadoutsInterface?.SetState(FolderLoadoutsUI);
        }

        public void HideUI()
        {
            _loadoutsInterface?.SetState(null);
        }

        public override void UpdateUI(GameTime gameTime)
        {
            if (_loadoutsInterface?.CurrentState != null)
            {
                _loadoutsInterface.Update(gameTime);
            }
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "BossLoadouts: Loadouts UI",
                    delegate {
                        if (_loadoutsInterface?.CurrentState != null)
                        {
                            _loadoutsInterface.Draw(Main.spriteBatch, new GameTime());
                        }
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }
    }
}