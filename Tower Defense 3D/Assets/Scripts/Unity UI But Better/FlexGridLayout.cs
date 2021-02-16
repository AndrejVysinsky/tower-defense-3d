using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlexGridLayout : LayoutGroup
{
    [SerializeField] int rows;
    [SerializeField] int columns;
    [SerializeField] Vector2 spacing;

    [HideInInspector] new TextAnchor childAlignment;

    private Vector2 cellSize = new Vector2();

    public override void CalculateLayoutInputHorizontal()
    {
        base.CalculateLayoutInputHorizontal();

        int childCount = rectChildren.Count;

        float parentWidth = rectTransform.rect.width - (padding.left + padding.right) - spacing.x * (columns - 1);
        float parentHeight = rectTransform.rect.height - (padding.top + padding.bottom) - spacing.y * (rows - 1);

        cellSize.x = parentWidth / columns;
        cellSize.y = parentHeight / rows;

        int currRow = 0;
        int currCol = 0;

        for (int i = 0; i < rectChildren.Count; i++)
        {
            var rectChild = rectChildren[i];

            float posX = cellSize.x * currCol + padding.left + spacing.x * currCol;
            float posY = cellSize.y * currRow + padding.top + spacing.y * currRow;

            SetChildAlongAxis(rectChild, 0, posX, cellSize.x);
            SetChildAlongAxis(rectChild, 1, posY, cellSize.y);

            currCol++;
            if (currCol >= columns)
            {
                currCol = 0;
                currRow++;
            }
        }
    }

    public override void CalculateLayoutInputVertical()
    {
    }

    public override void SetLayoutHorizontal()
    {
    }

    public override void SetLayoutVertical()
    {
    }
}
