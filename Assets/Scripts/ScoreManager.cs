using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [SerializeField] private Text scoreText;
    [SerializeField] private int baseScore = 10;
    [SerializeField] private int streakMultiplier = 10;

    private int score;

    void Awake()
    {
        Instance = this;
    }

    public void UpdateScore(int streak)
    {
        score += baseScore + (streakMultiplier * streak);
        UpdateScoreText();
    }

    public void FinalScoreUpdate(int timeLeft)
    {
        score += timeLeft * 10; // Adjust multiplier as needed
        UpdateScoreText();
    }

    public int GetCurrentScore()
    {
        return score;
    }

    public void SetScore(int value)
    {
        score = value;
        UpdateScoreText();
    }

    public void ResetScore()
    {
        score = 0;
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        scoreText.text = $"Score: {score}";
    }
}
