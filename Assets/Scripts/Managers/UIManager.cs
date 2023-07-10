using System;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject startUI;
    [SerializeField] private GameObject ingameUI;
    [SerializeField] private GameObject endUI;
    [SerializeField] private GameObject settingsUI;
    [Header("ScriptableObjects")]
    [SerializeField] private GameModeSO gameModeSO;

    private void OnEnable()
    {
        gameModeSO.OnGamePrepare += OnGamePrepare;
        gameModeSO.OnGameStart += OnGameStart;
        gameModeSO.OnGameWin += OnGameWin;
        gameModeSO.OnGameLost += OnGameLost;
    }

    private void OnDisable()
    {
        gameModeSO.OnGamePrepare -= OnGamePrepare;
        gameModeSO.OnGameStart -= OnGameStart;
        gameModeSO.OnGameWin -= OnGameWin;
        gameModeSO.OnGameLost -= OnGameLost;
    }

    private void OnGamePrepare()
    {
        settingsUI.SetActive(false);
        endUI.SetActive(false);
        ingameUI.SetActive(false);
        startUI.SetActive(true);
    }

    private void OnGameStart()
    {
        startUI.SetActive(false);
        ingameUI.SetActive(true);
    }

    private void OnGameWin()
    {
        ingameUI.SetActive(false);
        endUI.SetActive(true);
    }

    private void OnGameLost()
    {
        ingameUI.SetActive(false);
        endUI.SetActive(true);
    }
}
