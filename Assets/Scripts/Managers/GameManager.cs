using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameModeSO gameModeSO;
    [SerializeField] private GameStateSO gameStateSO;

    private void OnEnable()
    {
        gameModeSO.OnGamePrepare += OnGamePrepare;
        gameModeSO.OnGameStart += OnGameStart;
        gameModeSO.OnGameWin += OnGameWin;
        gameModeSO.OnGameLost += OnGameLost;
        gameModeSO.OnGameEnd += OnGameEnd;
    }

    private void OnDisable()
    {
        gameModeSO.OnGamePrepare -= OnGamePrepare;
        gameModeSO.OnGameStart -= OnGameStart;
        gameModeSO.OnGameWin -= OnGameWin;
        gameModeSO.OnGameLost -= OnGameLost;
        gameModeSO.OnGameEnd -= OnGameEnd;
    }

    private void Start()
    {
        gameModeSO.RaiseOnGamePrepare();
    }

    private void OnGamePrepare()
    {
        gameStateSO.currentState = GameStateSO.State.GamePrepare;
    }

    private void OnGameStart()
    {
        gameStateSO.currentState = GameStateSO.State.GameStart;
    }

    private void OnGameWin()
    {
        gameStateSO.currentState = GameStateSO.State.GameWin;
    }

    private void OnGameLost()
    {
        gameStateSO.currentState = GameStateSO.State.GameLost;
    }

    private void OnGameEnd()
    {
        gameStateSO.currentState = GameStateSO.State.GameEnd;
    }
}
