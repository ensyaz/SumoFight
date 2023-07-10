using TMPro;
using UnityEngine;

public class IngameUI : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float countDown;
    [Header("ScriptableObjects")]
    [SerializeField] private GameModeSO gameModeSO;
    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI countDownText;
    [Header("UI")]
    [SerializeField] private GameObject settingsUI;

    // Update is called once per frame
    void Update()
    {
        SetLevelTimeCounter();
    }
    // Countdown
    private void SetLevelTimeCounter()
    {
        if (countDown > 0)
        {
            countDown -= Time.deltaTime;
            DisplayTime(countDown);
        }
        else
        {
            countDown = 0;
            gameModeSO.RaiseOnGameEnd();
            gameModeSO.RaiseOnGameLost();
        }
    }
    // Displays countdown
    private void DisplayTime(float timeToDisplay)
    {
        if (timeToDisplay < 0)
        {
            timeToDisplay = 0;
        }

        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        countDownText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
    // Connected to Settings Button
    public void OnTouchSettingsButton()
    {
        settingsUI.SetActive(true);
    }
}
