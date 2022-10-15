using UnityEngine;

// manages the save menu UI
public class SaveDrone : MonoBehaviour
{
    PlayerController player;
    public SaveMenu saveMenu;
    int timer;

    private void Start()
    {
        player = PlayerController.instance;
        InvokeRepeating("Timer", .1f, 1f);
        timer = 2;
    }

    // if in range of save drone open save menu
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (GetComponent<Renderer>().bounds.Contains(player.transform.position) && timer > 2)
        {
            timer = 0;
            saveMenu.OpenSaveMenu();
        }
    }

    // keep track of how long since player has opened save menu to make smooth gameplay
    void Timer()
    {
        timer++;
    }

}
