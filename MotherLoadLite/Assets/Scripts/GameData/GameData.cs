using System.Collections.Generic;

/* This class stores all game data that needs to be kept track of when saving/loading a game */

[System.Serializable]
public class GameData
{
    // Equipment
    public string drill;
    public string fuelTank;
    public string hull;
    public string storageBay;

    // Player
    public float fuel;
    public float maxFuel;
    public float health;
    public float maxHealth;
    public float money;
    public float ancientTech;

    // Inventory
    public float inventoryCapacity;
    public float inventoryMaxCapacity;

    // Set the length based on number of minerals/items the player can have in their inventory
    public string[] minerals = new string[7];
    public int[] mineralCounts = new int[7];
    public string[] items = new string[5];
    public int[] itemCounts = new int[5];

    // Special Upgrades
    public bool hasAntiGravityDrill;
    public bool hasRegenerativeHull;
    public bool hasTeleporter;
    public bool hasAntiGravityBombs;

    // Mission Control Transmission progress
    public bool givenAncientTreasureTransmission;
    public bool givenFirstMissionControlTransmission;
    public bool givenBabyWormToSurfaceTransmission;
    public bool givenBabyWormGrowsUpTransmission;

    // Rogue Colony Transmission progress
    public bool givenFirstRougeColonyTransmission;
    public bool givenEncounterWormTransmission;
    public bool givenEncounterNestTransmission;
    public bool givenFoundBabyWormTransmission;
    public bool givenBlownUpQueenNestTransmission;

    // Baby Worm
    public string currentEvolution;
    public bool babyWormIsEvolved;

    public GameData (Inventory inventory, PlayerInfo playerInfo, 
                     MissionControl missionControl, RogueColony rogueColony,
                     BabyWorm babyWorm)
    {
        // Equipment
        drill = inventory.currentDrill.name;
        fuelTank = inventory.currentFuelTank.name;
        hull = inventory.currentHull.name;
        storageBay = inventory.currentStorageBay.name;

        // Player
        fuel = playerInfo.currentFuel;
        maxFuel = playerInfo.maxFuel;

        health = playerInfo.currentHealth;
        maxHealth = playerInfo.maxHealth;

        money = playerInfo.money;
        ancientTech = playerInfo.ancientTech;

        // Inventory
        inventoryCapacity = inventory.currentCapacity;
        inventoryMaxCapacity = inventory.maxCapacity;

        // Minerals
        int index = 0;
        foreach (KeyValuePair<Mineral, int> kvp in inventory.minerals)
        {
            minerals[index] = kvp.Key.name;
            mineralCounts[index] = kvp.Value;
            index++;
        }

        // Items
        index = 0;
        foreach (KeyValuePair<Item, int> kvp in inventory.items)
        {
            items[index] = kvp.Key.name;
            itemCounts[index] = kvp.Value;
            index++;
        }

        // Special Upgrades
        hasAntiGravityDrill = playerInfo.hasAntiGravityDrill;
        hasRegenerativeHull = playerInfo.hasRegenerativeHull;
        hasTeleporter = playerInfo.hasTeleporter;
        hasAntiGravityBombs = playerInfo.hasAntiGravityBombs;

        // Mission Control Transmission Progress
        givenFirstMissionControlTransmission = missionControl.givenFirstMissionControlTransmission;
        givenAncientTreasureTransmission = missionControl.givenAncientTreasureTransmission;
        givenBabyWormToSurfaceTransmission = missionControl.givenBabyWormToSurfaceTransmission;
        givenBabyWormGrowsUpTransmission = missionControl.givenBabyWormGrowsUpTransmission;

        // Rogue Colony Transmission Progress
        givenFirstRougeColonyTransmission = rogueColony.givenFirstRogueColonyTransmission;
        givenEncounterWormTransmission = rogueColony.givenEncounterWormTransmission;
        givenEncounterNestTransmission = rogueColony.givenEncounterNestTransmission;
        givenFoundBabyWormTransmission = rogueColony.givenFoundBabyWormTransmission;
        givenBlownUpQueenNestTransmission = rogueColony.givenBlownUpQueenNestTransmission;

        // Baby Worm
        babyWormIsEvolved = babyWorm.isEvolved;
        currentEvolution = babyWorm.currentEvolution;
    }
}
