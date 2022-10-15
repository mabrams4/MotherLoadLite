using UnityEngine;

// manages opening and closing repair shop UI
public class RepairShop : MonoBehaviour
{
    public GameObject repairShopUI;
    PlayerController player;


    private void Start()
    {
        player = PlayerController.instance;
    }
    private void Update()
    {
        // if player is in bounds of repair shop open the UI
        if (GetComponent<Renderer>().bounds.Contains(player.transform.position)
            && player.IsGrounded())
        {
            repairShopUI.SetActive(true);
        }
        else
        {
            repairShopUI.SetActive(false);
        }
    }
}
