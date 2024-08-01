using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager Instance;
    
    private const string ScoreKey = "Score";
    
    private ScoreManager _scoreManager;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        _scoreManager = ScoreManager.Instance;
    }

    public void SaveGame()
    {
        PlayerPrefs.SetInt(ScoreKey, _scoreManager.GetCurrentScore());
        PlayerPrefs.Save();
    }

    public void LoadGame()
    {
        if (PlayerPrefs.HasKey(ScoreKey))
        {
            _scoreManager.SetScore(PlayerPrefs.GetInt(ScoreKey));
        }
    }
}
