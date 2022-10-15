using UnityEngine;

// manages ancient tech
public class AncientTechText : MonoBehaviour
{
    #region Singleton
    public static AncientTechText instance;


    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one AncientTech?");
            return;
        }
        instance = this;
    }
    #endregion

    // sets player ancient tech text value
    public void SetAncientTechValue(float value)
    {
        GetComponentInChildren<TMPro.TextMeshProUGUI>().text = value + " AT";
    }
}
