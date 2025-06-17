using BossLoadouts.Common.Systems;
using BossLoadouts.Systems;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.UI;

class ConfirmDeleteUI : UIState
{
    private UIPanel panel;
    private UIText message;
    private UITextPanel<string> yesButton;
    private UITextPanel<string> noButton;
    private Folder folderToDelete;

    public override void OnActivate()
    {
        panel = new UIPanel
        {
            HAlign = 0.5f,
            VAlign = 0.5f,
            Width = { Pixels = 300 },
            Height = { Pixels = 150 }
        };
        Append(panel);

        message = new UIText("Are you sure you want to delete this folder?")
        {
            HAlign = 0.5f,
            VAlign = 0.3f,
        };
        panel.Append(message);

        yesButton = new UITextPanel<string>("Yes")
        {
            Width = { Pixels = 100 },
            Height = { Pixels = 40 },
            HAlign = 0.25f,
            VAlign = 0.7f,
        };
        yesButton.WithFadedMouseOver();
        yesButton.OnLeftClick += (evt, element) =>
        {
            FoldersManager.Folders.Remove(folderToDelete);
            var loadoutsSystem = ModContent.GetInstance<BossLoadoutsSystem>();
            loadoutsSystem.LoadoutsUI.RefreshFolders();
            loadoutsSystem.HideUI();
        };
        panel.Append(yesButton);

        noButton = new UITextPanel<string>("No")
        {
            Width = { Pixels = 100 },
            Height = { Pixels = 40 },
            HAlign = 0.75f,
            VAlign = 0.7f,
        };
        noButton.WithFadedMouseOver();
        noButton.OnLeftClick += (evt, element) =>
        {
            var loadoutsSystem = ModContent.GetInstance<BossLoadoutsSystem>();
            loadoutsSystem.HideUI();
        };
        panel.Append(noButton);
    }

    public void SetFolder(Folder folder)
    {
        folderToDelete = folder;
    }
}
