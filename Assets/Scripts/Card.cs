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
        CheckRunningBackRoutine();
        flipCardBackRoutine = StartCoroutine(FlipCardBack(delay));
    }

    public void HideCard()
    {
        CheckRunningBackRoutine();
        cardImage.enabled = false;
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
        //Instead of an IEnumarator for every code animations I would normally use Dotween plugin but I followed you remark "avoid prebuilt frameworks or purchased assets"
        yield return StartCoroutine(FlipCardRotation(Vector3.zero, Vector3.up * 90, cardFront));
        transform.localScale = new Vector3(-1, 1, 1);
        yield return StartCoroutine(FlipCardRotation(Vector3.up * 90, Vector3.up * 180));

        ResetCard(5);
        isFlipped = true;
        GameManager.Instance.CardFlipped(this);
    }

    private IEnumerator FlipCardBack(int delay)
    {
        //Instead of an IEnumarator for every code animations I would normally use Dotween plugin but I followed you remark "avoid prebuilt frameworks or purchased assets"
        yield return new WaitForSeconds(delay);

        GameManager.Instance.RemoveCard(this);

        yield return StartCoroutine(FlipCardRotation(Vector3.up * 180, Vector3.up * 90, cardBack));
        transform.localScale = new Vector3(1, 1, 1);
        yield return StartCoroutine(FlipCardRotation(Vector3.up * 90, Vector3.zero));

        isFlipped = false;
        cardButton.enabled = true;
    }

    private IEnumerator FlipCardRotation(Vector3 fromRotation, Vector3 toRotation, Sprite newSprite = null)
    {
        //Instead of an IEnumarator for every code animations I would normally use Dotween plugin but I followed you remark "avoid prebuilt frameworks or purchased assets"
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
