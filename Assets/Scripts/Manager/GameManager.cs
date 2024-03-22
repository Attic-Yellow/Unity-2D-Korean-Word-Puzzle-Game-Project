using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public FirebaseManager firebaseManager;
    public UIManager uiManager;
    public InputManager inputManager;
    public SceneManaged sceneManager;

    private int level;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public int GetLevel()
    {
        print(level);
        return level;
    }

    public void SetLevel(int level)
    {
        print(level);
        this.level = level;
        print(this.level);
    }
}
