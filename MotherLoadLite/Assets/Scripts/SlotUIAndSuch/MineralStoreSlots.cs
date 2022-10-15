using UnityEngine;
using UnityEngine.UI;

/* this script manages the UI for the mineral store */
public class MineralStoreSlots : MonoBehaviour
{
    Image[] mineralSlots;
    public Sprite[] mineralIcons;
    public Sprite emptySprite;
    public TMPro.TextMeshProUGUI totalValueText;

    Inventory inventory;
    DataMineralItem gameData;

    Color white = new Color(1, 1, 1, 1);
    Color clear = new Color(1, 1, 1, 0);

    void Start()
    {
        inventory = Inventory.instance;
        gameData = DataMineralItem.instance;
        mineralSlots = GetComponentsInChildren<Image>();
    }

    // updates the UI for the mineral store
    public void UpdateUI()
    {
        int index = 0;
        int totalValue = 0;
        foreach (Sprite sprite in mineralIcons)
        {
            Mineral m = gameData.nameToMineralDict[sprite.name];
            if (!inventory.minerals.ContainsKey(m)) continue;
            if (inventory.minerals[m] > 0)
            {
                int count = inventory.minerals[m];
                totalValue += m.cashValue * count;
                SetMineralSlot(sprite, index, m, count);
                index++;
            }
        }
        totalValueText.text = "$" + totalValue.ToString();
        RemoveMinerals(index);
    }

    // set the UI for a mineral slot given the slot index 
    void SetMineralSlot(Sprite sprite, int slotIndex, Mineral m, int count)
    {
        mineralSlots[slotIndex].GetComponent<Button>().interactable = true;
        mineralSlots[slotIndex].sprite = sprite;
        mineralSlots[slotIndex].color = white;
        string text = "   " + m.name + "   " + count + "x     $" + m.cashValue;
        mineralSlots[slotIndex].GetComponentInChildren<TMPro.TextMeshProUGUI>().text = text;
    }

    // remove UI for all minerals starting at the given slot index, if removing all minerals set total value to $0
    public void RemoveMinerals(int startIndex)
    {
        for (int i = startIndex; i < mineralSlots.Length; i++)
        {
            mineralSlots[i].GetComponent<Button>().interactable = false;
            mineralSlots[i].sprite = emptySprite;
            mineralSlots[i].color = clear;
            mineralSlots[i].GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "";
        }
        if (startIndex == 0) totalValueText.text = "$0";
    }
}
