using UnityEngine;

public class StartUI : MonoBehaviour
{
    [Header("ScriptableObjects")]
    [SerializeField] private GameModeSO gameModeSO;
    // Connected to Start Button
    public void OnTouchStartButton()
    {
        gameModeSO.RaiseOnGameStart();
    }
    // Connected to Quit Button
    public void OnTouchQuitButton()
    {
        Application.Quit();
    }
}
