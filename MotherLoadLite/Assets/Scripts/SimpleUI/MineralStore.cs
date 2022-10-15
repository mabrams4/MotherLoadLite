using UnityEngine;

// manages opening and closing the mineral store UI
public class MineralStore : MonoBehaviour
{
    public GameObject mineralStoreUI;
    public MineralStoreSlots mineralStoreSlots;
    PlayerController controller;

    bool hasBeenUpdated = false;

    private void Start()
    {
        controller = PlayerController.instance;
    }
    private void Update()
    {
        // if player is in bounds of mineral store open the UI
        if (GetComponent<Renderer>().bounds.Contains(controller.transform.position)
            && controller.IsGrounded())
        {
            mineralStoreUI.SetActive(true);
            if (!hasBeenUpdated)
            {
                mineralStoreSlots.UpdateUI();   // update UI on open but not every frame
                hasBeenUpdated = true;
            }
        }
        else
        {
            mineralStoreUI.SetActive(false);
            hasBeenUpdated = false;
        }
    }
}
