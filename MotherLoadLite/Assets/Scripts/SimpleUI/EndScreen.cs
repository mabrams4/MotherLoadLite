using UnityEngine;
using UnityEngine.SceneManagement;

/* contains functions for quitting the game and loading the main menu. Need a seperate script for this because
 * the End Screen is a different scene from the game and cannot access the pause menu functions 
 */

public class EndScreen : MonoBehaviour
{
    // load the main menu
    public void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    // quit the game
    public void Quit()
    {
        Application.Quit();
    }
}
