using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [SerializeField] private Text scoreText;
    private int score = 0;

    void Awake()
    {
        Instance = this;
    }

    public void UpdateScore(int multiplier)
    {
        int points = 10 * multiplier;
        score += points;
        scoreText.text = "Score: " + score;
    }

    public int GetCurrentScore()
    {
        return score;
    }

    public void ResetScore()
    {
        score = 0;
        scoreText.text = "Score: " + score;
    }
}
