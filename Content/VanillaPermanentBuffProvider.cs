using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BossLoadouts.Content.UI.PermanentBuffsEditorUI;
using Terraria.ID;
using Terraria;

namespace BossLoadouts.Content
{
    public class VanillaPermanentBuffProvider : IPermanentBuffProvider
    {
        public IEnumerable<PermanentBuffEntry> GetBuffEntries()
        {
            Player player = Main.LocalPlayer;

            return new List<PermanentBuffEntry>()
            {
                new PermanentBuffEntry()
                {
                    Name = "Demon Heart",
                    SortIndex = 20,
                    IsUnlocked = () => player.extraAccessory,
                    SetUnlocked = val =>
                    {
                        if (val && !player.extraAccessory)
                        {
                            player.extraAccessory = true;
                            player.extraAccessorySlots += 1;
                        }
                        else if (!val && player.extraAccessory)
                        {
                            player.extraAccessory = false;
                            player.extraAccessorySlots -= 1;
                        }
                    }
                },
            };
        }
    }
}
