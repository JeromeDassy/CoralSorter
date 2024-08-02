using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Unity.VisualScripting;

[RequireComponent(typeof(GridLayoutGroup))]
public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private int rows = 3;
    [SerializeField] private int columns = 4;
    [SerializeField] private float spacing = 20f;
    [SerializeField] private Sprite oddCard;

    [Header("Select Card Graphics folder (Must Be Under Resources)")]
    public FolderPath selectedFolder;

    private List<Sprite> cardImages;
    private List<CardData> shuffledCardData;
    private GridLayoutGroup gridLayoutGroup;
    private readonly Queue<GameObject> cardPool = new Queue<GameObject>();
    private readonly int poolSize = 30;

    void Awake()
    {
        Instance = this;
        gridLayoutGroup = GetComponent<GridLayoutGroup>();
        LoadCardImages();
        InitializeCardPool();
    }

    #region Save&Load
    public Vector2 GetGridDimensions()
    {
        return new Vector2(rows, columns);
    }

    public List<CardData> GetCardDataList()
    {
        List<CardData> cardDataList = new List<CardData>();

        int gridSize = rows * columns;
        int i = 0;
        foreach (Transform child in transform)
        {
            if(i == gridSize)
            {
                break;
            }

            Card card = child.GetComponent<Card>();
            if (card != null)
            {
                cardDataList.Add(new CardData(card.GetCardFront(), card.UniqueId, card.IsMatched));
            }
            i++;
        }
        return cardDataList;
    }

    public void LoadGrid(List<CardData> cardDataList, int x, int y, out int matchedCard)
    {
        ResetGrid();
        rows = x;
        columns = y;
        SetGridLayout();

        int matched = 0;
        foreach (var cardData in cardDataList)
        {
            GameObject cardObject = GetPooledCard();
            Card card = cardObject.GetComponent<Card>();
            card.SetCardData(cardData);
            card.SetMatched(cardData.isMatched);
            if (cardData.isMatched)
            {
                matched++;
            }
        }
        matchedCard = matched;
    }
    #endregion

    public void StartGameWithGrid(int x, int y)
    {
        rows = x;
        columns = y;
        SetGridLayout();
        ShuffleAndAssignImages();
        PlaceCards();
    }

    public void ResetGrid()
    {
        shuffledCardData?.Clear();
                
        foreach (Transform child in transform)
        {
            if (child.gameObject.activeInHierarchy)
            {
                ReturnCardToPool(child.gameObject);
            }
        }
    }

    private void InitializeCardPool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject cardObj = Instantiate(cardPrefab, transform);
            cardObj.SetActive(false);
            cardPool.Enqueue(cardObj);
            cardObj.name += $"_{i}";
        }
    }

    private GameObject GetPooledCard()
    {
        if (cardPool.Count > 0)
        {
            GameObject card = cardPool.Dequeue();
            card.SetActive(true);
            return card;
        }
        return Instantiate(cardPrefab, transform);
    }

    private void ReturnCardToPool(GameObject go)
    {
        Card card = go.GetComponent<Card>();
        card.ResetCard();
        go.SetActive(false);
        cardPool.Enqueue(go);
    }

    private void LoadCardImages()
    {
        string folderPath = string.IsNullOrEmpty(selectedFolder.path) ? "Cards/FrontGraphics" : selectedFolder.path;
        cardImages = new List<Sprite>(Resources.LoadAll<Sprite>(folderPath));
    }

    private void SetGridLayout()
    {
#if UNITY_ANDROID
        gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
#else
        gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedRowCount;
#endif
        gridLayoutGroup.constraintCount = rows;

        RectTransform rectTransform = gridLayoutGroup.GetComponent<RectTransform>();
        float cellWidth = (rectTransform.rect.width - spacing * (columns - 1)) / columns;
        float cellHeight = (rectTransform.rect.height - spacing * (rows - 1)) / rows;
        float cellSize = Mathf.Min(cellWidth, cellHeight);

        gridLayoutGroup.cellSize = new Vector2(cellSize, cellSize);
        gridLayoutGroup.spacing = new Vector2(spacing, spacing);
    }

    private void ShuffleAndAssignImages()
    {
        int numPairs = (rows * columns) / 2;
        shuffledCardData = new List<CardData>(numPairs * 2 + 1);

        for (int i = 0; i < numPairs; i++)
        {
            int uniqueId = Random.Range(1000, 10000);
            Sprite image = cardImages[i % cardImages.Count];
            shuffledCardData.Add(new CardData(image, uniqueId));
            shuffledCardData.Add(new CardData(image, uniqueId));
        }

        if ((rows * columns) % 2 != 0)
        {
            shuffledCardData.Add(new CardData(oddCard, int.MinValue));
        }

        shuffledCardData.Shuffle();
    }

    private void PlaceCards()
    {
        foreach (var cardData in shuffledCardData)
        {
            GameObject card = GetPooledCard();
            card.GetComponent<Card>().SetCardData(cardData);
        }
    }
}

[System.Serializable]
public class CardData
{
    public Sprite image;
    public int uniqueId;
    public bool isMatched;

    public CardData(Sprite image, int uniqueId, bool isMatched = false)
    {
        this.image = image;
        this.uniqueId = uniqueId;
        this.isMatched = isMatched;
    }
}
