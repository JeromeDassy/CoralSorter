using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private GameObject warningCard;

    private int cardCount;
    private int matchStreak = 0;
    private bool foundWarningCard = false;
    private readonly List<Card> flippedCards = new();

    private SoundManager _soundManager;
    private ScoreManager _scoreManager;
    private TimeManager _timeManager;
    private MenuManager _menuManager;
    private GridManager _gridManager;

    private bool isPaused = false;
    public bool IsPaused
    {
        get => isPaused;
        set
        {
            isPaused = value;
            OnPauseChanged(isPaused);
        }
    }

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        _soundManager = SoundManager.Instance;
        _scoreManager = ScoreManager.Instance;
        _timeManager = TimeManager.Instance;
        _menuManager = MenuManager.Instance;
        _gridManager = GridManager.Instance;
    }

    public void StartPreset(Text preset)
    {
        // Example text input: "2x2 ; 2x3 ; 3x3 ; ..."
        string presetText = preset.text;
        string[] entries = presetText.Split(';');

        foreach (string entry in entries)
        {
            string trimmedEntry = entry.Trim();
            if (string.IsNullOrEmpty(trimmedEntry)) continue;

            string[] dimensions = trimmedEntry.Split('x');
            if (dimensions.Length == 2 &&
                int.TryParse(dimensions[0], out int x) &&
                int.TryParse(dimensions[1], out int y))
            {
                StartGrid(x, y);
            }
            else
            {
                Debug.LogWarning($"Invalid entry format: {trimmedEntry}");
            }
        }
    }

    public void StartGrid(int x, int y)
    {
        ResetGame();

        _gridManager.StartGameWithGrid(x, y);
        SetCardCount(x * y);

        _menuManager.ShowHideMainMenu(false);
    }

    public void CardFlipped(Card card)
    {
        if (card.UniqueId == int.MinValue)
        {
            _soundManager.DeathCardSound();

            if (foundWarningCard)
            {
                GameOver();
                return;
            }

            warningCard.SetActive(true);
            foundWarningCard = true;
            matchStreak = 0;
        }

        flippedCards.Add(card);

        if (flippedCards.Count > 2)
        {
            ResetNonMatchingCard(0);
            return;
        }

        if (flippedCards.Count == 2)
        {
            CheckMatch();
        }
    }

    public void PlayPauseGame(bool pause)
    {
        IsPaused = pause;
    }

    public void GameOver()
    {
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

    private void OnPauseChanged(bool pause)
    {

    }

    private void ResetGame()
    {
        warningCard.SetActive(false);
        flippedCards.Clear();
        _scoreManager.ResetScore();
        IsPaused = false; // Ensure the game is not paused at the start
        _gridManager.ResetGrid();
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
            matchStreak = 0;
            ResetNonMatchingCard(1);
            _soundManager.PlayMismatchSound();
            return;
        }

        Matching();
    }

    private void Matching()
    {
        matchStreak++;
        _scoreManager.UpdateScore(matchStreak);
        _soundManager.PlayMatchSound();

        flippedCards[0].HideCard();
        flippedCards[1].HideCard();
        RemoveCard(flippedCards[1]);
        RemoveCard(flippedCards[0]);

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

        _timeManager.StopCountdown(out int timeLeft);
        _scoreManager.FinalScoreUpdate(timeLeft);

        _soundManager.PlayWinSound();
        _menuManager.ShowHideEndLevelMenu(true);
    }

    private void ResetNonMatchingCard(int resetDelay)
    {
        flippedCards[0].ResetCard(resetDelay);
        flippedCards[1].ResetCard(resetDelay);
        RemoveCard(flippedCards[1]);
        RemoveCard(flippedCards[0]);
    }

    private void RemoveCard(Card card)
    {
        flippedCards.Remove(card);
    }
}