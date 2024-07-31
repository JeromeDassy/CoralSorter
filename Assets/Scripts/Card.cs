using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(Button))]
public class Card : MonoBehaviour
{
    public Sprite cardBack;
    
    private Sprite cardFront;
    private int uniqueId;
    private bool isFlipped = false;
    private Image cardImage;
    private Button cardButton;

    private Coroutine flipCardBackRoutine;

    void Start()
    {
        cardImage = GetComponent<Image>();
        cardButton = GetComponent<Button>();
        cardButton.onClick.AddListener(OnCardClicked);
        cardImage.sprite = cardBack;  // Ensure card starts with back image
    }

    public void SetCardData(CardData data)
    {
        cardFront = data.image;
        uniqueId = data.uniqueId;
    }

    public void OnCardClicked()
    {
        if (isFlipped || !cardButton.enabled) return;

        cardButton.enabled = false;
        StartCoroutine(FlipCard());
    }

    public void ResetCard(int delay)
    {
        if (flipCardBackRoutine != null)
        {
            StopCoroutine(flipCardBackRoutine);
            flipCardBackRoutine = null;
        }
        flipCardBackRoutine = StartCoroutine(FlipCardBack(delay));
    }

    public void HideCard()
    {
        if (flipCardBackRoutine != null)
        {
            StopCoroutine(flipCardBackRoutine);
            flipCardBackRoutine = null;
        }
        cardImage.enabled = false;
    }

    public int GetUniqueId()
    {
        return uniqueId;
    }

    private IEnumerator FlipCard()
    {
        yield return StartCoroutine(FlipCardRotation(Vector3.zero, Vector3.up * 90, cardFront));
        transform.localScale = new Vector3(-1, 1, 1);
        yield return StartCoroutine(FlipCardRotation(Vector3.up * 90, Vector3.up * 180));

        ResetCard(5);
        isFlipped = true;
    }

    private IEnumerator FlipCardBack(int delay)
    {
        yield return new WaitForSeconds(delay);

        yield return StartCoroutine(FlipCardRotation(Vector3.up * 180, Vector3.up * 90, cardBack));
        transform.localScale = new Vector3(1, 1, 1);
        yield return StartCoroutine(FlipCardRotation(Vector3.up * 90, Vector3.zero));

        isFlipped = false;
        cardButton.enabled = true;
    }

    private IEnumerator FlipCardRotation(Vector3 fromRotation, Vector3 toRotation, Sprite newSprite = null)
    {
        float time = 0.2f;
        float halfTime = time / 2f;
        float elapsedTime = 0f;

        while (elapsedTime < halfTime)
        {
            transform.rotation = Quaternion.Slerp(Quaternion.Euler(fromRotation), Quaternion.Euler(toRotation), (elapsedTime / halfTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.rotation = Quaternion.Euler(toRotation);

        if (newSprite != null)
            cardImage.sprite = newSprite;
    }
}
