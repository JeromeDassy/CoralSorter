using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [SerializeField] private Text scoreText;
    [SerializeField] private GameObject multiplierTextPrefab; // Prefab for the multiplier text

    private int score = 0;
    private const int PoolSize = 2;
    private Queue<GameObject> textPool = new Queue<GameObject>();

    void Awake()
    {
        Instance = this;

        InitializeTextPool();
    }

    private void InitializeTextPool()
    {
        for (int i = 0; i < PoolSize; i++)
        {
            GameObject textObj = Instantiate(multiplierTextPrefab, scoreText.transform);
            textObj.SetActive(false);
            textPool.Enqueue(textObj);
        }
    }

    private GameObject GetPooledTextObject()
    {
        if (textPool.Count > 0)
        {
            GameObject textObj = textPool.Dequeue();
            textObj.SetActive(true);
            return textObj;
        }
        else
        {
            return Instantiate(multiplierTextPrefab, scoreText.transform);
        }
    }

    private void ReturnTextObjectToPool(GameObject textObj)
    {
        textObj.SetActive(false);
        textObj.transform.localScale = Vector3.one;
        Text textComponent = textObj.GetComponent<Text>();
        Color color = textComponent.color;
        color.a = 1f;
        textComponent.color = color;
        textPool.Enqueue(textObj);
    }

    public void UpdateScore(int multiplier)
    {
        score += 10 * multiplier;
        scoreText.text = $"Score: {score}";

        // Get a multiplier text object from the pool and animate it
        GameObject multiplierTextObj = GetPooledTextObject();
        Text multiplierText = multiplierTextObj.GetComponent<Text>();
        multiplierText.text = $"x{multiplier}";
        multiplierTextObj.transform.position = scoreText.transform.position;

        StartCoroutine(AnimateMultiplierText(multiplierTextObj, multiplier));
    }

    private IEnumerator AnimateMultiplierText(GameObject textObj, int multiplier)
    {
        float duration = 1.5f;
        float elapsed = 0f;
        Vector3 startScale = Vector3.one * 0.5f * multiplier;
        Vector3 targetScale = Vector3.one * 0.25f;
        Vector3 startPosition = textObj.transform.position;
        Vector3 targetPosition = startPosition + new Vector3(0, -30, 0); // Adjust the y-value as needed

        Text textComponent = textObj.GetComponent<Text>();
        Color originalColor = textComponent.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // Fade out alpha
            Color color = originalColor;
            color.a = Mathf.Lerp(1f, 0f, t);
            textComponent.color = color;

            // Scale down
            textObj.transform.localScale = Vector3.Lerp(startScale, targetScale, t);

            // Move down
            textObj.transform.position = Vector3.Lerp(startPosition, targetPosition, t);

            yield return null;
        }

        ReturnTextObjectToPool(textObj);
    }

    public void FinalScoreUpdate(int addScore)
    {
        score += addScore;
        scoreText.text = $"Score: {score}";
    }

    public int GetCurrentScore()
    {
        return score;
    }

    public void SetScore(int value)
    {
        score = value;
        scoreText.text = $"Score: {score}";
    }

    public void ResetScore()
    {
        score = 0;
        scoreText.text = $"Score: {score}";
    }
}
