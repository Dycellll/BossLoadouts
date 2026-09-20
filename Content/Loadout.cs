using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Linq;
using BossLoadouts.Common.Systems;
using System;
using static BossLoadouts.Content.UI.BossDownedEditorUI;
using BossLoadouts;
using BossLoadouts.Content.UI;

public class Loadout
{
    public string Name { get; set; }

    public Item[] Inventory;
    public Item[] Armor;
    public Item Hook, Mount;
    public Item[] Ammo;

    public int ConsumedLifeCrystals;
    public int ConsumedLifeFruit;
    public int ConsumedManaCrystals;
    public int[] Powerups;
    public bool[] downedBosses = new bool[45];

    public Loadout()
    {
        Name = "New Loadout";
        Inventory = new Item[50];
        Armor = new Item[10];
        Hook = new Item();
        Mount = new Item();
        Ammo = new Item[4];
        ConsumedLifeCrystals = Player.LifeCrystalMax;
        ConsumedManaCrystals = Player.ManaCrystalMax;

        for (int i = 0; i < Inventory.Length; i++) Inventory[i] = new Item();
        for (int i = 0; i < Armor.Length; i++) Armor[i] = new Item();
        Hook = new Item();
        Mount = new Item();
        for (int i = 0; i < Ammo.Length; i++) Ammo[i] = new Item();
    }

    public void LoadGear(Player player)
    {
        for (int i = 0; i < player.buffType.Length; i++)
        {
            player.ClearBuff(player.buffType[i]);
        }

        var mp = player.GetModPlayer<BossLoadouts.Content.MyPlayer>();

        if (ConsumedLifeCrystals >= 0)
        {
            player.ConsumedLifeCrystals = ConsumedLifeCrystals;
            mp.NegativeLifeCrystals = 0;
        }
        else
        {
            player.ConsumedLifeCrystals = 0;
            mp.NegativeLifeCrystals = -ConsumedLifeCrystals;
        }

        player.ConsumedLifeFruit = ConsumedLifeFruit;
        player.statLifeMax2 = (20 * player.ConsumedLifeCrystals + 100) + (5 * player.ConsumedLifeFruit) - (mp.NegativeLifeCrystals * 20);

        player.ConsumedManaCrystals = ConsumedManaCrystals;
        player.statManaMax2 = 20 + (20 * player.ConsumedManaCrystals);

        player.extraAccessory = false;
        player.extraAccessorySlots = 0;

        if (Powerups != null)
        {
            SetAllPowerups(Powerups);
        }

        for (int i = 0; i < Inventory.Length; i++)
            player.inventory[i] = Inventory[i].Clone();

        for (int i = 0; i < Armor.Length; i++)
            player.armor[i] = Armor[i].Clone();

        player.miscEquips[3] = Mount;
        player.miscEquips[4] = Hook;

        for (int i = 0; i < Ammo.Length; i++)
            player.inventory[Main.InventorySlotsTotal - 4 + i] = Ammo[i].Clone();

        SetAllDowned(downedBosses);
    }

    public void SaveGear(Player player)
    {
        for (int i = 0; i < Inventory.Length; i++)
            Inventory[i] = player.inventory[i].Clone();

        for (int i = 0; i < Armor.Length; i++)
            Armor[i] = player.armor[i].Clone();

        Mount = player.miscEquips[3].Clone();
        Hook = player.miscEquips[4].Clone();

        for (int i = 0; i < Ammo.Length; i++)
            Ammo[i] = player.inventory[Main.InventorySlotsTotal - 4 + i].Clone();

        var mp = player.GetModPlayer<BossLoadouts.Content.MyPlayer>();
        ConsumedLifeCrystals = mp.NegativeLifeCrystals > 0 ? -mp.NegativeLifeCrystals : player.ConsumedLifeCrystals;
        ConsumedLifeFruit = player.ConsumedLifeFruit;
        ConsumedManaCrystals = player.ConsumedManaCrystals;

        GetAllPowerups();
        GetAllDowned();
        SaveLoadSystem.SaveGlobalData();
    }

    public void GetAllPowerups()
    {
        int maxSortIndex = 0;

        foreach (PermanentBuffsEditorUI.IPermanentBuffProvider provider in BossLoadouts.BossLoadouts.BuffProviders)
        {
            foreach (var entry in provider.GetBuffEntries())
            {
                if (entry.SortIndex > maxSortIndex)
                    maxSortIndex = entry.SortIndex;
            }
        }

        Powerups = new int[maxSortIndex + 1];

        foreach (PermanentBuffsEditorUI.IPermanentBuffProvider provider in BossLoadouts.BossLoadouts.BuffProviders)
        {
            foreach (var entry in provider.GetBuffEntries())
            {
                if (entry.SortIndex < 0 || entry.SortIndex >= Powerups.Length)
                    continue;

                Powerups[entry.SortIndex] = entry.IsUnlocked() ? 1 : 0;
            }
        }
    }

    public static void SetAllPowerups(int[] powerups)
    {
        foreach (PermanentBuffsEditorUI.IPermanentBuffProvider provider in BossLoadouts.BossLoadouts.BuffProviders)
        {
            foreach (var entry in provider.GetBuffEntries())
            {
                if (entry.SortIndex < 0 || entry.SortIndex >= powerups.Length)
                    continue;

                entry.SetUnlocked(powerups[entry.SortIndex] != 0);
            }
        }
    }

    public void GetAllDowned()
    {
        int maxSortIndex = 0;

        foreach (IBossProvider provider in BossLoadouts.BossLoadouts.BossProviders)
        {
            foreach (var entry in provider.GetBossEntries())
            {
                if (entry.SortIndex > maxSortIndex)
                    maxSortIndex = entry.SortIndex;
            }
        }

        downedBosses = new bool[maxSortIndex + 1];

        foreach (IBossProvider provider in BossLoadouts.BossLoadouts.BossProviders)
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
        foreach (IBossProvider provider in BossLoadouts.BossLoadouts.BossProviders)
        {
            foreach (var entry in provider.GetBossEntries())
            {
                if (entry.SortIndex < 0 || entry.SortIndex >= downed.Length)
                    continue;

                entry.SetDowned(downed[entry.SortIndex]);
            }
        }
    }
}