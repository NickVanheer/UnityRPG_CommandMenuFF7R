using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class GameManager : MonoBehaviour {

    static GameManager instance;

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameManager();
            }
            return instance;
        }
    }

    public Text MPText;

    public int CurrentMP = 300;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void LoadLevel(string name)
    {
        SceneManager.LoadScene(name.Trim());
    }

    public void ChangeMP(int value)
    {
        CurrentMP += value;
        CurrentMP = Mathf.Clamp(CurrentMP, 0, 300);
        MPText.text = CurrentMP.ToString();
    }

    public void EndGame()
    {
        #if UNITY_EDITOR
                Debug.Break();
        #else
         Application.Quit();
        #endif
    }
}
