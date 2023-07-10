using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "ScriptableObject/Game Mode SO", fileName = "GameModeSO")]
public class GameModeSO : ScriptableObject
{
    public UnityAction OnGamePrepare;
    public UnityAction OnGameStart;
    public UnityAction OnGameWin;
    public UnityAction OnGameLost;
    public UnityAction OnGameEnd;

    public void RaiseOnGamePrepare()
    {
        OnGamePrepare?.Invoke();
        Debug.Log("RaiseOnGamePrepare");
    }

    public void RaiseOnGameStart()
    {
        OnGameStart?.Invoke();
        Debug.Log("RaiseOnGameStart");
    }

    public void RaiseOnGameWin()
    {
        OnGameWin?.Invoke();
        Debug.Log("RaiseOnGameWin");
    }

    public void RaiseOnGameLost()
    {
        OnGameLost?.Invoke();
        Debug.Log("RaiseOnGameLost");
    }

    public void RaiseOnGameEnd()
    {
        OnGameEnd?.Invoke();
        Debug.Log("RaiseOnGameEnd");
    }

}
