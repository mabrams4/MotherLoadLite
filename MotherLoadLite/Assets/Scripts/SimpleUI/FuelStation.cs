using UnityEngine;

// manages opening and closing fuel station UI
public class FuelStation : MonoBehaviour
{
    public GameObject fuelStationUI;
    PlayerController player;

    private void Start()
    {
        player = PlayerController.instance;
    }
    private void Update()
    {
        // if player is in bounds of fuel station open the UI
        if (GetComponent<Renderer>().bounds.Contains(player.transform.position)
            && player.IsGrounded())
        {
            fuelStationUI.SetActive(true);
        } 
        else 
        { 
            fuelStationUI.SetActive(false);
        }
    }
}
