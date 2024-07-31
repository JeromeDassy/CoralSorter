using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private GameObject warningCard;

    private int cardCount;
    private int matchStreak = 0;
    private bool foundWarningCard = false;
    private readonly List<Card> flippedCards = new();

    private SoundManager _soundManager;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        _soundManager = SoundManager.Instance;
    }

    public void SetCardCount(int count)
    {
        cardCount = count;
        TimeManager.Instance.StartCountdown(count);
    }

    public void CardFlipped(Card card)
    {
        if(card.UniqueId == int.MinValue)
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
        ScoreManager.Instance.UpdateScore(matchStreak);
        _soundManager.PlayMatchSound();
        flippedCards[0].HideCard();
        flippedCards[1].HideCard();

        cardCount -= 2;

        CheckWin();
    }

    private void CheckWin()
    {
        if(cardCount < 2)
        {
            LevelCompleted();
        }
    }

    private void LevelCompleted()
    {
        Debug.Log("YOU WON!");

        TimeManager.Instance.StopCountdown(out int timeLeft);
        ScoreManager.Instance.FinalScoreUpdate(timeLeft);

        _soundManager.PlayWinSound();
    }

    public void GameOver()
    {
        Debug.Log("GAME OVER!");
        _soundManager.PlayGameOverSound();
    }

    private void ResetNonMatchingCard(int resetDelay)
    {
        flippedCards[0].ResetCard(resetDelay);
        flippedCards[1].ResetCard(resetDelay);
    }

    public void RemoveCard(Card card)
    {
        flippedCards.Remove(card);
    }
}
