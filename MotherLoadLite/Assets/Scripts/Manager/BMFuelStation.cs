using UnityEngine;

/* this script deals with all buttons related to the fuel station */
public class BMFuelStation : MonoBehaviour
{
    PlayerInfo playerInfo;
    MoneyText moneyText;
    FuelBar fuelBar;
    public float fuelCost = 1;

    private void Start()
    {
        moneyText = MoneyText.instance;
        fuelBar = FuelBar.instance;
        playerInfo = PlayerInfo.instance;
    }

    // refill player fuel bar
    public void RefillFuel()
    {
        float fuelToBuy = playerInfo.maxFuel - playerInfo.currentFuel;
        float totalCost = fuelToBuy * fuelCost;
        if (playerInfo.money < totalCost)
        {
            fuelToBuy = playerInfo.money / fuelCost;
            totalCost = fuelToBuy * fuelCost;
        }
        playerInfo.money -= totalCost;
        playerInfo.currentFuel += fuelToBuy;
        fuelBar.SetFuel(playerInfo.currentFuel);
        moneyText.SetMoneyValue(playerInfo.money);
        GetComponent<AudioManager>().PlayPurchaseItemSound();
    }
}
