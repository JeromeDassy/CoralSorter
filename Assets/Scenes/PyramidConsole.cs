using UnityEngine;

public class PyramidConsole : MonoBehaviour
{
    void Start()
    {
        int baseLength = 3;
        DrawPyramid(baseLength);
    }

    void DrawPyramid(int baseLength)
    {
        for (int i = 1; i <= baseLength; i++)
        {
            string spaces = new string(' ', baseLength - i);

            string stars = "";
            for (int j = 0; j < i; j++)
            {
                stars += "*";
                if (j < i - 1) stars += " ";
            }

            Debug.Log(spaces + stars);
        }
    }
}
