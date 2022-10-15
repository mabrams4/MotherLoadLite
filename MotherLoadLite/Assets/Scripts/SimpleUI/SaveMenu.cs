using UnityEngine;

// manages save menu buttons
public class SaveMenu : MonoBehaviour
{
    public GameObject saveMenuUI;
    Inventory inventory;
    PlayerInfo player;
    MissionControl missionControl;
    RogueColony rogueColony;
    BabyWorm babyWorm;

    private void Start()
    {
        inventory = Inventory.instance;
        player = PlayerInfo.instance;
        missionControl = MissionControl.instance;
        rogueColony = RogueColony.instance;
        babyWorm = BabyWorm.instance;
    }

    // opens the save menu
    public void OpenSaveMenu()
    {
        saveMenuUI.SetActive(true);
        Time.timeScale = 0f;
    }

    // closes the save menu
    public void CloseSaveMenu()
    {
        saveMenuUI.SetActive(false);
        Time.timeScale = 1f;
    }

    // saves the game
    public void SaveGame()
    {
        SaveSystem.SaveGame(inventory, player, missionControl, rogueColony, babyWorm);
        CloseSaveMenu();
    }
}
