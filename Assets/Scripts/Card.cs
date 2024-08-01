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

    private SoundManager _soundManager;
    private GameManager _gameManager;

    void Start()
    {
        cardImage = GetComponent<Image>();
        cardButton = GetComponent<Button>();
        cardButton.onClick.AddListener(OnCardClicked);
        cardImage.sprite = cardBack;

        _soundManager = SoundManager.Instance;
        _gameManager = GameManager.Instance;
    }

    public void SetCardData(CardData data)
    {
        cardFront = data.image;
        UniqueId = data.uniqueId;
    }

    public void OnCardClicked()
    {
        if (isFlipped) return;

        StartCoroutine(AnimFlipCardFront());
    }

    public void FlipCardBack(int delay)
    {
        CheckRunningBackRoutine();
        flipCardBackRoutine = StartCoroutine(AnimFlipCardBack(delay));
    }

    public void ResetCard()
    {
        isFlipped = false;
        cardButton.interactable = true;
        cardImage.sprite = cardBack;
        transform.localScale = Vector3.one;
        transform.rotation = Quaternion.identity;
    }

    public void DisableCard()
    {
        CheckRunningBackRoutine();
        cardButton.interactable = false;
        _gameManager.RemoveCard(this);
    }

    private void CheckRunningBackRoutine()
    {
        if (flipCardBackRoutine != null)
        {
            StopCoroutine(flipCardBackRoutine);
            flipCardBackRoutine = null;
        }
    }

    private IEnumerator AnimFlipCardFront()
    {
        _soundManager.PlayFlipSound();

        yield return StartCoroutine(AnimFlipCardRotation(Vector3.zero, Vector3.up * 90, cardFront));
        transform.localScale = new Vector3(-1, 1, 1);
        yield return StartCoroutine(AnimFlipCardRotation(Vector3.up * 90, Vector3.up * 180));

        FlipCardBack(5);
        isFlipped = true;
        _gameManager.CardFlipped(this);
    }

    private IEnumerator AnimFlipCardBack(int delay)
    {
        float elapsedTime = 0f;
        while (elapsedTime < delay)
        {
            if (GameManager.Instance.IsPaused)
            {
                yield return null;
            }
            else
            {
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }

        _gameManager.RemoveCard(this);
        _soundManager.PlayFlipSound();

        yield return StartCoroutine(AnimFlipCardRotation(Vector3.up * 180, Vector3.up * 90, cardBack));
        transform.localScale = Vector3.one;
        yield return StartCoroutine(AnimFlipCardRotation(Vector3.up * 90, Vector3.zero));

        isFlipped = false;
    }


    private IEnumerator AnimFlipCardRotation(Vector3 fromRotation, Vector3 toRotation, Sprite newSprite = null)
    {
        float time = 0.2f;
        float halfTime = time / 2f;
        float elapsedTime = 0f;

        while (elapsedTime < halfTime)
        {
            if (!_gameManager.IsPaused)
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
