using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlexDirectionLayout : LayoutGroup
{
    [SerializeField] List<int> fillRatios;
    [SerializeField] Direction layoutDirection = Direction.Vertical;

    [SerializeField] Vector2 spacing;

    public enum Direction
    {
        Vertical,
        Horizontal
    }

    protected override void OnRectTransformDimensionsChange()
    {
        base.OnRectTransformDimensionsChange();

        CalculateLayoutInputHorizontal();
    }

    public override void CalculateLayoutInputHorizontal()
    {
        base.CalculateLayoutInputHorizontal();

        int childCount = transform.childCount;

        int rows = 1;
        int columns = 1;

        if (layoutDirection == Direction.Vertical)
            rows = childCount;
        else
            columns = childCount;

        float parentWidth = rectTransform.rect.width - (padding.left + padding.right) - spacing.x * (columns - 1);
        float parentHeight = rectTransform.rect.height - (padding.top + padding.bottom) - spacing.y * (rows - 1);

        int ratioSum = 0;

        if (fillRatios != null)
        {
            fillRatios.ForEach(x =>
            {
                if (x == 0)
                    ratioSum++;
                else
                    ratioSum += x;
            });

            ratioSum += childCount - fillRatios.Count;
        }
        else
        {
            ratioSum = childCount;
        }

        float childWidth = parentWidth / ratioSum;
        float childHeight = parentHeight / ratioSum;

        if (layoutDirection == Direction.Vertical)
            childWidth = parentWidth;
        else
            childHeight = parentHeight;

        float xPos = padding.left;
        float yPos = padding.top;

        for (int i = 0; i < rectChildren.Count; i++)
        {
            var rect = rectChildren[i];

            int fillRatioX = 1;
            int fillRatioY = 1;

            if (fillRatios != null && i < fillRatios.Count)
            {
                if (layoutDirection == Direction.Vertical)
                    fillRatioY = fillRatios[i] == 0 ? 1 : fillRatios[i];
                else
                    fillRatioX = fillRatios[i] == 0 ? 1 : fillRatios[i];
            }

            SetChildAlongAxis(rect, 0, xPos, childWidth * fillRatioX);
            SetChildAlongAxis(rect, 1, yPos, childHeight * fillRatioY);

            if (layoutDirection == Direction.Vertical)
                yPos += childHeight * fillRatioY + spacing.y;
            else
                xPos += childWidth * fillRatioX + spacing.x;
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