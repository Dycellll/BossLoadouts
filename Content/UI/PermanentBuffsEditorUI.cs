using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.UI;
using BossLoadouts.Systems;
using Terraria.ID;
using Terraria.Audio;
using BossLoadouts.Content;

namespace BossLoadouts.Content.UI
{
    public class PermanentBuffsEditorUI : UIState
    {
        public interface IPermanentBuffProvider
        {
            IEnumerable<PermanentBuffEntry> GetBuffEntries();
        }

        public class PermanentBuffEntry
        {
            public string Name;
            public Func<bool> IsUnlocked;
            public Action<bool> SetUnlocked;
            public int SortIndex;
        }
        public static List<Action<UIPanel>> OnInjectExtraButtons = new List<Action<UIPanel>>();
        List<UITextPanel<string>> buffButtons = new();
        List<int> buffButtonSortIndices = new();
        List<Action> statRefreshers = new();
        bool[] unlockedBuffs;

        void SetBools()
        {
            int maxSortIndex = 0;
            foreach (IPermanentBuffProvider provider in BossLoadouts.BuffProviders)
            {
                foreach (var entry in provider.GetBuffEntries())
                {
                    if (entry.SortIndex > maxSortIndex)
                        maxSortIndex = entry.SortIndex;
                }
            }

            unlockedBuffs = new bool[maxSortIndex + 1];

            foreach (IPermanentBuffProvider provider in BossLoadouts.BuffProviders)
            {
                foreach (var entry in provider.GetBuffEntries())
                {
                    if (entry.SortIndex < 0 || entry.SortIndex >= unlockedBuffs.Length)
                        continue;

                    unlockedBuffs[entry.SortIndex] = entry.IsUnlocked();
                }
            }
        }

        public static void SetAllUnlocked(bool[] unlocked)
        {
            foreach (IPermanentBuffProvider provider in BossLoadouts.BuffProviders)
            {
                foreach (var entry in provider.GetBuffEntries())
                {
                    if (entry.SortIndex < 0 || entry.SortIndex >= unlocked.Length)
                        continue;

                    entry.SetUnlocked(unlocked[entry.SortIndex]);
                }
            }
        }

        void DrawNumericStepper(string label, Func<int> getValue, Action<int> setValue, int min, int max)
        {
            var row = new UIElement
            {
                Width = { Percent = 1f },
                Height = { Pixels = 40 },
            };

            var valueText = new UIText("")
            {
                VAlign = 0.5f,
                HAlign = 0f,
            };

            void Refresh() => valueText.SetText($"{label}: {getValue()}");

            var minusButton = new UITextPanel<string>("-")
            {
                Width = { Pixels = 30 },
                Height = { Pixels = 30 },
                HAlign = 1f,
            };
            minusButton.Left.Set(-65f, 0f);
            minusButton.WithFadedMouseOver();
            minusButton.OnLeftClick += (evt, element) =>
            {
                int newVal = Math.Clamp(getValue() - 1, min, max);
                setValue(newVal);
                Refresh();
                SoundEngine.PlaySound(SoundID.MenuTick);
            };

            var plusButton = new UITextPanel<string>("+")
            {
                Width = { Pixels = 30 },
                Height = { Pixels = 30 },
                HAlign = 1f,
            };
            plusButton.Left.Set(0f, 0f);
            plusButton.WithFadedMouseOver();
            plusButton.OnLeftClick += (evt, element) =>
            {
                int newVal = Math.Clamp(getValue() + 1, min, max);
                setValue(newVal);
                Refresh();
                SoundEngine.PlaySound(SoundID.MenuTick);
            };

            Refresh();
            statRefreshers.Add(Refresh);

            row.Append(valueText);
            row.Append(minusButton);
            row.Append(plusButton);
            buffsList.Add(row);
        }

        private UIPanel mainPanel;
        private UIHeader titleHeader;
        private UIList buffsList;
        private UITextPanel<string> backButton;

        public override void OnActivate()
        {
            base.OnActivate();
            SetBools();
            foreach (var refresh in statRefreshers) refresh();

            for (int i = 0; i < buffButtons.Count; i++)
            {
                int sortIndex = buffButtonSortIndices[i];
                if (sortIndex >= 0 && sortIndex < unlockedBuffs.Length)
                    UpdateButtonAppearance(buffButtons[i], unlockedBuffs[sortIndex]);
            }
        }

        private void UpdateButtonAppearance(UITextPanel<string> button, bool isUnlocked)
        {
            button.BackgroundColor = isUnlocked ? Color.Green : Color.Red;
            button.WithFadedMouseOver(
                isUnlocked ? Color.DarkGreen : Color.DarkRed,
                isUnlocked ? Color.Green : Color.Red);
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

            titleHeader = new UIHeader("Permanent Buffs Editor")
            {
                HAlign = 0.5f,
                VAlign = 0.05f,
            };
            mainPanel.Append(titleHeader);

            buffsList = new UIList
            {
                HAlign = 0.05f,
                VAlign = 0.6f,
                Width = { Percent = 0.9f },
                Height = { Percent = 0.55f },
                ListPadding = 5f,
                ManualSortMethod = (e) => { }
            };
            mainPanel.Append(buffsList);

            var buffsScrollbar = new UIScrollbar
            {
                HAlign = 0.95f,
                VAlign = 0.6f,
                Height = { Percent = 0.55f },
            };
            mainPanel.Append(buffsScrollbar);
            buffsList.SetScrollbar(buffsScrollbar);

            backButton = new UITextPanel<string>("Back")
            {
                HAlign = 0.5f,
                VAlign = 0.9f,
            };
            backButton.WithFadedMouseOver();
            backButton.OnLeftClick += (evt, element) =>
            {
                var loadoutsSystem = ModContent.GetInstance<BossLoadoutsSystem>();
                loadoutsSystem.ShowUI("loadouts");
            };
            mainPanel.Append(backButton);

            foreach (var inject in OnInjectExtraButtons)
            {
                inject(mainPanel);
            }

            SetBools();

            buffButtons.Clear();
            buffButtonSortIndices.Clear();
            buffsList.Clear();

            statRefreshers.Clear();

            DrawNumericStepper(
                "Life Crystals",
                () =>
                {
                    var mp = Main.LocalPlayer.GetModPlayer<MyPlayer>();
                    int level = mp.NegativeLifeCrystals > 0 ? -mp.NegativeLifeCrystals : Main.LocalPlayer.ConsumedLifeCrystals;
                    return level + 5;
                },
                newDisplay =>
                {
                    int newLevel = newDisplay - 5;
                    var mp = Main.LocalPlayer.GetModPlayer<MyPlayer>();
                    if (newLevel >= 0)
                    {
                        Main.LocalPlayer.ConsumedLifeCrystals = newLevel;
                        mp.NegativeLifeCrystals = 0;
                    }
                    else
                    {
                        Main.LocalPlayer.ConsumedLifeCrystals = 0;
                        mp.NegativeLifeCrystals = -newLevel;
                    }
                },
                1, 20);

            DrawNumericStepper(
                "Life Fruit",
                () => Main.LocalPlayer.ConsumedLifeFruit,
                newVal => Main.LocalPlayer.ConsumedLifeFruit = newVal,
                0, Player.LifeFruitMax);

            DrawNumericStepper(
                "Mana Crystals",
                () => Main.LocalPlayer.ConsumedManaCrystals,
                newVal => Main.LocalPlayer.ConsumedManaCrystals = newVal,
                0, Player.ManaCrystalMax);

            List<PermanentBuffEntry> sortedEntries = new List<PermanentBuffEntry>();

            foreach (IPermanentBuffProvider provider in BossLoadouts.BuffProviders)
            {
                foreach (var entry in provider.GetBuffEntries())
                {
                    if (entry.SortIndex < 0 || entry.SortIndex >= unlockedBuffs.Length)
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

            foreach (var entry in sortedEntries)
            {
                DrawBuffButton(entry.Name, unlockedBuffs[entry.SortIndex], entry.SortIndex);
            }

        }

        void DrawBuffButton(string name, bool enabled, int index)
        {
            var button = new UITextPanel<string>(name)
            {
                Width = { Percent = 1f },
                Height = { Pixels = 30 },
                BackgroundColor = enabled ? Color.Green : Color.Red
            };

            button.WithFadedMouseOver(
                enabled ? Color.DarkGreen : Color.DarkRed,
                enabled ? Color.Green : Color.Red);

            button.OnLeftClick += (evt, element) =>
            {
                unlockedBuffs[index] = !unlockedBuffs[index];
                SetAllUnlocked(unlockedBuffs);

                bool newVal = unlockedBuffs[index];
                button.BackgroundColor = newVal ? Color.Green : Color.Red;
                Main.NewText($"{name} is now {(newVal ? "unlocked" : "locked")}.",
                             newVal ? Color.Green : Color.Red);
                button.WithFadedMouseOver(
                    newVal ? Color.DarkGreen : Color.DarkRed,
                    newVal ? Color.Green : Color.Red);
            };

            buffButtons.Add(button);
            buffButtonSortIndices.Add(index);
            buffsList.Add(button);
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
