using System;
using System.Collections.Generic;
using Terraria;

namespace BossLoadouts.Common.Systems
{
    public class Loadout
    {
        public string Name { get; set; }

        // Items for the loadout — this could be IDs or Item instances
        public List<Item> Weapons { get; set; }
        public List<Item> Armor { get; set; }
        public List<Item> Accessories { get; set; }
        public List<Item> Potions { get; set; }
        public List<Item> Miscellaneous { get; set; }

        // Optional: description or notes
        public string Description { get; set; }

        // DateTime created, could be useful for sorting or display
        public DateTime CreatedAt { get; set; }

        public Loadout()
        {
            Name = "New Loadout";
            Weapons = new List<Item>();
            Armor = new List<Item>();
            Accessories = new List<Item>();
            Potions = new List<Item>();
            Miscellaneous = new List<Item>();
            Description = "";
            CreatedAt = DateTime.Now;
        }

        // Example method to clear the loadout (reset items)
        public void Clear()
        {
            Weapons.Clear();
            Armor.Clear();
            Accessories.Clear();
            Potions.Clear();
            Miscellaneous.Clear();
        }

        // Example method to add an item to a category
        public void AddItem(Item item, LoadoutItemType type)
        {
            switch (type)
            {
                case LoadoutItemType.Weapon:
                    Weapons.Add(item);
                    break;
                case LoadoutItemType.Armor:
                    Armor.Add(item);
                    break;
                case LoadoutItemType.Accessory:
                    Accessories.Add(item);
                    break;
                case LoadoutItemType.Potion:
                    Potions.Add(item);
                    break;
                case LoadoutItemType.Misc:
                    Miscellaneous.Add(item);
                    break;
            }
        }
    }

    public enum LoadoutItemType
    {
        Weapon,
        Armor,
        Accessory,
        Potion,
        Misc
    }
}
