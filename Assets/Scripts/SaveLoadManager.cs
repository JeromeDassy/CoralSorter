using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{
    private const string ScoreKey = "Score";

    public void SaveGame()
    {
        PlayerPrefs.SetInt(ScoreKey, ScoreManager.Instance.GetCurrentScore());
        PlayerPrefs.Save();
    }

    public void LoadGame()
    {
        if (PlayerPrefs.HasKey(ScoreKey))
        {
            ScoreManager.Instance.SetScore(PlayerPrefs.GetInt(ScoreKey));
        }
    }
}
