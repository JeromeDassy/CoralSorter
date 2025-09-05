using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

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
    private const int poolSize = 30;
    private int lastWidth, lastHeight;

    void Awake()
    {
        Instance = this;
        gridLayoutGroup = GetComponent<GridLayoutGroup>();
        lastWidth = Screen.width;
        lastHeight = Screen.height;

        LoadCardImages();
        InitializeCardPool();
    }

    void Update()
    {
        if (Screen.width != lastWidth || Screen.height != lastHeight)
        {
            lastWidth = Screen.width;
            lastHeight = Screen.height;
            ResizeGridCells();
        }
    }

    #region Save&Load
    public Vector2 GetGridDimensions() => new Vector2(rows, columns);

    public List<CardData> GetCardDataList()
    {
        int gridSize = rows * columns;
        List<CardData> cardDataList = new List<CardData>(gridSize);

        for (int i = 0; i < transform.childCount && i < gridSize; i++)
        {
            Card card = transform.GetChild(i).GetComponent<Card>();
            if (card != null)
            {
                cardDataList.Add(new CardData(card.GetCardFront(), card.UniqueId, card.IsMatched));
            }
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
            Card card = GetPooledCard().GetComponent<Card>();
            card.SetCardData(cardData);
            card.SetMatched(cardData.isMatched);
            if (cardData.isMatched) matched++;
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

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
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
            cardObj.name = $"{cardPrefab.name}_{i}";
            cardPool.Enqueue(cardObj);
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
        go.GetComponent<Card>().ResetCard();
        go.SetActive(false);
        cardPool.Enqueue(go);
    }

    private void LoadCardImages()
    {
        string folderPath = string.IsNullOrEmpty(selectedFolder.path) ? "Cards/FrontGraphics" : selectedFolder.path;
        cardImages = new List<Sprite>(Resources.LoadAll<Sprite>(folderPath));
    }

    private void ApplyCellSizing()
    {
        RectTransform rectTransform = gridLayoutGroup.GetComponent<RectTransform>();

        float cellWidth = (rectTransform.rect.width - spacing * (columns - 1)) / columns;
        float cellHeight = (rectTransform.rect.height - spacing * (rows - 1)) / rows;
        float cellSize = Mathf.Min(cellWidth, cellHeight);

        gridLayoutGroup.cellSize = new Vector2(cellSize, cellSize);
        gridLayoutGroup.spacing = new Vector2(spacing, spacing);
    }

    public void ResizeGridCells() => ApplyCellSizing();

    private void SetGridLayout()
    {
        int height = Screen.height;
        int width = Screen.width;
        gridLayoutGroup.constraint = (width < height)
            ? GridLayoutGroup.Constraint.FixedRowCount
            : GridLayoutGroup.Constraint.FixedColumnCount;

        gridLayoutGroup.constraintCount = Mathf.Max(rows, columns);
        ApplyCellSizing();
    }

    private void ShuffleAndAssignImages()
    {
        int total = rows * columns;
        int numPairs = total / 2;
        shuffledCardData = new List<CardData>(total);

        for (int i = 0; i < numPairs; i++)
        {
            int uniqueId = Random.Range(1000, 10000);
            Sprite image = cardImages[i % cardImages.Count];
            shuffledCardData.Add(new CardData(image, uniqueId));
            shuffledCardData.Add(new CardData(image, uniqueId));
        }

        if (total % 2 != 0)
        {
            shuffledCardData.Add(new CardData(oddCard, int.MinValue));
        }

        shuffledCardData.Shuffle();
    }

    private void PlaceCards()
    {
        foreach (var cardData in shuffledCardData)
        {
            GetPooledCard().GetComponent<Card>().SetCardData(cardData);
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
