using UnityEngine;
using UnityEngine.SceneManagement;

// manages the pause menu and its buttons
public class PauseMenu : MonoBehaviour
{
    public static bool gameIsPasued;
    public GameObject pauseMenuUI;
    public GameObject saveMenuUI;
    public GameObject gameUI;

    void Update()
    {
        // shortcut to open pause menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gameIsPasued) Resume();
            else { Pause(); }
        }
    }

    // resumes the game
    public void Resume()
    {
        gameUI.SetActive(true);
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        gameIsPasued = false;
    }

    // pauses the game
    void Pause()
    {
        gameUI.SetActive(false);
        saveMenuUI.SetActive(false);
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        gameIsPasued = true;
    }

    // loads main menu
    public void LoadMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    // quits the game
    public void QuitGame()
    {
        Application.Quit();
    }
}
