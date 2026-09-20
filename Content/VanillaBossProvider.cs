using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BossLoadouts.Content.UI;
using Terraria;

namespace BossLoadouts.Content
{
    public class VanillaBossProvider : BossDownedEditorUI.IBossProvider
    {
        public IEnumerable<BossDownedEditorUI.BossEntry> GetBossEntries()
        {
            return new List<BossDownedEditorUI.BossEntry>
        {
            new() { Name = "King Slime", SortIndex = 5, IsDowned = () => 
                        NPC.downedSlimeKing, SetDowned = val => 
                        NPC.downedSlimeKing = val },
            new() { Name = "Eye of Cthulhu", SortIndex = 20, IsDowned = () => 
                        NPC.downedBoss1, SetDowned = val => 
                        NPC.downedBoss1 = val },
            new() { Name = "Evil Boss (Eow/BoC)", SortIndex = 40, IsDowned = () => 
                        NPC.downedBoss2, SetDowned = val => 
                        NPC.downedBoss2 = val },
            new() { Name = "Queen Bee", SortIndex = 60, IsDowned = () =>
                        NPC.downedQueenBee, SetDowned = val =>
                        NPC.downedQueenBee = val },
            new() { Name = "Deerclops", SortIndex = 80, IsDowned = () =>
                        NPC.downedDeerclops, SetDowned = val =>
                        NPC.downedDeerclops = val },
            new() { Name = "Skeletron", SortIndex = 100, IsDowned = () =>
                        NPC.downedBoss3, SetDowned = val =>
                        NPC.downedBoss3 = val },
            new() { Name = "Wall of Flesh", SortIndex = 120, IsDowned = () =>
                        Main.hardMode, SetDowned = val =>
                        Main.hardMode = val },
            new() { Name = "Queen Slime", SortIndex = 140, IsDowned = () =>
                        NPC.downedQueenSlime, SetDowned = val =>
                        NPC.downedQueenSlime = val },
            new() { Name = "Destroyer", SortIndex = 160, IsDowned = () =>
                        NPC.downedMechBoss1, SetDowned = val =>
                        NPC.downedMechBoss1 = val },
            new() { Name = "Twins", SortIndex = 180, IsDowned = () =>
                        NPC.downedMechBoss2, SetDowned = val =>
                        NPC.downedMechBoss2 = val },
            new() { Name = "Skeletron Prime", SortIndex = 200, IsDowned = () =>
                        NPC.downedMechBoss3, SetDowned = val =>
                        NPC.downedMechBoss3 = val },
            new() { Name = "Plantera", SortIndex = 220, IsDowned = () =>
                        NPC.downedPlantBoss, SetDowned = val =>
                        NPC.downedPlantBoss = val },
            new() { Name = "Golem", SortIndex = 240, IsDowned = () =>
                        NPC.downedGolemBoss, SetDowned = val =>
                        NPC.downedGolemBoss = val },
            new() { Name = "Duke Fishron", SortIndex = 260, IsDowned = () =>
                        NPC.downedFishron, SetDowned = val =>
                        NPC.downedFishron = val },
            new() { Name = "Empress of Light", SortIndex = 280, IsDowned = () =>
                        NPC.downedEmpressOfLight, SetDowned = val =>
                        NPC.downedEmpressOfLight = val },
            new() { Name = "Lunatic Cultist", SortIndex = 300, IsDowned = () =>
                        NPC.downedAncientCultist, SetDowned = val =>
                        NPC.downedAncientCultist = val },
            new() { Name = "Moonlord", SortIndex = 320, IsDowned = () =>
                        NPC.downedMoonlord, SetDowned = val =>
                        NPC.downedMoonlord = val },
        };
        }
    }
}
