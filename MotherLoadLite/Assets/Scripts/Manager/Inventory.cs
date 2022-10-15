using System.Collections.Generic;
using UnityEngine;

/* This script manages the players inventory */
public class Inventory : MonoBehaviour
{
    #region Singleton
    public static Inventory instance;


    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one inventory?");
            return;
        }
        instance = this;
    }
    #endregion

    // minerals
    public Dictionary<Mineral, int> minerals = new Dictionary<Mineral, int>();

    // items
    public Dictionary<Item, int> items = new Dictionary<Item, int>();

    // special upgrades
    public List<Item> specialUpgrades = new List<Item>();

    // current upgrades
    public Drill currentDrill;
    public FuelTank currentFuelTank;
    public StorageBay currentStorageBay;
    public Hull currentHull;

    public float maxCapacity = 20;
    public float currentCapacity = 0;
    public GameObject inventoryUI;

    AudioManager audioManager;

    
    void Start()
    {
        inventoryUI.SetActive(false);
        if (SaveSystem.isNewGame) maxCapacity = currentStorageBay.maxCapacity;
        audioManager = GetComponent<AudioManager>();
    }

    // When loading a saved game adds the correct amount of a given mineral to the inventory
    public void LoadMineral(Mineral m, int count)
    {
        minerals.Add(m, count);
    }

    // When loading a saved game adds the correct amount of a given item to the inventory
    public void LoadItem(Item item, int count)
    {
        items.Add(item, count);
    }

    // adds a mineral to the inventory
    public void AddMineral(Mineral m)
    {
        if (currentCapacity < maxCapacity)
        {
            Debug.Log("+1 " + m.name);
            if (!minerals.ContainsKey(m)) minerals.Add(m, 1);
            else { minerals[m]++; }
            currentCapacity++;
        }
        else { audioManager.PlayInventoryFullSound(); }
    }

    // removes one of mineral m from the inventory
    public void RemoveOneMineral(Mineral m)
    {
        if (!minerals.ContainsKey(m) || minerals[m] == 0) return;
        minerals[m]--;
        currentCapacity--;
    }

    // removes all of mineral m from the inventory
    public void RemoveAll(Mineral m)
    {
        minerals[m] = 0;
    }

    // adds an item to the inventory
    public void AddItem(Item item)
    {
        if (!items.ContainsKey(item))
        {
            items.Add(item, 1);
        }
        else
        {
            items[item]++;
        }
        audioManager.PlayPurchaseItemSound();
        currentCapacity++;
        Debug.Log("+1 " + item);
    }

    // Uses an item and updates its UI in the inventory
    public void UseItem(Item item)
    {
        items[item]--;
        currentCapacity--;
    }
}
