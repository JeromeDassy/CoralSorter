using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(Button))]
public class Card : MonoBehaviour
{
    [SerializeField] private Sprite cardBack;
    
    private bool isFlipped = false;

    private Sprite cardFront;
    private Image cardImage;
    private Button cardButton;
    private Coroutine flipCardBackRoutine;

    public int UniqueId { get; private set; }

    void Start()
    {
        cardImage = GetComponent<Image>();
        cardButton = GetComponent<Button>();
        cardButton.onClick.AddListener(OnCardClicked);
        cardImage.sprite = cardBack;
    }

    void OnEnable()
    {
        if (cardButton != null)
        {
            cardButton.interactable = true;
        }
    }

    public void SetCardData(CardData data)
    {
        cardFront = data.image;
        UniqueId = data.uniqueId;
    }

    public void OnCardClicked()
    {
        if (isFlipped || !cardButton.enabled) return;

        cardButton.enabled = false;
        StartCoroutine(FlipCard());
    }

    public void ResetCard(int delay)
    {
        //GameManager.Instance.RemoveCard(this);
        CheckRunningBackRoutine();
        flipCardBackRoutine = StartCoroutine(FlipCardBack(delay));
    }

    public void HideCard()
    {
        //GameManager.Instance.RemoveCard(this);
        CheckRunningBackRoutine();
        cardButton.interactable = false;
    }

    private void CheckRunningBackRoutine()
    {
        if (flipCardBackRoutine != null)
        {
            StopCoroutine(flipCardBackRoutine);
            flipCardBackRoutine = null;
        }
    }

    private IEnumerator FlipCard()
    {
        SoundManager.Instance.PlayFlipSound();

        yield return StartCoroutine(FlipCardRotation(Vector3.zero, Vector3.up * 90, cardFront));
        transform.localScale = new Vector3(-1, 1, 1);
        yield return StartCoroutine(FlipCardRotation(Vector3.up * 90, Vector3.up * 180));

        ResetCard(5);
        isFlipped = true;
        GameManager.Instance.CardFlipped(this);
    }

    private IEnumerator FlipCardBack(int delay)
    {
        yield return new WaitForSeconds(delay);

        while (GameManager.Instance.IsPaused)
        {
            yield return null;
        }

        SoundManager.Instance.PlayFlipSound();

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
            if (!GameManager.Instance.IsPaused)
            {
                transform.rotation = Quaternion.Slerp(Quaternion.Euler(fromRotation), Quaternion.Euler(toRotation), elapsedTime / halfTime);
                elapsedTime += Time.deltaTime;
            }
            yield return null;
        }

        transform.rotation = Quaternion.Euler(toRotation);
        if (newSprite != null)
        {
            cardImage.sprite = newSprite;
        }
    }
}
