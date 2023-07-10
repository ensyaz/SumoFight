using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndUI : MonoBehaviour
{
    [SerializeField] private GameStateSO gameStateSO;
    [SerializeField] private TextMeshProUGUI finalText;

    private void FixedUpdate()
    {
        if (gameStateSO.currentState == GameStateSO.State.GameWin)
            finalText.text = "YOU WON!";
        if (gameStateSO.currentState == GameStateSO.State.GameLost)
            finalText.text = "YOU LOST!";
    }
    
    public void OnTouchMainMenuButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
