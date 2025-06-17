using System.Collections.Generic;
using BossLoadouts.UI;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace BossLoadouts.Systems
{
    public class BossLoadoutsSystem : ModSystem
    {
        internal BossLoadoutsUI LoadoutsUI;
        public UserInterface _loadoutsInterface;

        public override void Load()
        {
            if (!Main.dedServ)
            {
                LoadoutsUI = new BossLoadoutsUI();
                _loadoutsInterface = new UserInterface();
                _loadoutsInterface.SetState(null);
            }
        }

        public void ShowUI()
        {
            _loadoutsInterface?.SetState(LoadoutsUI);
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