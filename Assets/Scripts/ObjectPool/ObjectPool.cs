using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public int disabledFoodCount;
    public int inactiveAICount = 0;

    [Header("Settings")]
    [SerializeField] private int foodSpawnAmount;
    [SerializeField] private int playerSpawnAmount;
    [SerializeField] private int maxDisabledFoodCount;

    [Header("Prefabs")]
    [SerializeField] private GameObject foodPrefab;
    [SerializeField] private Transform groundTransform;

    [Header("ScriptableObjects")]
    [SerializeField] private GameModeSO gameModeSO;
    [SerializeField] private GameStateSO gameStateSO;

    [Header("Components")]
    [SerializeField] private Transform foodParent;
    [SerializeField] private GameObject[] AI;

    private Transform foodTransform;
    private GameObject[] foodArray;

    private void Awake()
    {
        foodTransform = foodPrefab.GetComponent<Transform>();
        foodArray = new GameObject[foodSpawnAmount];
    }

    private void OnEnable()
    {
        gameModeSO.OnGamePrepare += OnGamePrepare;
        gameModeSO.OnGameStart += OnGameStart;
        gameModeSO.OnGameEnd += OnGameEnd;
    }

    private void OnDisable()
    {
        gameModeSO.OnGamePrepare -= OnGamePrepare;
        gameModeSO.OnGameStart -= OnGameStart;
        gameModeSO.OnGameEnd -= OnGameEnd;
    }

    private void OnGamePrepare()
    {
        InitFoods();
    }

    private void OnGameStart()
    {
        SetFoods(true);
        SetAI();
    }

    private void OnGameEnd()
    {
        SetFoods(false);
    }

    private void FixedUpdate()
    {
        if (gameStateSO.currentState == GameStateSO.State.GameStart)
        {
            SetEnableDisabledFood();

            if (inactiveAICount == AI.Length)
            {
                gameModeSO.RaiseOnGameEnd();
                gameModeSO.RaiseOnGameWin();
            }
                
        }
    }
    // Keep enable when disabled food's count more than maxDisabledFoodCount
    private void SetEnableDisabledFood()
    {
        if (disabledFoodCount > maxDisabledFoodCount)
        {
            for (int i = 0; i < foodArray.Length ; i++)
            {
                if (!foodArray[i].activeSelf)
                {
                    foodArray[i].transform.position = GetRandomLocation();
                    foodArray[i].SetActive(true);
                }
            }
            disabledFoodCount = 0;
        }
    }
    // Instantiate food prefab and sets its transform
    private void InitFoods()
    {
        for (int i = 0; i < foodArray.Length; i++)
        {
            foodArray[i] = Instantiate(foodPrefab, GetRandomLocation(), Quaternion.identity, foodParent);
            foodArray[i].SetActive(false);
        }
    }
    // Sets AI when game starts
    private void SetAI()
    {
        for (int i = 0; i < AI.Length; i++)
        {
            AI[i].SetActive(true);
        }
    }

    private void SetFoods(bool state)
    {
        for (int i = 0; i < foodArray.Length; i++)
        {
            foodArray[i].SetActive(state);
        }
    }

    private Vector3 GetRandomLocation()
    {
        Vector3 randomLocation = (UnityEngine.Random.insideUnitCircle * (groundTransform.localScale.x - 3) / 2);

        randomLocation.z = randomLocation.y;
        randomLocation.y = foodTransform.position.y;

        return randomLocation;
    }    
}
