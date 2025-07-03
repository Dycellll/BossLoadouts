using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BossLoadouts.Common.Systems;
using CalamityMod;
using Terraria.ModLoader;

namespace BossLoadouts
{
	public class BossLoadouts : Mod
	{
        public override void Load()
        {
            ModLoader.TryGetMod("CalamityMod", out ModCompatibility.calamityMod);
            ModLoader.TryGetMod("FargowiltasSouls", out ModCompatibility.fargoMod);
        }
        public override void Unload()
        {
            ModCompatibility.calamityMod = null;
            ModCompatibility.fargoMod = null;
        }
    }
}
