using System.Collections.Generic;
using UnityEngine;

public class BuildCategory : MonoBehaviour
{
    [SerializeField] List<GameObject> categories;

    private int _activeCategoryIndex = 0;

    private void Awake()
    {
        for (int i = 0; i < categories.Count; i++)
        {
            if (i == 0)
                categories[i].SetActive(true);
            else
                categories[i].SetActive(false);
        }    
    }

    public void ShowCategory(int index)
    {
        if (_activeCategoryIndex == index || index >= categories.Count)
            return;

        categories[_activeCategoryIndex].SetActive(false);
        categories[index].SetActive(true);

        _activeCategoryIndex = index;
    }
}