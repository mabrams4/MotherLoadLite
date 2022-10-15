using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

/* this script deals with all buttons related to the mineral store */

public class BMMineralStore : MonoBehaviour
{

    public MineralStoreSlots mineralStoreSlots;
    public Canvas canvas;

    List<Mineral> toRemove = new List<Mineral>();

    Inventory inventory;
    PlayerInfo playerInfo;
    MoneyText moneyText;
    GraphicRaycaster m_Raycaster;
    PointerEventData m_PointerEventData;
    EventSystem m_EventSystem;
    AudioManager audioManager;

    private void Start()
    {
        inventory = Inventory.instance;
        playerInfo = PlayerInfo.instance;
        moneyText = MoneyText.instance;
        audioManager = GetComponent<AudioManager>();

        m_Raycaster = canvas.GetComponent<GraphicRaycaster>();
        m_EventSystem = FindObjectOfType<EventSystem>();
    }

    // sells all minerals in players inventory
    public void SellMinerals()
    {
        foreach (KeyValuePair<Mineral, int> kvp in inventory.minerals)
        {
            playerInfo.money += (kvp.Value * kvp.Key.cashValue);
            toRemove.Add(kvp.Key);
        }
        foreach (Mineral m in toRemove)
        {
            inventory.RemoveAll(m);
        }
        inventory.currentCapacity = 0;
        moneyText.SetMoneyValue(playerInfo.money);
        mineralStoreSlots.RemoveMinerals(0);
        audioManager.PlayPurchaseItemSound();
    }

    // sells a single mineral and updates mineral store UI to look cleannnn
    public void SellOneMineral()
    {
        // create new pointer event data
        m_PointerEventData = new PointerEventData(m_EventSystem);

        // set position of event to mouse position
        m_PointerEventData.position = Input.mousePosition;

        // generate list of raycast results
        List<RaycastResult> results = new List<RaycastResult>();
        m_Raycaster.Raycast(m_PointerEventData, results);
        
        foreach (RaycastResult result in results)
        {
            // if clicked on a mineral sell one mineral
            if (result.gameObject.name == "MineralCount")
            {
                string mineralName = result.gameObject.GetComponentInParent<Image>().sprite.name;
                foreach (Mineral m in inventory.minerals.Keys)
                {
                    if (m.name == mineralName)
                    {
                        if (inventory.minerals[m] == 0) return;
                        Debug.Log("Selling one " + mineralName);
                        inventory.RemoveOneMineral(m);
                        playerInfo.money += m.cashValue;
                        moneyText.SetMoneyValue(playerInfo.money);
                        mineralStoreSlots.UpdateUI();
                        audioManager.PlayPurchaseItemSound();
                        break;
                    }
                }
                break;
            }
        }
    }
}
