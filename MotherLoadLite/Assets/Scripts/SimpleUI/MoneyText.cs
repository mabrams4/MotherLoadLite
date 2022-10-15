using UnityEngine;

// manages player money text UI
public class MoneyText : MonoBehaviour
{
    #region Singleton
    public static MoneyText instance;


    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one MoneyText?");
            return;
        }
        instance = this;
    }
    #endregion

    public void SetMoneyValue(float amount)
    {
        GetComponent<TMPro.TextMeshProUGUI>().text = "$" + amount;
    }
}
