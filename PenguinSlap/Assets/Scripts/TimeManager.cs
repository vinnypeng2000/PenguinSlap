using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TimeManager : MonoBehaviour
{
    [SerializeField] private float startTime = 210f; // 3 min 30 sec (editable in the Inspector)
    [SerializeField] private Slider timeSlider;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private GameObject gameOverScreen; // UI that pops up when time reaches 0
    [SerializeField] private GameObject player; // Reference to player object to disable movement

    private float currentTime;
    private bool isTimerRunning = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentTime = startTime;
        timeSlider.maxValue = startTime;
        timeSlider.value = startTime;
        gameOverScreen.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (isTimerRunning)
        {
            currentTime -= Time.deltaTime;
            if (currentTime <= 0)
            {
                currentTime = 0;
                EndTimer();
            }

            UpdateUI();
        }
    }

    void UpdateUI()
    {
        // Update slider value
        timeSlider.value = currentTime;

        // Convert time to MM:SS format
        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);
        timerText.text = string.Format("Time Remaining: {0:00}:{1:00}", minutes, seconds);
    }

    void EndTimer()
    {
        isTimerRunning = false;
        gameOverScreen.SetActive(true);

        // Disable player movement (assuming there's a script with a method for this)
        if (player.TryGetComponent<PlayerController>(out var movementScript))
        {
            movementScript.enabled = false;
        }
    }
}
