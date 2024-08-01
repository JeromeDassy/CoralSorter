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
    private Queue<GameObject> cardPool = new Queue<GameObject>();
    private int poolSize = 30;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        gridLayoutGroup = GetComponent<GridLayoutGroup>();
        LoadCardImages();
        InitializeCardPool();
    }

    public void StartGameWithGrid(int x, int y)
    {
        ResetGrid();

        rows = x;
        columns = y;
        SetGridLayout(rows, columns);
        ShuffleAndAssignImages();
        PlaceCards();
    }

    public void ResetGrid()
    {
        if (shuffledCardData != null && shuffledCardData.Count > 1)
        {
            shuffledCardData.Clear();
        }

        foreach (Transform child in transform)
        {
            ReturnCardToPool(child.gameObject);
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
        else
        {
            GameObject newCard = Instantiate(cardPrefab);
            return newCard;
        }
    }

    private void ReturnCardToPool(GameObject card)
    {
        card.SetActive(false);
        cardPool.Enqueue(card);
    }

    private void LoadCardImages()
    {
        if (string.IsNullOrEmpty(selectedFolder.path))
        {
            Debug.LogError("No Folder selected, fallback on default folder");
            cardImages = new List<Sprite>(Resources.LoadAll<Sprite>("Cards/FrontGraphics"));
            return;
        }
        cardImages = new List<Sprite>(Resources.LoadAll<Sprite>(selectedFolder.path));
    }

    public void SetGridLayout(int x, int y)
    {
        rows = Mathf.Min(x, y);
        columns = Mathf.Max(x, y);
        gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedRowCount;
        gridLayoutGroup.constraintCount = rows;

        float cellWidth = (gridLayoutGroup.GetComponent<RectTransform>().rect.width - spacing * (columns - 1)) / columns;
        float cellHeight = (gridLayoutGroup.GetComponent<RectTransform>().rect.height - spacing * (rows - 1)) / rows;
        float cellSize = Mathf.Min(cellWidth, cellHeight);

        gridLayoutGroup.cellSize = new Vector2(cellSize, cellSize);
        gridLayoutGroup.spacing = new Vector2(spacing, spacing);
    }

    private void ShuffleAndAssignImages()
    {
        int numPairs = (rows * columns) / 2;
        shuffledCardData = new List<CardData>();

        for (int i = 0; i < numPairs; i++)
        {
            int uniqueId = Random.Range(1000, 10000);
            shuffledCardData.Add(new CardData(cardImages[i], uniqueId));
            shuffledCardData.Add(new CardData(cardImages[i], uniqueId));
        }

        if ((rows * columns) % 2 != 0)
        {
            shuffledCardData.Add(new CardData(oddCard, int.MinValue));
        }

        shuffledCardData.Shuffle();
    }

    private void PlaceCards()
    {
        for (int i = 0; i < rows * columns; i++)
        {
            GameObject card = GetPooledCard();
            card.GetComponent<Card>().SetCardData(shuffledCardData[i]);
        }
    }
}

[System.Serializable]
public class CardData
{
    public Sprite image;
    public int uniqueId;

    public CardData(Sprite image, int uniqueId)
    {
        this.image = image;
        this.uniqueId = uniqueId;
    }
}