using CalamityMod.Items.PermanentBoosters;
using CalamityMod;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Linq;
using BossLoadouts.Common.Systems;

public class Loadout
{
    public string Name { get; set; }

    public Item[] Inventory;
    public Item[] Armor;
    public Item Hook, Mount;
    public Item[] Ammo;

    public int ConsumedLifeFruit;
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

        // Initialize all Item arrays
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

        // Restore max life
        player.ConsumedLifeCrystals = 15; // Max crystal HP
        player.ConsumedLifeFruit = ConsumedLifeFruit;
        player.statLifeMax2 = (20 * player.ConsumedLifeCrystals + 100) + (5 * player.ConsumedLifeFruit);

        // Restore max mana
        player.ConsumedManaCrystals = 9;
        player.statManaMax2 = 20 + (20 * player.ConsumedManaCrystals);

        // Reset accessory slots
        player.extraAccessory = false;
        player.extraAccessorySlots = 0;

        // Apply saved powerups
        if (Powerups != null)
        {
            if (Powerups.Contains(ItemID.DemonHeart))
            {
                player.extraAccessory = true;
                player.extraAccessorySlots = 1;
            }

            var modPlayer = player.Calamity();

            // Calamity buffs
            modPlayer.bOrange = Powerups.Contains(ModContent.ItemType<BloodOrange>());
            modPlayer.dFruit = Powerups.Contains(ModContent.ItemType<Dragonfruit>());
            modPlayer.eBerry = Powerups.Contains(ModContent.ItemType<Elderberry>());
            modPlayer.mFruit = Powerups.Contains(ModContent.ItemType<MiracleFruit>());
            modPlayer.cShard = Powerups.Contains(ModContent.ItemType<CometShard>());
            modPlayer.eCore = Powerups.Contains(ModContent.ItemType<EtherealCore>());
            modPlayer.pHeart = Powerups.Contains(ModContent.ItemType<PhantomHeart>());
            if (Powerups.Contains(ModContent.ItemType<CelestialOnion>()))
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

            modPlayer.adrenalineBoostOne = Powerups.Contains(ModContent.ItemType<ElectrolyteGelPack>());
            modPlayer.adrenalineBoostTwo = Powerups.Contains(ModContent.ItemType<StarlightFuelCell>());
            modPlayer.adrenalineBoostThree = Powerups.Contains(ModContent.ItemType<Ectoheart>());

            modPlayer.rageBoostOne = Powerups.Contains(ModContent.ItemType<MushroomPlasmaRoot>());
            modPlayer.rageBoostTwo = Powerups.Contains(ModContent.ItemType<InfernalBlood>());
            modPlayer.rageBoostThree = Powerups.Contains(ModContent.ItemType<RedLightningContainer>());

            modPlayer.UpdateVisibleAccessories();
        }

        // Load inventory
        for (int i = 0; i < Inventory.Length; i++)
            player.inventory[i] = Inventory[i].Clone();

        // Load armor
        for (int i = 0; i < Armor.Length; i++)
            player.armor[i] = Armor[i].Clone();

        // Load misc equips
        player.miscEquips[3] = Mount;
        player.miscEquips[4] = Hook;

        // Load ammo
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

        ConsumedLifeFruit = player.ConsumedLifeFruit;
        Powerups = GetPowerupsFromPlayer(player);

        GetAllDowned();
        SaveLoadSystem.SaveGlobalData();
    }

    private int[] GetPowerupsFromPlayer(Player player)
    {
        List<int> powerups = new List<int>();
        var modPlayer = player.Calamity();

        if (player.extraAccessory) powerups.Add(ItemID.DemonHeart);

        if (modPlayer.bOrange) powerups.Add(ModContent.ItemType<BloodOrange>());
        if (modPlayer.dFruit) powerups.Add(ModContent.ItemType<Dragonfruit>());
        if (modPlayer.eBerry) powerups.Add(ModContent.ItemType<Elderberry>());
        if (modPlayer.mFruit) powerups.Add(ModContent.ItemType<MiracleFruit>());
        if (modPlayer.cShard) powerups.Add(ModContent.ItemType<CometShard>());
        if (modPlayer.eCore) powerups.Add(ModContent.ItemType<EtherealCore>());
        if (modPlayer.pHeart) powerups.Add(ModContent.ItemType<PhantomHeart>());
        if (modPlayer.extraAccessoryML) powerups.Add(ModContent.ItemType<CelestialOnion>());

        if (modPlayer.adrenalineBoostOne) powerups.Add(ModContent.ItemType<ElectrolyteGelPack>());
        if (modPlayer.adrenalineBoostTwo) powerups.Add(ModContent.ItemType<StarlightFuelCell>());
        if (modPlayer.adrenalineBoostThree) powerups.Add(ModContent.ItemType<Ectoheart>());
        if (modPlayer.rageBoostOne) powerups.Add(ModContent.ItemType<MushroomPlasmaRoot>());
        if (modPlayer.rageBoostTwo) powerups.Add(ModContent.ItemType<InfernalBlood>());
        if (modPlayer.rageBoostThree) powerups.Add(ModContent.ItemType<RedLightningContainer>());

        return powerups.ToArray();
    }

    public void GetAllDowned()
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
        downedBosses[29] = NPC.downedMechBoss1;
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
        NPC.downedBoss2 = downed[4];
        NPC.downedBoss2 = downed[5];
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
}
