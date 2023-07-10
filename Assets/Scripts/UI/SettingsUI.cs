using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsUI : MonoBehaviour
{
    // Pauses the game
    private void OnEnable()
    {
        Time.timeScale = 0;
    }
    // Connected to Settings's Main Menu Button
    public void OnTouchMainMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    // Connected to Settings's Game Quit Button
    public void OnTouchQuitButton()
    {
        Application.Quit();
    }
    // Connected to Settings's Close Button
    public void OnTouchCloseSettings()
    {
        this.gameObject.SetActive(false);
    }
    // Resumes the game
    private void OnDisable()
    {
        Time.timeScale = 1;
    }
}
