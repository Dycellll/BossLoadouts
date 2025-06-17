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
        internal ConfirmDeleteUI ConfirmDeleteUI;
        internal FolderLoadoutsUI FolderLoadoutsUI;
        internal RenameLoadoutUI RenameLoadoutUI;
        public UserInterface _loadoutsInterface;

        public override void Load()
        {
            if (!Main.dedServ)
            {
                LoadoutsUI = new BossLoadoutsUI();
                CreateFolderUI = new CreateFolderUI();
                RenameFolderUI = new RenameFolderUI();
                FolderLoadoutsUI = new FolderLoadoutsUI();
                ConfirmDeleteUI = new ConfirmDeleteUI();
                RenameLoadoutUI = new RenameLoadoutUI();
                _loadoutsInterface = new UserInterface();
                _loadoutsInterface.SetState(null);
            }
            Main.blockInput = false;
        }

        public void ShowUI(string type)
        {
            if (type.ToLower() == "loadouts")
            {
                _loadoutsInterface?.SetState(LoadoutsUI);
            }
            else if (type.ToLower() == "renamefolder")
            {
                _loadoutsInterface?.SetState(RenameFolderUI ??= new RenameFolderUI());
            }
            else if(type.ToLower() == "confirmdelete")
            {
                _loadoutsInterface?.SetState(ConfirmDeleteUI ??= new ConfirmDeleteUI());
            }
            else if (type.ToLower() == "renameloadout")
            {
                _loadoutsInterface?.SetState(RenameLoadoutUI ??= new RenameLoadoutUI());
            }
            else
            {
                _loadoutsInterface?.SetState(CreateFolderUI ??= new CreateFolderUI());
            }
        }

        public void ShowLoadoutsUIForFolder(Folder folder)
        {
            FolderLoadoutsUI.SetFolder(folder);
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