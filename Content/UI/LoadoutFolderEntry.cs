using Microsoft.Xna.Framework;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace BossLoadouts.UI
{
    public class UIFolderEntry : UIPanel
    {
        public UIText FolderName;
        public UITextPanel<string> OpenButton;
        public UITextPanel<string> RenameButton;
        public UITextPanel<string> DeleteButton;

        public UIFolderEntry(string name)
        {
            Width = new StyleDimension(0f, 1f);
            Height = new StyleDimension(50f, 0f);
            SetPadding(6f);
            BackgroundColor = new Color(30, 30, 60); 

            FolderName = new UIText(name);
            FolderName.Left.Set(10f, 0f);
            FolderName.Top.Set(15f, 0f);
            Append(FolderName);

            OpenButton = new UITextPanel<string>("Open");
            OpenButton.Width.Set(80f, 0f);
            OpenButton.Height.Set(30f, 0f);
            OpenButton.Left.Set(-270f, 1f);
            OpenButton.WithFadedMouseOver();
            Append(OpenButton);

            RenameButton = new UITextPanel<string>("Rename");
            RenameButton.Width.Set(80f, 0f);
            RenameButton.Height.Set(30f, 0f);
            RenameButton.Left.Set(-180f, 1f);
            RenameButton.WithFadedMouseOver();
            Append(RenameButton);

            DeleteButton = new UITextPanel<string>("Delete");
            DeleteButton.Width.Set(80f, 0f);
            DeleteButton.Height.Set(30f, 0f);
            DeleteButton.Left.Set(-90f, 1f);
            DeleteButton.WithFadedMouseOver();
            Append(DeleteButton);
        }
    }
}