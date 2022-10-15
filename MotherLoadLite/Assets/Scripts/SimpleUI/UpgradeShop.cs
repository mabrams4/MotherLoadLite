using UnityEngine;

// manages opening and closing upgrade shop UI
public class UpgradeShop : MonoBehaviour
{
    public GameObject UpgradeShopUI;
    public BMUpgradeShop upgradeShopManager;
    PlayerController player;
    public GameObject inventoryFullUI;

    bool resetUI;

    private void Start()
    {
        player = PlayerController.instance;
    }
    private void Update()
    {
        // if player is in bounds of upgrade shop open the UI
        if (GetComponent<Renderer>().bounds.Contains(player.transform.position)
            && player.IsGrounded())
        {
            UpgradeShopUI.SetActive(true);
            if (!resetUI)
            {
                upgradeShopManager.ResetUI();
                resetUI = true;
            }
        }
        else
        {
            UpgradeShopUI.SetActive(false);
            resetUI = false;
        }
    }
}
