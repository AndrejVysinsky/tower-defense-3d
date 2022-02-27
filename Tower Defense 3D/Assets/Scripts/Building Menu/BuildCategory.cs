using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildCategory : MonoBehaviour
{
    [SerializeField] Color activeCategoryColor;
    [SerializeField] Color defaultCategoryColor;
    [SerializeField] List<GameObject> categoryHeaders;
    [SerializeField] List<GameObject> categories;

    private int _activeCategoryIndex = 0;

    private void Awake()
    {
        for (int i = 0; i < categories.Count; i++)
        {
            if (i == 0)
            {
                categories[i].SetActive(true);
                SwitchHeaderColor(i, activeCategoryColor);
            }
            else
            {
                categories[i].SetActive(false);
                SwitchHeaderColor(i, defaultCategoryColor);
            }
        }    
    }

    public void ShowCategory(int index)
    {
        if (_activeCategoryIndex == index || index >= categories.Count)
            return;

        SwitchHeaderColor(_activeCategoryIndex, defaultCategoryColor);
        SwitchHeaderColor(index, activeCategoryColor);

        categories[_activeCategoryIndex].SetActive(false);
        categories[index].SetActive(true);

        _activeCategoryIndex = index;
    }

    private void SwitchHeaderColor(int index, Color color)
    {
        var button = categoryHeaders[index].GetComponent<Button>();

        var headerColorBlock = button.colors;
        headerColorBlock.normalColor = color;
        button.colors = headerColorBlock;
    }
}