using UnityEngine;
using UnityEngine.UI;

/* this script is attatched to all inventory items and manages their UI in the player inventory */
public class InventoryItem : MonoBehaviour
{
    public Item item;

    // if player has an item highlight image and text otherwise grey it out
    public void UpdateConsumableItem()
    {
        // had to do this here and not in start method to avoid occasional random null reference exceptions
        Inventory inventory = Inventory.instance;   

        if (!inventory.items.ContainsKey(item)) return;
        TMPro.TextMeshProUGUI itemText = GetComponentInChildren<TMPro.TextMeshProUGUI>();
        itemText.text = item.name;
        if (inventory.items[item] > 0)
        {
            string text = "x" + inventory.items[item];
            GetComponentInChildren<TMPro.TextMeshProUGUI>().text = text;
            GetComponentInChildren<TMPro.TextMeshProUGUI>().color = Color.white;
            GetComponent<Image>().color = Color.white;
        }
        else
        {
            GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "";
            GetComponent<Image>().color = new Color32(255, 255, 255, 150);
        }
    }

    // update the inventory UI to show that the player has the special upgrade
    public void UpdateSpecialUpgrade()
    {
        GetComponent<Image>().sprite = item.sprite;
        GetComponent<Image>().color = Color.white;
        GetComponentInChildren<TMPro.TextMeshProUGUI>().text = item.name;
        GetComponentInChildren<TMPro.TextMeshProUGUI>().color = Color.white;
    }
}
