﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelMenu : MonoBehaviour {

    public Button level1;
    public Button level2;
    public Button level3;

    void Start ()
    {
        level1.onClick.AddListener(Level1);
        level2.onClick.AddListener(Level2);
        level3.onClick.AddListener(Level3);
	}

    void Level1()
    {
        SceneManager.LoadScene(4);
    }

    void Level2()
    {
        SceneManager.LoadScene(1);
    }

    void Level3()
    {

    }
}
