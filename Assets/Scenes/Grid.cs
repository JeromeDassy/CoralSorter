using UnityEngine;
using UnityEngine.UI;

public class Grid : MonoBehaviour
{
    public int rows = 4;
    public GameObject element;
    private GridLayoutGroup GridLayoutGroup;

    void Start()
    {
        GridLayoutGroup = GetComponent<GridLayoutGroup>();

        GridLayoutGroup.spacing = new Vector2(20, 20);
        GridLayoutGroup.childAlignment = TextAnchor.MiddleCenter;
        GridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedRowCount;
        GridLayoutGroup.constraintCount = rows;

        int a = 0;
        for (int i = 0; i < rows*rows; i++)
        {
            GameObject obj = Instantiate(element, this.transform);
            if (i == rows || i == rows*(a+1) )
            {
                a++;
            }   
            int condition = (a % 2 == 1) ? 1 : 0;

            if (i%2 == condition)
            {
                Image img = obj.GetComponent<Image>();
                img.color = Color.black;
            }
        }
    }
}
