using UnityEngine;
using UnityEngine.UI;

/* Each tab in the upgrade shop has an icon that contains its own array of items to display.
 * Whenever a tab is clicked, the appropriate sprites are swapped into place using the 
 * SwapSprites function
 */
public class UpgradeStoreTabIcon : MonoBehaviour
{
    public Item[] items;
    public Drill[] drills;
    public FuelTank[] fuelTanks;
    public Hull[] hulls;
    public StorageBay[] storageBays;

    public Transform tabParent;
    public Image currentUpgradeImage;
    public Image currentItemImage;

    public TMPro.TextMeshProUGUI titleText;
    public TMPro.TextMeshProUGUI currentUpgradeName;
    public TMPro.TextMeshProUGUI currentUpgradeDescription;
    public TMPro.TextMeshProUGUI purchaseText;

    public GameObject selectedUpgrade;

    Inventory inventory;

    private void Start()
    {
        inventory = Inventory.instance;
    }

    // swaps the image sprites of the upgrade shop to the sprites for the
    // selected tab and resets the other UI elements
    public void SwapSprites()
    {
        Image[] imageArray = tabParent.GetComponentsInChildren<Image>();
        currentUpgradeImage.gameObject.SetActive(true);
        currentItemImage.gameObject.SetActive(false);
        selectedUpgrade.SetActive(false);
        titleText.text = gameObject.name;
        purchaseText.text = "";
        purchaseText.GetComponentInParent<Button>().interactable = false;

        for (int i = 0; i < imageArray.Length; i++)
        {
            imageArray[i].enabled = true;
            imageArray[i].GetComponent<Button>().interactable = true;
            imageArray[i].color = Color.white;

            switch (gameObject.name)
            {
                case ("Drills"):
                    imageArray[i].sprite = drills[i].sprite;
                    currentUpgradeImage.sprite = inventory.currentDrill.sprite;
                    currentUpgradeName.text = inventory.currentDrill.name;
                    currentUpgradeDescription.text = "Drill speed: " + inventory.currentDrill.drillSpeed;
                    break;
                case ("FuelTanks"):
                    imageArray[i].sprite = fuelTanks[i].sprite;
                    currentUpgradeImage.sprite = inventory.currentFuelTank.sprite;
                    currentUpgradeName.text = inventory.currentFuelTank.name;
                    currentUpgradeDescription.text = "Max fuel: " + inventory.currentFuelTank.maxFuel + "L";
                    break;
                case ("Hulls"):
                    imageArray[i].sprite = hulls[i].sprite;
                    currentUpgradeImage.sprite = inventory.currentHull.sprite;
                    currentUpgradeName.text = inventory.currentHull.name;
                    currentUpgradeDescription.text = "Max health: " + inventory.currentHull.maxHealth;
                    break;
                case ("StorageBays"):
                    // only 5 total storage bays
                    if (i == 5)
                    {
                        imageArray[i].enabled = false;
                        imageArray[i].GetComponent<Button>().interactable = false;
                        return;
                    }
                    imageArray[i].sprite = storageBays[i].sprite;
                    currentUpgradeImage.sprite = inventory.currentStorageBay.sprite;
                    currentUpgradeName.text = inventory.currentStorageBay.name;
                    currentUpgradeDescription.text = "Storage Capacity: " + inventory.currentStorageBay.maxCapacity;
                    break;
                case ("Consumables"):
                    // only 5 consumable items
                    if (i == 5)
                    {
                        imageArray[i].enabled = false;
                        imageArray[i].GetComponent<Button>().interactable = false;
                        return;
                    }
                    imageArray[i].sprite = items[i].sprite;
                    currentUpgradeImage.gameObject.SetActive(false);
                    break;
            }

        }
    }
}
