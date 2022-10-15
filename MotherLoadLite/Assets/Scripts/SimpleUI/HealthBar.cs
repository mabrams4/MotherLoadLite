using UnityEngine;
using UnityEngine.UI;

// manages player health bar
public class HealthBar : MonoBehaviour
{

    #region Singleton
    public static HealthBar instance;


    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one HealthBar?");
            return;
        }
        instance = this;
    }
    #endregion

    // set health bar value
    public void SetHealth(float health)
    {
        GetComponent<Slider>().value = health ;
    }

    // set max health bar value
    public void SetMaxHealth(float health)
    {
        GetComponent<Slider>().maxValue = health;
    }
}
