using UnityEngine;
using UnityEngine.UI;

// manages player fuel bar
public class FuelBar : MonoBehaviour
{
    #region Singleton
    public static FuelBar instance;


    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one FuelBar?");
            return;
        }
        instance = this;
    }
    #endregion

    // sets fuek bar value
    public void SetFuel(float fuel)
    {
        GetComponent<Slider>().value = fuel;
    }

    // sets max fuel value
    public void SetMaxFuel(float fuel)
    {
        GetComponent<Slider>().maxValue = fuel;
    }
}
