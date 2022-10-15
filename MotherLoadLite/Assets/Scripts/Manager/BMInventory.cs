using UnityEngine;
using UnityEngine.UI;

/* this script deals with all buttons related to the players inventory */

public class BMInventory : MonoBehaviour
{
    InventoryMineral[] inventoryMinerals;
    InventoryItem[] inventoryItems;
    InventoryItem[] inventorySpecialUpgrades;
    Image[] inventoryUpgrades;


    public Transform inventoryItemsParent;
    public Transform inventoryMineralsParent;
    public Transform inventoryUpgradesParent;
    public Transform inventorySpecialUpgradeParent;

    public GameObject inventoryUI;
    public GameObject storageCapacity;
    public GameObject player;
    public GameObject mineralTab;
    public GameObject itemsTab;
    public GameObject upgradesTab;
    public GameObject mineralTabButton;
    public GameObject itemTabButton;
    public GameObject upgradesTabButton;
    public GameObject moneyText;

    Inventory inventory;

    Color32 greyedOut;

    private void Start()
    {
        inventory = Inventory.instance;

        inventoryItems = inventoryItemsParent.GetComponentsInChildren<InventoryItem>();
        inventoryMinerals = inventoryMineralsParent.GetComponentsInChildren<InventoryMineral>();
        inventoryUpgrades = inventoryUpgradesParent.GetComponentsInChildren<Image>();
        inventorySpecialUpgrades = inventorySpecialUpgradeParent.GetComponentsInChildren<InventoryItem>();

        greyedOut = new Color32(195, 193, 224, 100);
    }

    // switches to upgrades tab of the inventory and updates UI
    public void OpenUpgradesTab()
    {
        upgradesTab.SetActive(true);
        itemsTab.SetActive(false);
        mineralTab.SetActive(false);
        upgradesTabButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().color = Color.white;
        itemTabButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().color = greyedOut;
        mineralTabButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().color = greyedOut;

        foreach (Image i in inventoryUpgrades)
        {
            switch (i.gameObject.name)
            {
                case ("CurrentDrill"):
                    i.sprite = inventory.currentDrill.sprite;
                    i.gameObject.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = inventory.currentDrill.name;
                    break;
                case ("CurrentFuelTank"):
                    i.sprite = inventory.currentFuelTank.sprite;
                    i.gameObject.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = inventory.currentFuelTank.name;
                    break;
                case ("CurrentHull"):
                    i.sprite = inventory.currentHull.sprite;
                    i.gameObject.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = inventory.currentHull.name;
                    break;
                case ("CurrentStorageBay"):
                    i.sprite = inventory.currentStorageBay.sprite;
                    i.gameObject.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = inventory.currentStorageBay.name;
                    break;
            }
        }

        foreach (InventoryItem i in inventorySpecialUpgrades)
        {
            if (inventory.specialUpgrades.Contains(i.item)) i.UpdateSpecialUpgrade();
        }
    }

    // switches to mineral tab of the inventory and updates UI
    public void OpenMineralsTab()
    {
        itemsTab.SetActive(false);
        upgradesTab.SetActive(false);
        mineralTab.SetActive(true);
        mineralTabButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().color = Color.white;
        itemTabButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().color = greyedOut;
        upgradesTabButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().color = greyedOut;
        foreach (InventoryMineral im in inventoryMinerals) im.UpdateUI();
    }

    // switches to the items tab of the inventory and updates UI
    public void OpenItemsTab()
    {
        mineralTab.SetActive(false);
        upgradesTab.SetActive(false);
        itemsTab.SetActive(true);
        itemTabButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().color = Color.white;
        mineralTabButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().color = greyedOut;
        upgradesTabButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().color = greyedOut;

        foreach (InventoryItem i in inventoryItems)
        {
            if (inventory.items.ContainsKey(i.item)) i.UpdateConsumableItem();
        }
    }

    // opens players inventory updating UI data. Pauses game while inventory is open. Default tab is minearl tab
    public void OpenInventory()
    {
        if (inventoryUI.activeInHierarchy)  // inventory already open
        {
            Time.timeScale = 1f;
            moneyText.SetActive(true);
            inventoryUI.SetActive(false);
        } 
        else    // inventory was not open
        {
            Time.timeScale = 0f;
            inventoryUI.SetActive(true);
            moneyText.SetActive(false);
            OpenMineralsTab();
        }
    }
}
