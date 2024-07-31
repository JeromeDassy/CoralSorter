using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;

    public int secondsPerCard = 3;

    [SerializeField] private Text countdownText;
    private Coroutine countdownCoroutine;
    private int currentTime;

    void Awake()
    {
        Instance = this;
    }

    public void StartCountdown(int multiplier)
    {
        if (countdownCoroutine != null)
        {
            StopCoroutine(countdownCoroutine);
        }

        currentTime = secondsPerCard * multiplier;
        countdownCoroutine = StartCoroutine(Countdown());
    }

    private IEnumerator Countdown()
    {
        WaitForSeconds wait = new WaitForSeconds(1);

        while (currentTime > 0)
        {
            countdownText.text = $"Time left: {currentTime}";
            yield return wait;
            currentTime--;
        }

        countdownText.text = "Time's up!";
        OnCountdownEnd();
    }

    private void OnCountdownEnd()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }
    }

    public void StopCountdown(out int timeLeft)
    {
        if (countdownCoroutine != null)
        {
            StopCoroutine(countdownCoroutine);
            countdownCoroutine = null;
        }
        timeLeft = currentTime;
    }

    public void ResetCountdown()
    {
        StopCountdown(out _);
        countdownText.text = "Time left: " + currentTime;
    }
}
