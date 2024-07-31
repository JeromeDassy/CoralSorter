using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[RequireComponent(typeof(GridLayoutGroup))]
public class GridManager : MonoBehaviour
{
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

    void Start()
    {
        gridLayoutGroup = GetComponent<GridLayoutGroup>();
        LoadCardImages();
        SetGridLayout(rows, columns);
        ShuffleAndAssignImages();
        PlaceCards();
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
        GameManager.Instance.SetCardCount(x * y);

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

        // Duplicate images and assign unique IDs
        for (int i = 0; i < numPairs; i++)
        {
            int uniqueId = Random.Range(1000, 10000);
            shuffledCardData.Add(new CardData(cardImages[i], uniqueId));
            shuffledCardData.Add(new CardData(cardImages[i], uniqueId));
        }

        // If odd number of cards, add a unique image with a unique ID
        if ((rows * columns) % 2 != 0)
        {
            shuffledCardData.Add(new CardData(oddCard, int.MinValue));
        }

        // Shuffle the card data
        for (int i = 0; i < shuffledCardData.Count; i++)
        {
            CardData temp = shuffledCardData[i];
            int randomIndex = Random.Range(i, shuffledCardData.Count);
            shuffledCardData[i] = shuffledCardData[randomIndex];
            shuffledCardData[randomIndex] = temp;
        }
    }

    private void PlaceCards()
    {
        for (int i = 0; i < rows * columns; i++)
        {
            GameObject card = Instantiate(cardPrefab, gridLayoutGroup.transform);
            card.GetComponent<Card>().SetCardData(shuffledCardData[i]);
            card.name += $"_{i}";
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
