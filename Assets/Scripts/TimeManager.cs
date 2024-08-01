using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;

    [SerializeField] private Text timerText;
    [SerializeField] private int baseTime = 10;

    private int countdownTime;
    private Coroutine countdownRoutine;
    private GameManager _gameManager;

    void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _gameManager = GameManager.Instance;
    }

    public void StartCountdown(int cardCount)
    {
        countdownTime = baseTime + (cardCount * 5); // Adjust formula as needed
        StartCountdownRoutine();
    }

    public void StopCountdown(out int timeLeft)
    {
        StopCountdownRoutine();
        timeLeft = countdownTime;
    }

    private void StartCountdownRoutine()
    {
        countdownRoutine = StartCoroutine(Countdown());
    }

    private void StopCountdownRoutine()
    {
        if (countdownRoutine != null)
        {
            StopCoroutine(countdownRoutine);
            countdownRoutine = null;
        }
    }

    private IEnumerator Countdown()
    {
        float startTime = Time.time;
        float targetTime = startTime + countdownTime;

        while (countdownTime > 0)
        {
            if (!_gameManager.IsPaused)
            {
                float remainingTime = targetTime - Time.time;
                countdownTime = Mathf.Max(0, Mathf.CeilToInt(remainingTime));
                UpdateTimerText();
            }

            yield return null;
        }

        _gameManager.GameOver();
    }


    private void UpdateTimerText()
    {
        timerText.text = $"Time: {countdownTime}";
    }
}
