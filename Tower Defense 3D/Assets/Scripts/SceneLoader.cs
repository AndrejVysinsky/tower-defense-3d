﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        
    }

    public void ChangeScene()
    {
        SceneManager.LoadScene(1);
    }
}
