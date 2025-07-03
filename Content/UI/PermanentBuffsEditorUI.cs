using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BossLoadouts.Systems;
using CalamityMod;
using CalamityMod.Buffs.Alcohol;
using CalamityMod.Items.PermanentBoosters;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace BossLoadouts.Content.UI
{
    class PermanentBuffsEditorUI : UIState
    {
        List<UITextPanel<string>> buffButtons = new List<UITextPanel<string>>();
        public UITextPanel<string> shroomedToggleButton;
        UITextPanel<string> shroomedLevelDisplay;
        UITextPanel<string> shroomedLevelUpButton;
        UITextPanel<string> shroomedLevelDownButton;
        bool[] unlockedBuffs = new bool[15];
        public static readonly string[] buffNames = new string[]
        {
            "Mushroom Plasma Root", //0
            "Electrolyte Gel Pack", //1
            "Demon Heart", //2
            "Comet Shard", //3
            "Blood Orange", //4
            "Miracle Fruit", //5
            "Starlight Fuel Cell", //6
            "Infernal Blood", //7
            "Ethereal Core", //8
            "Celestial Onion", //9
            "Red Lightning Container", //10
            "Elderberry", //11
            "Phantom Heart", //12
            "Ectoheart", //13
            "Dragonfruit" //14
        };

        void SetBools()
        {
            Player player = Main.player[Main.myPlayer];
            var modPlayer = player.Calamity();
            unlockedBuffs[0] = modPlayer.rageBoostOne;
            unlockedBuffs[1] = modPlayer.adrenalineBoostOne;
            unlockedBuffs[2] = player.extraAccessory;
            unlockedBuffs[3] = modPlayer.cShard;
            unlockedBuffs[4] = modPlayer.bOrange;
            unlockedBuffs[5] = modPlayer.mFruit;
            unlockedBuffs[6] = modPlayer.adrenalineBoostTwo;
            unlockedBuffs[7] = modPlayer.rageBoostTwo;
            unlockedBuffs[8] = modPlayer.eCore;
            unlockedBuffs[9] = modPlayer.extraAccessoryML;
            unlockedBuffs[10] = modPlayer.rageBoostThree;
            unlockedBuffs[11] = modPlayer.eBerry;
            unlockedBuffs[12] = modPlayer.pHeart;
            unlockedBuffs[13] = modPlayer.adrenalineBoostThree;
            unlockedBuffs[14] = modPlayer.dFruit;
        }

        public static void SetAllUnlocked(bool[] unlocked)
        {
            Player player = Main.player[Main.myPlayer];
            var modPlayer = player.Calamity();
            modPlayer.rageBoostOne = unlocked[0];
            modPlayer.adrenalineBoostOne = unlocked[1];
            if (unlocked[2])
            {
                player.extraAccessory = true;
                player.extraAccessorySlots = 1;
            }
            else
            {
                player.extraAccessory = false;
                player.extraAccessorySlots = 0;
            }
                modPlayer.cShard = unlocked[3];
            modPlayer.bOrange = unlocked[4];
            modPlayer.mFruit = unlocked[5];
            modPlayer.adrenalineBoostTwo = unlocked[6];
            modPlayer.rageBoostTwo = unlocked[7];
            modPlayer.eCore = unlocked[8];
            if (unlocked[9])
            {
                if (!modPlayer.extraAccessoryML)
                {
                    modPlayer.extraAccessoryML = true;
                    player.extraAccessorySlots += 1;
                }
            }
            else
            {
                modPlayer.extraAccessoryML = false;
                player.extraAccessorySlots = player.extraAccessorySlots == 0 ? 0 : player.extraAccessorySlots - 1;
            }
            modPlayer.rageBoostThree = unlocked[10];
            modPlayer.eBerry = unlocked[11];
            modPlayer.pHeart = unlocked[12];
            modPlayer.adrenalineBoostThree = unlocked[13];
            modPlayer.dFruit = unlocked[14];
        }

        private UIPanel mainPanel;
        private UIHeader titleHeader;
        private UIList buffsList;
        private UITextPanel<string> backButton;
        public override void OnActivate()
        {
            base.OnActivate();
            SetBools(); 
            UpdateShroomedUI();

            if (buffButtons.Count == unlockedBuffs.Length)
            {
                for (int i = 0; i < unlockedBuffs.Length; i++)
                    UpdateButtonAppearance(buffButtons[i], unlockedBuffs[i]);
            }
            else
            {
                ModContent.GetInstance<BossLoadouts>().Logger.Warn("Mismatch in buffButtons and unlockedBuffs length.");
            }
        }

        private void UpdateButtonAppearance(UITextPanel<string> button, bool isUnlocked)
        {
            button.BackgroundColor = isUnlocked ? Color.Green : Color.Red;
            button.WithFadedMouseOver(isUnlocked ? Color.DarkGreen : Color.DarkRed,
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

            shroomedToggleButton = new UITextPanel<string>("Shroomed: OFF")
            {
                HAlign = 0.05f,
                VAlign = 0.2f,
                Width = { Pixels = 250 },
                Height = { Pixels = 30 },
            };
            shroomedToggleButton.OnLeftClick += (evt, element) =>
            {
                Player player = Main.LocalPlayer;
                var modPlayer = player.Calamity();
                Terraria.Audio.SoundEngine.PlaySound(SoundID.MenuTick);

                if (modPlayer.trippy)
                {
                    SetShroomedLevel(0);
                }
                else
                {
                    SetShroomedLevel(1);
                }
                UpdateShroomedUI();
            };
            mainPanel.Append(shroomedToggleButton);

            shroomedLevelDisplay = new UITextPanel<string>("Level: OFF")
            {
                HAlign = 0.825f,
                VAlign = 0.2f,
                Width = { Pixels = 80 },
                Height = { Pixels = 30 },
                BackgroundColor = Color.Gray
            };
            mainPanel.Append(shroomedLevelDisplay);

            shroomedLevelDownButton = new UITextPanel<string>("▼")
            {
                HAlign = 0.65f,
                VAlign = 0.2f,
                Width = { Pixels = 25 },
                Height = { Pixels = 30 },
            };
            shroomedLevelDownButton.WithFadedMouseOver();
            shroomedLevelDownButton.OnLeftClick += (evt, element) =>
            {
                Player player = Main.LocalPlayer;
                var modPlayer = player.Calamity();

                if (modPlayer.trippyLevel > 1)
                {
                    Terraria.Audio.SoundEngine.PlaySound(SoundID.MenuTick);
                    SetShroomedLevel(modPlayer.trippyLevel - 1);
                    UpdateShroomedUI();
                }
            };
            mainPanel.Append(shroomedLevelDownButton);

            shroomedLevelUpButton = new UITextPanel<string>("▲")
            {
                HAlign = 0.95f,
                VAlign = 0.2f,
                Width = { Pixels = 25 },
                Height = { Pixels = 30 },
            };
            shroomedLevelUpButton.WithFadedMouseOver();
            shroomedLevelUpButton.OnLeftClick += (evt, element) =>
            {
                Player player = Main.LocalPlayer;
                var modPlayer = player.Calamity();

                if (modPlayer.trippyLevel < 3)
                {
                    Terraria.Audio.SoundEngine.PlaySound(SoundID.MenuTick);
                    SetShroomedLevel(modPlayer.trippyLevel + 1);
                    UpdateShroomedUI();
                }
            };
            mainPanel.Append(shroomedLevelUpButton);

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

            for (int i = 0; i < unlockedBuffs.Length; i++)
            {
                DrawBuffButton(buffNames[i], unlockedBuffs[i], i);
            }
        }

        private void SetShroomedLevel(int level)
        {
            Player player = Main.LocalPlayer;
            var modPlayer = player.Calamity();

            // Clamp level between 0 and 3
            level = Math.Max(0, Math.Min(3, level));
            modPlayer.trippyLevel = level;

            if (level > 0)
            {
                // Ensure the buff is active
                if (!player.HasBuff<Trippy>())
                {
                    player.AddBuff(ModContent.BuffType<Trippy>(), int.MaxValue);
                }
                modPlayer.trippy = true;
            }
            else
            {
                // Remove the buff
                player.ClearBuff(ModContent.BuffType<Trippy>());
                modPlayer.trippy = false;
            }
        }

        private void UpdateShroomedUI()
        {
            Player player = Main.LocalPlayer;
            var modPlayer = player?.Calamity();
            if (modPlayer == null) return;

            bool isActive = modPlayer.trippyLevel > 0 && modPlayer.trippy;

            // Update toggle button
            if (shroomedToggleButton != null)
            {
                shroomedToggleButton.SetText(isActive ? "Shroomed: ON" : "Shroomed: OFF");
                shroomedToggleButton.BackgroundColor = isActive ? Color.Green : Color.Red;
                shroomedToggleButton.WithFadedMouseOver(
                    isActive ? Color.DarkGreen : Color.DarkRed,
                    isActive ? Color.Green : Color.Red);
            }

            // Update level display
            if (shroomedLevelDisplay != null)
            {
                shroomedLevelDisplay.SetText($"Level: {Math.Max(1, modPlayer.trippyLevel)}");
                shroomedLevelDisplay.BackgroundColor = isActive ? Color.LightGray : Color.Gray;
            }

            // Update level buttons
            if (shroomedLevelUpButton != null)
            {
                shroomedLevelUpButton.BackgroundColor = (isActive && modPlayer.trippyLevel < 3) ?
                    Color.LightBlue : Color.Gray;
                shroomedLevelUpButton.WithFadedMouseOver((isActive && modPlayer.trippyLevel < 3) ?
                    Color.LightBlue : Color.Gray,
                    (isActive && modPlayer.trippyLevel < 3) ? Color.LightBlue : Color.Gray);
            }

            if (shroomedLevelDownButton != null)
            {
                shroomedLevelDownButton.BackgroundColor = (isActive && modPlayer.trippyLevel > 1) ?
                    Color.LightBlue : Color.Gray;
                shroomedLevelDownButton.WithFadedMouseOver((isActive && modPlayer.trippyLevel > 1) ?
                    Color.LightBlue : Color.Gray,
                    (isActive && modPlayer.trippyLevel > 1) ? Color.LightBlue : Color.Gray);
            }
        }

        void DrawBuffButton(string name, bool enabled, int index)
        {
            var color = enabled ? Color.Green : Color.Red;
            var button = new UITextPanel<string>(name)
            {
                Width = { Percent = 1f },
                Height = { Pixels = 30 },
                BackgroundColor = color
            };

            if (!unlockedBuffs[index])
            {
                button.WithFadedMouseOver(Color.DarkRed, Color.Red);
            }
            else
            {
                button.WithFadedMouseOver(Color.DarkGreen, Color.Green);
            }

            button.OnLeftClick += (evt, element) =>
            {
                unlockedBuffs[index] = !unlockedBuffs[index];
                SetAllUnlocked(unlockedBuffs);
                button.BackgroundColor = unlockedBuffs[index] ? Color.Green : Color.Red;
                Main.NewText($"{name} is now marked as {(unlockedBuffs[index] ? "unlocked" : "not unlocked")}.", unlockedBuffs[index] ? Color.Green : Color.Red);
                button.WithFadedMouseOver(unlockedBuffs[index] ? Color.DarkGreen : Color.DarkRed, unlockedBuffs[index] ? Color.Green : Color.Red);
            };

            buffButtons.Add(button);
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
