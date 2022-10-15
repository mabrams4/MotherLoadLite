using UnityEngine;

// manages player altitude text
public class AltitudeText : MonoBehaviour
{
    // set player altitude text value
    public void SetAltitude(int altitude)
    {
        GetComponent<TMPro.TextMeshProUGUI>().text = altitude + " ft";
    }
}
