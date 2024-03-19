using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Analytics;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;

    [Header(" Elements ")]
    [SerializeField] private WordContainer[] wordContainers;
    [SerializeField] private Button enterButton;

    [Header(" Settings ")]
    private int currentWordContaainerIndex;
    private bool canAddLetter = true;
    private bool shouldReset;

    [Header(" Events ")]
    public static Action onLetterAdded;
    public static Action onLetterRemoved;

    public void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        //연결되어 있음
        Initialize();

        KeyboardKey.onKeyPressed += KeyPressedCallback;
    }

    private void OnDestroy()
    {
        KeyboardKey.onKeyPressed -= KeyPressedCallback;
    }

    private void Initialize()
    {
        currentWordContaainerIndex = 0;
        canAddLetter = true;

        DisableEnterButton();

        for (int i = 0; i < wordContainers.Length; i++)
        {
            wordContainers[i].Initialize();
        }

        shouldReset = false;
    }

    private void KeyPressedCallback(char letter)
    {
        if (!canAddLetter)
            return;

        wordContainers[currentWordContaainerIndex].Add(letter);

        if (wordContainers[currentWordContaainerIndex].IsComplete())
        {
            canAddLetter = false;
            EnableEnterButton();
            //CheckWord();
            //currentWordContaainerIndex++;
        }

        onLetterAdded?.Invoke();
    }

    public void BackspacePressedCallback()
    {
        bool removedLetter = wordContainers[currentWordContaainerIndex].RemoveLetter();

        if (removedLetter)
            DisableEnterButton();

        canAddLetter = true;

        onLetterAdded?.Invoke();
    }

    private void EnableEnterButton()
    {
        enterButton.interactable = true;
    }

    private void DisableEnterButton()
    {
        enterButton.interactable = false;
    }

    public WordContainer GetCurrentWordContainer()
    {
        return wordContainers[currentWordContaainerIndex];
    }
}
