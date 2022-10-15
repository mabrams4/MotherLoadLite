using UnityEngine;

/* this script deals with all buttons related to the repair shop */

public class BMRepairShop : MonoBehaviour
{
    PlayerInfo playerInfo;
    MoneyText moneyText;
    HealthBar healthBar;
    public float repairCost = 5;

    private void Start()
    {
        healthBar = HealthBar.instance;
        moneyText = MoneyText.instance;
        playerInfo = PlayerInfo.instance;
    }

    // refills player health
    public void RefillHealth()
    {
        if (playerInfo.currentHealth == playerInfo.maxHealth || playerInfo.money == 0) return;
        float healthToBuy = playerInfo.maxHealth - playerInfo.currentHealth;
        float totalCost = Mathf.Floor(healthToBuy * repairCost);
        if (playerInfo.money < totalCost)
        {
            healthToBuy = playerInfo.money / repairCost;
            totalCost = healthToBuy * repairCost;
        }
        playerInfo.money -= totalCost;
        playerInfo.currentHealth += healthToBuy;
        healthBar.SetHealth(playerInfo.currentHealth);
        moneyText.SetMoneyValue(playerInfo.money);
        GetComponent<AudioManager>().PlayPurchaseItemSound();
    }
}
