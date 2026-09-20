using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BossLoadouts.Systems;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.UI;
using static BossLoadouts.Content.UI.PermanentBuffsEditorUI;

namespace BossLoadouts.Content.UI
{
    public class BossDownedEditorUI : UIState
    {
        public interface IBossProvider
        {
            IEnumerable<BossEntry> GetBossEntries();
        }

        public class BossEntry
        {
            public string Name;
            public Func<bool> IsDowned;
            public Action<bool> SetDowned;
            public int SortIndex;
        }


        List<UITextPanel<string>> bossButtons = new List<UITextPanel<string>>();
        List<int> bossButtonSortIndices = new List<int>();
        bool[] downedBosses;
        
        void SetBools()
        {
            int maxSortIndex = 0;
            foreach (IBossProvider provider in BossLoadouts.BossProviders)
            {
                foreach (var entry in provider.GetBossEntries())
                {
                    if (entry.SortIndex > maxSortIndex)
                        maxSortIndex = entry.SortIndex;
                }
            }

            downedBosses = new bool[maxSortIndex + 1];

            foreach (IBossProvider provider in BossLoadouts.BossProviders)
            {
                foreach (var entry in provider.GetBossEntries())
                {
                    if (entry.SortIndex < 0 || entry.SortIndex >= downedBosses.Length)
                        continue;

                    downedBosses[entry.SortIndex] = entry.IsDowned();
                }
            }
        }

        public static void SetAllDowned(bool[] downed)
        {
            foreach (IBossProvider provider in BossLoadouts.BossProviders)
            {
                foreach (var entry in provider.GetBossEntries())
                {
                    if (entry.SortIndex < 0 || entry.SortIndex >= downed.Length)
                        continue;

                    entry.SetDowned(downed[entry.SortIndex]);
                }
            }
        }

        private UIPanel mainPanel;
        private UIHeader titleHeader;
        private UIList bossesList;
        private UITextPanel<string> backButton;
        public override void OnActivate()
        {
            base.OnActivate();
            SetBools();

            for (int i = 0; i < bossButtons.Count; i++)
            {
                int sortIndex = bossButtonSortIndices[i];

                if (sortIndex >= 0 && sortIndex < downedBosses.Length)
                {
                    UpdateButtonAppearance(bossButtons[i], downedBosses[sortIndex]);
                }
            }
        }

        private void UpdateButtonAppearance(UITextPanel<string> button, bool isDowned)
        {
            button.BackgroundColor = isDowned ? Color.Green : Color.Red;
            button.WithFadedMouseOver(isDowned ? Color.DarkGreen : Color.DarkRed,
                                    isDowned ? Color.Green : Color.Red);
        }
        public override void OnInitialize()
        {
            mainPanel = new UIPanel
            {
                Width = { Pixels = 500 },
                Height = { Pixels = 500 },
                HAlign = 0.5f,
                VAlign = 0.5f,
            };
            Append(mainPanel);

            titleHeader = new UIHeader("Downed Bosses Editor")
            {
                HAlign = 0.5f,
                VAlign = 0.05f,
            };
            mainPanel.Append(titleHeader);

            bossesList = new UIList
            {
                HAlign = 0.05f,
                VAlign = 0.5f,
                Width = { Percent = 0.9f },
                Height = { Percent = 0.6f },
                ListPadding = 5f,
                ManualSortMethod = (e) => { }
            };
            mainPanel.Append(bossesList);

            var bossesScrollbar = new UIScrollbar
            {
                HAlign = 0.95f,
                VAlign = 0.5f,
                Height = { Percent = 0.6f },
            };
            mainPanel.Append(bossesScrollbar);
            bossesList.SetScrollbar(bossesScrollbar);

            backButton = new UITextPanel<string>("Back")
            {
                HAlign = 0.5f,
                VAlign = 0.95f,
            };
            backButton.WithFadedMouseOver();
            backButton.OnLeftClick += (evt, element) =>
            {
                var loadoutsSystem = ModContent.GetInstance<BossLoadoutsSystem>();
                loadoutsSystem.ShowUI("loadouts");
            };
            mainPanel.Append(backButton);

            SetBools();

            List<BossEntry> sortedEntries = new List<BossEntry>();
            foreach (IBossProvider provider in BossLoadouts.BossProviders)
            {
                foreach (var entry in provider.GetBossEntries())
                {
                    int index = entry.SortIndex;
                    if (index < 0 || index >= downedBosses.Length)
                        continue;

                    bool inserted = false;

                    for (int i = 0; i < sortedEntries.Count; i++)
                    {
                        if (sortedEntries[i].SortIndex > entry.SortIndex)
                        {
                            sortedEntries.Insert(i, entry);
                            inserted = true;
                            break;
                        }
                    }

                    if (!inserted)
                    {
                        sortedEntries.Add(entry);
                    }
                }
            }

            foreach(var entry in sortedEntries)
            {
                DrawBossButton(entry.Name, downedBosses[entry.SortIndex], entry.SortIndex);
            }
        }

        void DrawBossButton(string name, bool enabled, int index)
        {
            var color = enabled ? Color.Green : Color.Red;
            var button = new UITextPanel<string>(name)
            {
                Width = { Percent = 1f },
                Height = { Pixels = 30 },
                BackgroundColor = color
            };

            if (!downedBosses[index])
            {
                    button.WithFadedMouseOver(Color.DarkRed, Color.Red);
            }
            else
            {
                button.WithFadedMouseOver(Color.DarkGreen, Color.Green);
            }

            button.OnLeftClick += (evt, element) =>
            {
                downedBosses[index] = !downedBosses[index];
                SetAllDowned(downedBosses);
                button.BackgroundColor = downedBosses[index] ? Color.Green : Color.Red;
                Main.NewText($"{name} is now marked as {(downedBosses[index] ? "downed" : "not downed")}.", downedBosses[index] ? Color.Green : Color.Red);
                button.WithFadedMouseOver(downedBosses[index] ? Color.DarkGreen : Color.DarkRed, downedBosses[index] ? Color.Green : Color.Red);
            };

            bossButtons.Add(button);
            bossButtonSortIndices.Add(index);
            bossesList.Add(button);
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
}