using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;

    [SerializeField] private Text timerText;
    [SerializeField] private int baseTime = 60;

    private int countdownTime;
    private Coroutine countdownRoutine;

    void Awake()
    {
        Instance = this;
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
        while (countdownTime > 0)
        {
            if (!GameManager.Instance.IsPaused)
            {
                UpdateTimerText();
                countdownTime--;
            }
            yield return new WaitForSeconds(1f);
        }

        GameManager.Instance.GameOver();
    }

    private void UpdateTimerText()
    {
        timerText.text = $"Time: {countdownTime}";
    }
}
