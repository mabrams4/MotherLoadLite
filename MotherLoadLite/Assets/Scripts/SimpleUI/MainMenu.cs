using UnityEngine;
using UnityEngine.SceneManagement;

// manages main menu buttons
public class MainMenu : MonoBehaviour
{
    // start a new game
    public void PlayNewGame()
    {
        SaveSystem.isNewGame = true;
        SceneManager.LoadScene(1);
    }

    // load a saved game
    public void LoadGame()
    {
        SaveSystem.isNewGame = false;
        SceneManager.LoadScene(1);
    }

    // quit the game
    public void QuitGame()
    {
        Application.Quit();
    }
}
