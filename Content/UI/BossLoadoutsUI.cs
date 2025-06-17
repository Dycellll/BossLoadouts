using BossLoadouts.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace BossLoadouts.UI
{
    public class BossLoadoutsUI : UIState
    {
        public UIPanel MainPanel { get; private set; }
        public UIList LoadoutList { get; private set; }
        public UITextPanel<string> NewLoadoutButton { get; private set; }
        public UITextPanel<string> CloseButton { get; private set; }

        public override void OnInitialize()
        {
            MainPanel = new UIPanel();
            MainPanel.Width.Set(600f, 0f);
            MainPanel.Height.Set(400f, 0f);
            MainPanel.HAlign = 0.5f;
            MainPanel.VAlign = 0.5f;
            MainPanel.SetPadding(12f);
            Append(MainPanel);

            var title = new UIText("Boss Loadouts", 1.5f);
            title.HAlign = 0.5f;
            title.Top.Set(15f, 0f);
            MainPanel.Append(title);

            LoadoutList = new UIList();
            LoadoutList.Width.Set(-25f, 1f);
            LoadoutList.Height.Set(-100f, 1f);
            LoadoutList.Top.Set(20f, 0f);
            LoadoutList.ListPadding = 5f;
            MainPanel.Append(LoadoutList);

            var scrollbar = new UIScrollbar();
            scrollbar.Height.Set(0f, .7f);
            scrollbar.Top.Set(50f, 0f);
            scrollbar.HAlign = 1f;
            MainPanel.Append(scrollbar);
            LoadoutList.SetScrollbar(scrollbar);

            NewLoadoutButton = new UITextPanel<string>("Save Current Loadout");
            NewLoadoutButton.Width.Set(-10f, 0.5f);
            NewLoadoutButton.Height.Set(30f, 0f);
            NewLoadoutButton.VAlign = 1f;
            NewLoadoutButton.WithFadedMouseOver();
            MainPanel.Append(NewLoadoutButton);

            CloseButton = new UITextPanel<string>("Close");
            CloseButton.Width.Set(-10f, 0.5f);
            CloseButton.Height.Set(30f, 0f);
            CloseButton.VAlign = 1f;
            CloseButton.HAlign = 1f;
            CloseButton.WithFadedMouseOver();
            CloseButton.OnLeftClick += (evt, element) =>
            {
                var loadoutsSystem = ModContent.GetInstance<BossLoadoutsSystem>();
                loadoutsSystem.HideUI();
            };
            MainPanel.Append(CloseButton);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            if (MainPanel.ContainsPoint(Main.MouseScreen))
            {
                Main.LocalPlayer.mouseInterface = true;
                PlayerInput.LockVanillaMouseScroll("uiMSL");
            }
        }
    }
}