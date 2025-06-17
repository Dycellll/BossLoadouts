using Microsoft.Xna.Framework;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace BossLoadouts.UI
{
    public class UILoadoutEntry : UIPanel
    {
        public UIText LoadoutName;
        public UIText BossName;
        public UITextPanel<string> LoadButton;
        public UITextPanel<string> DeleteButton;

        public UILoadoutEntry(string name, string boss)
        {
            Width = new StyleDimension(0f, 1f);
            Height = new StyleDimension(50f, 0f);
            SetPadding(6f);

            LoadoutName = new UIText(name);
            LoadoutName.Left.Set(10f, 0f);
            LoadoutName.Top.Set(5f, 0f);
            Append(LoadoutName);

            BossName = new UIText($"For: {boss}");
            BossName.Left.Set(10f, 0f);
            BossName.Top.Set(25f, 0f);
            BossName.TextColor = Color.LightGray;
            Append(BossName);

            LoadButton = new UITextPanel<string>("Load");
            LoadButton.Width.Set(80f, 0f);
            LoadButton.Height.Set(30f, 0f);
            LoadButton.Left.Set(-180f, 1f);
            LoadButton.Top.Set(10f, 0f);
            LoadButton.WithFadedMouseOver();
            Append(LoadButton);

            DeleteButton = new UITextPanel<string>("Delete");
            DeleteButton.Width.Set(80f, 0f);
            DeleteButton.Height.Set(30f, 0f);
            DeleteButton.Left.Set(-90f, 1f);
            DeleteButton.Top.Set(10f, 0f);
            DeleteButton.WithFadedMouseOver();
            Append(DeleteButton);
        }
    }
}