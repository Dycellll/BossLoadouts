using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BossLoadouts.Systems;
using CalamityMod;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace BossLoadouts.Content.UI
{
    class BossDownedEditorUI : UIState
    {
        List<UITextPanel<string>> bossButtons = new List<UITextPanel<string>>();
        bool[] downedBosses = new bool[45];
        public static readonly string[] bossNames = new string[]
        {
            "King Slime",                     // 0
            "Desert Scourge",                // 1
            "Eye of Cthulhu",                // 2
            "Crabulon",                      // 3
            "Eater of Worlds",              // 4
            "Brain of Cthulhu",             // 5
            "The Hive Mind",                // 6
            "The Perforators",              // 7
            "Queen Bee",                    // 8
            "Deerclops",                    // 9
            "Skeletron",                    //10
            "The Slime God",                //11
            "Wall of Flesh",            //12
            "Queen Slime",                  //13
            "Cryogen",                      //14
            "Skeletron Prime",                    //15
            "Aquatic Scourge",              //16
            "The Twins",                //17
            "Brimstone Elemental",          //18
            "The Destroyer",              //19
            "Calamitas Clone",              //20
            "Plantera",                     //21
            "Astrum Aureus",                //22
            "Leviathan and Anahita",        //23
            "Golem",                        //24
            "Duke Fishron",                 //25
            "Empress of Light",             //26
            "The Plaguebringer Goliath",    //27
            "Ravager",                      //28
            "Lunatic Cultist",              //29
            "Astrum Deus",                  //30
            "Moon Lord",                    //31
            "Profaned Guardians",           //32
            "Dragonfolly",                  //33
            "Providence",                   //34
            "Signus",                       //35
            "Storm Weaver",                     //36
            "Ceaseless Void",                   //37
            "Polterghast",                      //38
            "Old Duke",                         //39
            "The Devourer of Gods",             //40
            "Yharon",                           //41
            "Primordial Wyrm",                  //42
            "Exo Mechs",                        //43
            "Supreme Witch, Calamitas"         //44
        };

        void SetBools()
        {
            downedBosses[0] = NPC.downedSlimeKing;
            downedBosses[1] = DownedBossSystem.downedDesertScourge;
            downedBosses[2] = NPC.downedBoss1;
            downedBosses[3] = DownedBossSystem.downedCrabulon;
            downedBosses[4] = NPC.downedBoss2;
            downedBosses[5] = NPC.downedBoss2;
            downedBosses[6] = DownedBossSystem.downedHiveMind;
            downedBosses[7] = DownedBossSystem.downedPerforator;
            downedBosses[8] = NPC.downedQueenBee;
            downedBosses[9] = NPC.downedDeerclops;
            downedBosses[10] = NPC.downedBoss3;
            downedBosses[11] = DownedBossSystem.downedSlimeGod;
            downedBosses[12] = Main.hardMode;
            downedBosses[13] = NPC.downedQueenSlime;
            downedBosses[14] = DownedBossSystem.downedCryogen;
            downedBosses[15] = NPC.downedMechBoss3;
            downedBosses[16] = DownedBossSystem.downedAquaticScourge;
            downedBosses[17] = NPC.downedMechBoss2;
            downedBosses[18] = DownedBossSystem.downedBrimstoneElemental;
            downedBosses[19] = NPC.downedMechBoss1;
            downedBosses[20] = DownedBossSystem.downedCalamitasClone;
            downedBosses[21] = NPC.downedPlantBoss;
            downedBosses[22] = DownedBossSystem.downedAstrumAureus;
            downedBosses[23] = DownedBossSystem.downedLeviathan;
            downedBosses[24] = NPC.downedGolemBoss;
            downedBosses[25] = NPC.downedFishron;
            downedBosses[26] = NPC.downedEmpressOfLight;
            downedBosses[27] = DownedBossSystem.downedPlaguebringer;
            downedBosses[28] = DownedBossSystem.downedRavager;
            downedBosses[29] = NPC.downedAncientCultist;
            downedBosses[30] = DownedBossSystem.downedAstrumDeus;
            downedBosses[31] = NPC.downedMoonlord;
            downedBosses[32] = DownedBossSystem.downedGuardians;
            downedBosses[33] = DownedBossSystem.downedDragonfolly;
            downedBosses[34] = DownedBossSystem.downedProvidence;
            downedBosses[35] = DownedBossSystem.downedSignus;
            downedBosses[36] = DownedBossSystem.downedStormWeaver;
            downedBosses[37] = DownedBossSystem.downedCeaselessVoid;
            downedBosses[38] = DownedBossSystem.downedPolterghast;
            downedBosses[39] = DownedBossSystem.downedBoomerDuke;
            downedBosses[40] = DownedBossSystem.downedDoG;
            downedBosses[41] = DownedBossSystem.downedYharon;
            downedBosses[42] = DownedBossSystem.downedPrimordialWyrm;
            downedBosses[43] = DownedBossSystem.downedExoMechs;
            downedBosses[44] = DownedBossSystem.downedCalamitas;
        }

        public static void SetAllDowned(bool[] downed)
        {
            NPC.downedSlimeKing = downed[0];
            DownedBossSystem.downedDesertScourge = downed[1];
            NPC.downedBoss1 = downed[2];
            DownedBossSystem.downedCrabulon = downed[3];
            NPC.downedBoss2 = downed[4] || downed[5];
            DownedBossSystem.downedHiveMind = downed[6];
            DownedBossSystem.downedPerforator = downed[7];
            NPC.downedQueenBee = downed[8];
            NPC.downedDeerclops = downed[9];
            NPC.downedBoss3 = downed[10];
            DownedBossSystem.downedSlimeGod = downed[11];
            Main.hardMode = downed[12];
            NPC.downedQueenSlime = downed[13];
            DownedBossSystem.downedCryogen = downed[14];
            NPC.downedMechBoss3 = downed[15];
            DownedBossSystem.downedAquaticScourge = downed[16];
            NPC.downedMechBoss2 = downed[17];
            DownedBossSystem.downedBrimstoneElemental = downed[18];
            NPC.downedMechBoss1 = downed[19];
            DownedBossSystem.downedCalamitasClone = downed[20];
            NPC.downedPlantBoss = downed[21];
            DownedBossSystem.downedAstrumAureus = downed[22];
            DownedBossSystem.downedLeviathan = downed[23];
            NPC.downedGolemBoss = downed[24];
            NPC.downedFishron = downed[25];
            NPC.downedEmpressOfLight = downed[26];
            DownedBossSystem.downedPlaguebringer = downed[27];
            DownedBossSystem.downedRavager = downed[28];
            NPC.downedAncientCultist = downed[29];
            DownedBossSystem.downedAstrumDeus = downed[30];
            NPC.downedMoonlord = downed[31];
            DownedBossSystem.downedGuardians = downed[32];
            DownedBossSystem.downedDragonfolly = downed[33];
            DownedBossSystem.downedProvidence = downed[34];
            DownedBossSystem.downedSignus = downed[35];
            DownedBossSystem.downedStormWeaver = downed[36];
            DownedBossSystem.downedCeaselessVoid = downed[37];
            DownedBossSystem.downedPolterghast = downed[38];
            DownedBossSystem.downedBoomerDuke = downed[39];
            DownedBossSystem.downedDoG = downed[40];
            DownedBossSystem.downedYharon = downed[41];
            DownedBossSystem.downedPrimordialWyrm = downed[42];
            DownedBossSystem.downedExoMechs = downed[43];
            DownedBossSystem.downedCalamitas = downed[44];
        }

        private UIPanel mainPanel;
        private UIHeader titleHeader;
        private UIList bossesList;
        private UITextPanel<string> backButton;
        public override void OnActivate()
        {
            base.OnActivate();
            SetBools();

            if (bossButtons.Count != downedBosses.Length - 1)
                return;

            int buttonIndex = 0;
            for (int i = 0; i < downedBosses.Length; i++)
            {
                if (i == 5) continue;

                if (i == 4)
                {
                    bool evilBossDowned = downedBosses[4] || downedBosses[5];
                    UpdateButtonAppearance(bossButtons[buttonIndex], evilBossDowned);
                    buttonIndex++;
                    continue;
                }

                UpdateButtonAppearance(bossButtons[buttonIndex], downedBosses[i]);
                buttonIndex++;
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

            for (int i = 0; i < downedBosses.Length; i++)
            {
                if (i == 4)
                {
                    bool evilBossDowned = downedBosses[4] || downedBosses[5];
                    DrawBossButton("Evil Boss (EoW/BoC)", evilBossDowned, -1);
                    continue;
                }
                if (i == 5) continue;

                DrawBossButton(bossNames[i], downedBosses[i], i);
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

            if (index == -1)
            {
                button.WithFadedMouseOver(enabled ? Color.DarkGreen : Color.DarkRed, enabled ? Color.Green : Color.Red);
                button.OnLeftClick += (evt, element) =>
                {
                    bool newValue = !enabled;
                    downedBosses[4] = newValue;
                    downedBosses[5] = newValue;
                    SetAllDowned(downedBosses);

                    button.BackgroundColor = newValue ? Color.Green : Color.Red;
                    Main.NewText($"{name} is now marked as {(newValue ? "downed" : "not downed")}.", newValue ? Color.Green : Color.Red);
                    button.WithFadedMouseOver(newValue ? Color.DarkGreen : Color.DarkRed, newValue ? Color.Green : Color.Red);
                    enabled = newValue;
                };
            }
            else
            {
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
            }

            bossButtons.Add(button);
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
