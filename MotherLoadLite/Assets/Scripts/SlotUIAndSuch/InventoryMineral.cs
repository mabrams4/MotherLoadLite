using UnityEngine;
using UnityEngine.UI;

/* this script is attatched to all inventory minerals and manages their UI in the player inventory */

public class InventoryMineral : MonoBehaviour
{
    Inventory inventory;

    public TMPro.TextMeshProUGUI storageCapacity;
    public Mineral mineral;

    // update the UI for the given mineral if player has the mineral, highlight image and text otherwise grey it out
    public void UpdateUI()
    {
        // had to do this here and not in start method to avoid occasional random null reference exceptions
        inventory = Inventory.instance;

        if (!inventory.minerals.ContainsKey(mineral)) return;
        Image img = GetComponent<Image>();
        TMPro.TextMeshProUGUI t = GetComponentInChildren<TMPro.TextMeshProUGUI>();
        if (inventory.minerals[mineral] > 0)
        {
            GetComponent<Button>().interactable = true;
            t.text = "x" + inventory.minerals[mineral] + " " + mineral.name;
            img.color = Color.white;
        }
        else
        {
            GetComponent<Button>().interactable = false;
            t.text = "";
            img.color = new Color32(255, 255, 255, 150);
        }
        UpdateStorageCapacity();
    }

    // remove the mineral from inventory and update UI
    public void RemoveMineral()
    {
        inventory.RemoveOneMineral(mineral);
        UpdateUI();
    }

    // update storage capacity text
    void UpdateStorageCapacity()
    {
        double percent = System.Math.Round(inventory.currentCapacity / inventory.maxCapacity * 100f, 2);
        storageCapacity.text = ("Storage Capacity: " + percent + "%");
    }
}
