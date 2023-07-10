using System;
using UnityEngine;

public class CameraOnOff : MonoBehaviour
{
    [SerializeField] GameModeSO gameModeSO;
    [SerializeField] GameObject mainVC;

    private void OnEnable()
    {
        gameModeSO.OnGameEnd += OnGameEnd;
    }

    private void OnDisable()
    {
        gameModeSO.OnGameEnd -= OnGameEnd;
    }

    private void OnGameEnd()
    {
        mainVC.SetActive(false);
    }
}
