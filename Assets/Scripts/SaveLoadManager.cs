using UnityEngine;
using System.Collections.Generic;

public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager Instance;

    private const string ScoreKey = "Score";
    private const string TimeKey = "Time";
    private const string GridDataKey = "GridData";
    private const string GridXKey = "GridX";
    private const string GridYKey = "GridY";

    private ScoreManager _scoreManager;
    private GridManager _gridManager;
    private GameManager _gameManager;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        _scoreManager = ScoreManager.Instance;
        _gridManager = GridManager.Instance;
        _gameManager = GameManager.Instance;
    }

    public void SaveGame()
    {
        PlayerPrefs.SetInt(ScoreKey, _gameManager.GetScore());

        PlayerPrefs.SetInt(TimeKey, _gameManager.GetRemainingTime());

        var gridDimensions = _gridManager.GetGridDimensions();
        PlayerPrefs.SetInt(GridXKey, (int)gridDimensions.x);
        PlayerPrefs.SetInt(GridYKey, (int)gridDimensions.y);

        List<CardData> cardDataList = _gameManager.GetGridState();
        string gridData = JsonUtility.ToJson(new Serialization<List<CardData>>(cardDataList));
        PlayerPrefs.SetString(GridDataKey, gridData);

        PlayerPrefs.Save();

        Debug.Log("Game Saved");
    }

    public void LoadGame()
    {
        // Load and set the score if saved data exists
        if (PlayerPrefs.HasKey(ScoreKey))
        {
            int score = PlayerPrefs.GetInt(ScoreKey);

            // Load and set the remaining time
            int remainingTime = PlayerPrefs.GetInt(TimeKey);

            // Load and set the grid dimensions
            int gridX = PlayerPrefs.GetInt(GridXKey);
            int gridY = PlayerPrefs.GetInt(GridYKey);

            // Load and set the grid state
            string gridData = PlayerPrefs.GetString(GridDataKey);
            List<CardData> cardDataList = JsonUtility.FromJson<Serialization<List<CardData>>>(gridData).data;

            // Set the loaded game state
            _gameManager.SetGameState(score, remainingTime, cardDataList, gridX, gridY);

            Debug.Log("Game Loaded");
        }
    }

    // Helper class for serializing and deserializing lists with Unity's JsonUtility
    [System.Serializable]
    public class Serialization<T>
    {
        public T data;
        public Serialization(T data)
        {
            this.data = data;
        }
    }
}
