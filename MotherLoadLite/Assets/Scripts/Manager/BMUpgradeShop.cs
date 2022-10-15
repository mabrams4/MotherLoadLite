using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/* this script deals with all buttons related to the upgrade shop */
public class BMUpgradeShop : MonoBehaviour
{
    PlayerInfo playerInfo;
    PlayerController playerController;
    MoneyText moneyText;
    DataMineralItem gameData;
    Inventory inventory;
    FuelBar fuelBar;
    HealthBar healthBar;
    AudioManager audioManager;

    public TMPro.TextMeshProUGUI purchaseText;
    public TMPro.TextMeshProUGUI titleText;

    public GameObject selectedUpgrade;
    public TMPro.TextMeshProUGUI selectedUpgradeName;
    public TMPro.TextMeshProUGUI selectedUpgradeDescription;

    public Image currentItemImage;
    public Image currentUpgradeImage;
    public Transform tabParent;

    public Sprite nullImage;
    public Item AMBUpgrade;
    Image selectedImage;
    public GameObject inventoryFullUI;
    public GameObject upgradeTabUIParent;
    public GameObject upgradeShopHomeTabText;

    void Start()
    {
        moneyText = MoneyText.instance;
        playerInfo = PlayerInfo.instance;
        playerController = PlayerController.instance;
        gameData = DataMineralItem.instance;
        inventory = Inventory.instance;
        fuelBar = FuelBar.instance;
        healthBar = HealthBar.instance;
        audioManager = GetComponent<AudioManager>();
    }

    // purchaces the selected item/upgrade
    public void PurchaseSelectedItem()
    {
        // extract the upgrade type or item from the sprite name
        string itemName = selectedImage.sprite.name;
        string[] tokens = itemName.Split(' ');
        string itemType = tokens[tokens.Length - 1];

        // update player inventory/money/stats based on upgrade/item being purchased
        switch (itemType)
        {
            case ("Drill"):
                Drill d = gameData.nameToDrillDict[itemName];
                // dont let player purchace a worse or the same upgrade and waste money
                if (inventory.currentDrill.drillSpeed >= d.drillSpeed) return;

                if (playerInfo.money >= d.cost)
                {
                    audioManager.PlayPurchaseItemSound();
                    UpdatePlayerMoney(d.cost);
                    inventory.currentDrill = d;
                    playerController.drillSpeed = d.drillSpeed;
                }
                else { Debug.Log("Not Enough Money"); }
                break;

            case ("Tank"):  // Fuel tanks
                FuelTank f = gameData.nameToFuelTankDict[itemName];
                // dont let player purchace a worse or the same upgrade and waste money
                if (inventory.currentFuelTank.maxFuel >= f.maxFuel) return;

                if (playerInfo.money >= f.cost)
                {
                    audioManager.PlayPurchaseItemSound();
                    UpdatePlayerMoney(f.cost);
                    inventory.currentFuelTank = f;
                    playerInfo.maxFuel = f.maxFuel;
                    playerInfo.currentFuel = f.maxFuel;
                    fuelBar.SetMaxFuel(f.maxFuel);
                    fuelBar.SetFuel(f.maxFuel);
                }
                else { Debug.Log("Not Enough Money"); }
                break;

            case ("Hull"):
                Hull h = gameData.nameToHullDict[itemName];
                // dont let player purchace a worse or the same upgrade and waste money
                if (inventory.currentHull.maxHealth >= h.maxHealth) return;

                if (playerInfo.money >= h.cost)
                {
                    audioManager.PlayPurchaseItemSound();
                    UpdatePlayerMoney(h.cost);
                    inventory.currentHull = h;
                    playerInfo.maxHealth = h.maxHealth;
                    playerInfo.currentHealth = h.maxHealth;
                    healthBar.SetMaxHealth(h.maxHealth);
                    healthBar.SetHealth(h.maxHealth);
                }
                else { Debug.Log("Not Enough Money"); }
                break;

            case ("Bay"):   // Storage Bays
                StorageBay s = gameData.nameToStorageBayDict[itemName];
                // dont let player purchace a worse or the same upgrade and waste money
                if (inventory.currentStorageBay.maxCapacity >= s.maxCapacity) return;

                if (playerInfo.money >= s.cost)
                {
                    audioManager.PlayPurchaseItemSound();
                    UpdatePlayerMoney(s.cost);
                    inventory.currentStorageBay = s;
                    inventory.maxCapacity = s.maxCapacity;
                }
                else { Debug.Log("Not Enough Money"); }
                break;

            default:    // Consumable Item
                Item item = gameData.nameToItemDict[selectedImage.sprite.name];
                if (item.name == "Anti-Matter Bomb" && !inventory.specialUpgrades.Contains(AMBUpgrade))
                {
                    Debug.Log("Anti-Matter Bomb research upgrade not yet purchased");
                    return;
                }
                if (playerInfo.money >= item.cost)
                {
                    if (inventory.currentCapacity >= inventory.maxCapacity)
                    {
                        audioManager.PlayInventoryFullSound();
                        inventoryFullUI.SetActive(true);

                        // diable all visible UI Buttons
                        Button[] buttons = upgradeTabUIParent.GetComponentsInChildren<Button>();
                        foreach (Button b in buttons) b.interactable = false;
                    }
                    else
                    {
                        audioManager.PlayPurchaseItemSound();
                        UpdatePlayerMoney(item.cost);
                        inventory.AddItem(item);
                    }
                }
                else { Debug.Log("Not Enough Money"); }
                break;
        }
    }

    // sets all Upgrade shop UI buttons active after clicking inventory Full OK button
    public void SetUIElementsActive()
    {
        Button[] buttons = upgradeTabUIParent.GetComponentsInChildren<Button>();
        foreach (Button b in buttons) b.interactable = true;
    }


    // updates player money 
    void UpdatePlayerMoney(float amount)
    {
        playerInfo.money -= amount;
        moneyText.SetMoneyValue(playerInfo.money);
    }

    // selects an item/upgrade and updates the UI accordingly
    public void SelectItem(Image img)
    {
        // extract the upgrade type or item from the sprite name
        selectedImage = img;    // set the selected image
        string itemName = img.sprite.name;
        string[] tokens = itemName.Split(' ');
        string itemType = tokens[tokens.Length - 1];
        
        // update selected upgrade UI
        selectedUpgrade.SetActive(true);
        selectedUpgradeName.text = itemName;
        selectedUpgrade.GetComponentInChildren<Image>().sprite = img.sprite;

        purchaseText.GetComponentInParent<Button>().interactable = true;
        
        switch (itemType)
        {
            case ("Drill"):
                Drill d = gameData.nameToDrillDict[itemName];
                purchaseText.text = "Purchase: $" + d.cost;
                selectedUpgradeDescription.text = "Drill speed: " + d.drillSpeed;
                break;
            case ("Tank"):  // Fuel tank
                FuelTank f = gameData.nameToFuelTankDict[itemName];
                purchaseText.text = "Purchase: $" + f.cost;
                selectedUpgradeDescription.text = "Max Fuel: " + f.maxFuel + "L";
                break;
            case ("Hull"):
                Hull h = gameData.nameToHullDict[itemName];
                purchaseText.text = "Purchase: $" + h.cost;
                selectedUpgradeDescription.text = "Max Health: " + h.maxHealth;
                break;
            case ("Bay"):   // Storage bay
                StorageBay s = gameData.nameToStorageBayDict[itemName];
                purchaseText.text = "Purchase: $" + s.cost;
                selectedUpgradeDescription.text = "Storage capacity: " + s.maxCapacity;
                break;
            default:    // Consumable Item
                UpdateUIForItem();
                break;
        }
    }

    // sets the current item UI to be true and updates it accordingly based on current selected image
    void UpdateUIForItem()
    {
        Item item = gameData.nameToItemDict[selectedImage.sprite.name];
        currentItemImage.gameObject.SetActive(true);
        currentItemImage.sprite = item.sprite;
        selectedUpgrade.SetActive(false);
        StopAllCoroutines();
        StartCoroutine(TypeSentence(item));
        purchaseText.text = "Purchase: $" + item.cost;
    }

    // resets the upgrade shop UI back to default case
    public void ResetUI()
    {
        titleText.text = "Upgrade Shop";
        purchaseText.text = "";
        purchaseText.GetComponentInParent<Button>().interactable = false;
        currentItemImage.gameObject.SetActive(false);
        currentUpgradeImage.gameObject.SetActive(false);
        selectedUpgrade.SetActive(false);
        upgradeShopHomeTabText.SetActive(true);

        Image[] imageArray = tabParent.GetComponentsInChildren<Image>();
        for (int i = 0; i < imageArray.Length; i++)
        {
            imageArray[i].color = new Color(1, 1, 1, 0);
            imageArray[i].GetComponent<Button>().interactable = false;
        }
    }

    // Animates out typing a sentence one character at a time
    IEnumerator TypeSentence(Item item)
    {
        currentItemImage.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = item.name + ":\n";

        foreach (char c in item.description.ToCharArray())
        {
            currentItemImage.GetComponentInChildren<TMPro.TextMeshProUGUI>().text += c;
            yield return new WaitForSecondsRealtime(.01f);
        }
    }
}
