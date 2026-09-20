using System.Reflection;
using Terraria;
using Terraria.GameContent.UI.ResourceSets;
using Terraria.ModLoader;

namespace BossLoadouts.Common.Systems
{
    public class HeartDisplaySystem : ModSystem
    {
        private delegate void OrigSnapshotCtor(ref PlayerStatsSnapshot self, Player player);
        private delegate void HookSnapshotCtor(OrigSnapshotCtor orig, ref PlayerStatsSnapshot self, Player player);

        public override void Load()
        {
            if (Main.dedServ)
                return;

            ConstructorInfo ctor = typeof(PlayerStatsSnapshot).GetConstructor(
                BindingFlags.Public | BindingFlags.Instance,
                null,
                new[] { typeof(Player) },
                null);

            if (ctor == null)
            {
                Mod.Logger.Warn("Could not find PlayerStatsSnapshot(Player)..? heart display fix disabled.");
                return;
            }

            MonoModHooks.Add(ctor, (HookSnapshotCtor)CorrectHeartCount);
        }

        private static void CorrectHeartCount(OrigSnapshotCtor orig, ref PlayerStatsSnapshot self, Player player)
        {
            orig(ref self, player);

            if (player.ConsumedLifeFruit <= 0 || player.ConsumedLifeCrystals >= Player.LifeCrystalMax)
                return;

            var mp = player.GetModPlayer<Content.MyPlayer>();

            int hearts = 5 + player.ConsumedLifeCrystals - mp.NegativeLifeCrystals;

            self.AmountOfLifeHearts = Utils.Clamp(hearts, 1, 20);
        }
    }
}