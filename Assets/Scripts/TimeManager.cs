using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;

    [SerializeField] private Text timerText;

    private int countdownTime;
    private Coroutine countdownRoutine;
    private GameManager _gameManager;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        _gameManager = GameManager.Instance;
    }

    public void StartCountdown(int cardCount)
    {
        countdownTime = cardCount * 5;
        StartCountdownRoutine();
    }

    public void StopCountdown(out int timeLeft)
    {
        StopCountdownRoutine();
        timeLeft = GetCountdownTime();
    }

    public int GetCountdownTime()
    {
        return countdownTime;
    }

    public void SetCountdownTime(int time)
    {
        countdownTime = time;
        UpdateTimerText();
        StartCountdownRoutine();
    }

    public void ResetCountdown()
    {
        StopCountdownRoutine();
        countdownTime = 0;
        UpdateTimerText();
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
        while (countdownTime > 0)
        {
            if (!_gameManager.IsPaused)
            {
                UpdateTimerText();
                countdownTime--;
            }
            yield return new WaitForSeconds(1f);
        }

        _gameManager.GameOver();
    }

    private void UpdateTimerText()
    {
        timerText.text = $"Time: {countdownTime}";
    }
}