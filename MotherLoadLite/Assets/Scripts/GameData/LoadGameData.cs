using UnityEngine;

/* This script is responsible for loading in a saved game */
public class LoadGameData : MonoBehaviour
{
    Inventory inventory;
    PlayerInfo player;
    DataMineralItem d;

    AncientTechText ancientTechText;
    MoneyText moneyText;
    FuelBar fuelBar;
    HealthBar healthBar;

    MissionControl missionControl;
    RogueColony rogueColony;

    public GameObject researchStation;

    BabyWorm babyWorm;
    void Start()
    {
        inventory = Inventory.instance;
        player = PlayerInfo.instance;
        d = DataMineralItem.instance;
        ancientTechText = AncientTechText.instance;
        moneyText = MoneyText.instance;
        fuelBar = FuelBar.instance;
        healthBar = HealthBar.instance;
        missionControl = MissionControl.instance;
        rogueColony = RogueColony.instance;
        babyWorm = BabyWorm.instance;

        LoadGame();
    }

    void LoadGame()
    {
        if (!SaveSystem.isNewGame)
        {
            GameData data = SaveSystem.LoadGame();      // retrive all relevant game data

            // Equipment
            inventory.currentDrill = d.nameToDrillDict[data.drill];
            inventory.currentFuelTank = d.nameToFuelTankDict[data.fuelTank];
            inventory.currentHull = d.nameToHullDict[data.hull];
            inventory.currentStorageBay = d.nameToStorageBayDict[data.storageBay];


            // Player
            player.GetComponent<PlayerController>().drillSpeed = d.nameToDrillDict[data.drill].drillSpeed;
            player.currentFuel = data.fuel;
            player.maxFuel = data.maxFuel;
            player.currentHealth = data.health;
            player.maxHealth = data.maxHealth;
            player.money = data.money;
            player.ancientTech = data.ancientTech;

            fuelBar.SetFuel(data.fuel);
            fuelBar.SetMaxFuel(data.maxFuel);
            healthBar.SetMaxHealth(data.maxHealth);
            healthBar.SetHealth(data.health);
            moneyText.SetMoneyValue(data.money);
            ancientTechText.SetAncientTechValue(data.ancientTech);

            // Inventory 
            inventory.currentCapacity = data.inventoryCapacity;
            inventory.maxCapacity = data.inventoryMaxCapacity;

            // Minerals
            for (int i = 0; i < data.minerals.Length; i++)
            {
                if (data.minerals[i] == null) continue;
                Mineral m = d.nameToMineralDict[data.minerals[i]];
                int count = data.mineralCounts[i];
                inventory.LoadMineral(m, count);
            }

            // Items
            for (int i = 0; i < data.items.Length; i++)
            {
                if (data.items[i] == null) continue;
                Item item = d.nameToItemDict[data.items[i]];
                int count = data.itemCounts[i];
                inventory.LoadItem(item, count);
            }

            // Special Upgrades
            if (data.hasAntiGravityDrill)
            {
                player.hasAntiGravityDrill = true;
                Item antiGravityDrill = d.nameToItemDict["Anti-Gravity Drill"];
                inventory.specialUpgrades.Add(antiGravityDrill);
            }
            if (data.hasRegenerativeHull)
            {
                player.hasRegenerativeHull = true;
                Item regenerativeHull = d.nameToItemDict["Regenerative Hull"];
                inventory.specialUpgrades.Add(regenerativeHull);
            }
            if (data.hasTeleporter)
            {
                player.hasTeleporter = true;
                Item teleporter = d.nameToItemDict["Teleporter"];
                inventory.specialUpgrades.Add(teleporter);
            }
            if (data.hasAntiGravityBombs)
            {
                player.hasAntiGravityBombs = true;
                Item antiGravityBombs = d.nameToItemDict["Anti-Matter Bombs"];
                inventory.specialUpgrades.Add(antiGravityBombs);
            }

            // Mission Control Transmission Progress
            missionControl.givenFirstMissionControlTransmission = data.givenFirstMissionControlTransmission;
            missionControl.givenAncientTreasureTransmission = data.givenAncientTreasureTransmission;
            missionControl.givenBabyWormToSurfaceTransmission = data.givenBabyWormToSurfaceTransmission;
            missionControl.givenBabyWormGrowsUpTransmission = data.givenBabyWormGrowsUpTransmission;

            // Rogue Colony Transmission Progress
            rogueColony.givenFirstRogueColonyTransmission = data.givenFirstRougeColonyTransmission;
            if (data.givenFirstRougeColonyTransmission) researchStation.SetActive(true);

            rogueColony.givenEncounterWormTransmission = data.givenEncounterWormTransmission;
            rogueColony.givenEncounterNestTransmission = data.givenEncounterNestTransmission;
            rogueColony.givenFoundBabyWormTransmission = data.givenFoundBabyWormTransmission;
            rogueColony.givenBlownUpQueenNestTransmission = data.givenBlownUpQueenNestTransmission;

            // Baby Worm
            switch (data.currentEvolution)      
            {
                case ("UNDISCOVERED"):
                    babyWorm.evolution = BabyWorm.Evolutions.UNDISCOVERED;
                    babyWorm.currentEvolution = "UNDISCOVERED";
                    break;
                case ("BABY"):
                    babyWorm.evolution = BabyWorm.Evolutions.BABY;
                    babyWorm.currentEvolution = "BABY";
                    break;
                case ("ADULT"):
                    babyWorm.evolution = BabyWorm.Evolutions.ADULT;
                    babyWorm.currentEvolution = "ADULT";
                    babyWorm.Mature();
                    break;
            }
        }
    }
}
