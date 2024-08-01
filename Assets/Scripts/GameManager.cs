using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameObject warningCard;

    private int cardCount;
    private int matchStreak;
    private bool foundWarningCard;
    private readonly List<Card> flippedCards = new();

    private SoundManager _soundManager;
    private ScoreManager _scoreManager;
    private TimeManager _timeManager;
    private MenuManager _menuManager;
    private GridManager _gridManager;

    private bool IsPlaying { get; set; }

    private bool isPaused;
    public bool IsPaused
    {
        get => isPaused;
        set
        {
            isPaused = value;
        }
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        _soundManager = SoundManager.Instance;
        _scoreManager = ScoreManager.Instance;
        _timeManager = TimeManager.Instance;
        _menuManager = MenuManager.Instance;
        _gridManager = GridManager.Instance;
    }

    #region Save&Load 
    public int GetScore()
    {
        return _scoreManager.GetCurrentScore();
    }

    public int GetRemainingTime()
    {
        return _timeManager.GetCountdownTime();
    }

    public List<CardData> GetGridState()
    {
        return _gridManager.GetCardDataList();
    }

    public void SetGameState(int score, int remainingTime, List<CardData> cardDataList, int x, int y)
    {
        IsPlaying = true;
        _scoreManager.SetScore(score);
        _gridManager.LoadGrid(cardDataList, x, y, out int matchedCard);
        SetCardCount((x * y) - matchedCard);
        _timeManager.SetCountdownTime(remainingTime);
        _menuManager.ShowHideMainMenu(false);
    }
    #endregion

    public void StartPreset(Text preset)
    {
        foreach (var entry in preset.text.Split(';'))
        {
            var dimensions = entry.Trim().Split('x');
            if (dimensions.Length == 2 &&
                int.TryParse(dimensions[0], out int x) &&
                int.TryParse(dimensions[1], out int y))
            {
                StartGrid(x, y);
            }
            else
            {
                Debug.LogWarning($"Invalid entry format: {entry}");
            }
        }
    }

    public void StartGrid(int x, int y)
    {
        IsPlaying = true;
        _gridManager.StartGameWithGrid(x, y);
        SetCardCount(x * y);
        _menuManager.ShowHideMainMenu(false);
    }

    public void CardFlipped(Card card)
    {
        if (card.UniqueId == int.MinValue)
        {
            HandleWarningCard();
            return;
        }

        flippedCards.Add(card);

        if (flippedCards.Count > 2)
        {
            ResetFlippedCards(0);
        }
        
        if (flippedCards.Count == 2)
        {
            CheckMatch();
        }
    }

    private void HandleWarningCard()
    {
        if (foundWarningCard)
        {
            GameOver();
            return;
        }

        _soundManager.DeathCardSound();
        warningCard.SetActive(true);
        foundWarningCard = true;
        matchStreak = 0;
    }

    public void PlayPauseGame(bool pause)
    {
        IsPaused = pause;
    }

    public void GameOver()
    {
        if (!IsPlaying) return;

        IsPlaying = false;
        Debug.Log("GAME OVER!");
        _menuManager.ShowHideGameOverMenu(true);
        _soundManager.PlayGameOverSound();
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void ResetGame()
    {
        IsPlaying = false;
        IsPaused = false;
        foundWarningCard = false;
        warningCard.SetActive(false);
        flippedCards.Clear();
        _scoreManager.ResetScore();
        _timeManager.ResetCountdown();
        _gridManager.ResetGrid();
    }

    public void RemoveCard(Card card)
    {
        flippedCards.Remove(card);
    }

    private void SetCardCount(int count)
    {
        cardCount = count;
        _timeManager.StartCountdown(count);
    }

    private void CheckMatch()
    {
        if (flippedCards[0].UniqueId != flippedCards[1].UniqueId)
        {
            ResetFlippedCards(1);
            _soundManager.PlayMismatchSound();
            return;
        }

        MatchCards();
    }

    private void MatchCards()
    {
        matchStreak++;
        _scoreManager.UpdateScore(matchStreak);
        _soundManager.PlayMatchSound();

        flippedCards[0].DisableCard();
        flippedCards[0].DisableCard();

        cardCount -= 2;
        CheckWin();
    }

    private void CheckWin()
    {
        if (cardCount < 2)
        {
            LevelCompleted();
        }
    }

    private void LevelCompleted()
    {
        Debug.Log("YOU WON!");
        IsPlaying = false;
        _timeManager.StopCountdown(out int timeLeft);
        _scoreManager.FinalScoreUpdate(timeLeft);
        _soundManager.PlayWinSound();
        _menuManager.ShowHideEndLevelMenu(true);
    }

    private void ResetFlippedCards(int delay)
    {
        flippedCards[0].FlipCardBack(delay);
        flippedCards[0].FlipCardBack(delay);
    }
}
