using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BossLoadouts.Common.Systems;
using Terraria.ModLoader;
using BossLoadouts.Content.UI;
using Microsoft.Xna.Framework;

namespace BossLoadouts
{
	public class BossLoadouts : Mod
	{
        public static List<BossDownedEditorUI.IBossProvider> BossProviders = new();
        public static List<PermanentBuffsEditorUI.IPermanentBuffProvider> BuffProviders = new();

        public override object Call(params object[] args)
        {
            if (args[0] is "RegisterBossProvider" && args[1] is BossDownedEditorUI.IBossProvider provider)
            {
                BossProviders.Add(provider);
            }
            else if (args[0] is "RegisterBuffProvider" && args[1] is PermanentBuffsEditorUI.IPermanentBuffProvider buffProvider)
            {
                BuffProviders.Add(buffProvider);
            }
            return null;
        }

        public override void Load()
        {
            BossProviders.Add(new Content.VanillaBossProvider());
            BuffProviders.Add(new Content.VanillaPermanentBuffProvider());
        }
    }
}
