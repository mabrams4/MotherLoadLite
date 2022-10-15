using System.Collections.Generic;
using UnityEngine;

/* This class contains various dictionaries that map different game items/upgrades to their names for easy access
 * in other scripts 
 */
public class DataMineralItem : MonoBehaviour
{

    #region Singleton
    public static DataMineralItem instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one DataMineralItem?");
            return;
        }
        instance = this;
    }
    #endregion

    public Dictionary<string, Mineral> nameToMineralDict = new();
    public Dictionary<string, Item> nameToItemDict = new();
    public Dictionary<string, Drill> nameToDrillDict = new();
    public Dictionary<string, FuelTank> nameToFuelTankDict = new();
    public Dictionary<string, Hull> nameToHullDict = new();
    public Dictionary<string, StorageBay> nameToStorageBayDict = new();

    // passed in manually
    public Mineral[] mineralArray;  
    public Item[] items;
    public Drill[] drills;
    public FuelTank[] fuelTanks;
    public Hull[] hulls;
    public StorageBay[] storageBays;

    // initializes all dictionaries
    private void Start()
    {
        foreach (Mineral m in mineralArray)
        {
            nameToMineralDict[m.name] = m;
        }
        foreach (Item i in items)
        {
            nameToItemDict[i.name] = i;
        }
        foreach (Drill d in drills)
        {
            nameToDrillDict[d.name] = d;
        }
        foreach (FuelTank f in fuelTanks)
        {
            nameToFuelTankDict[f.name] = f;
        }
        foreach (Hull h in hulls)
        {
            nameToHullDict[h.name] = h;
        }
        foreach (StorageBay s in storageBays)
        {
            nameToStorageBayDict[s.name] = s;
        }
    }   
}
