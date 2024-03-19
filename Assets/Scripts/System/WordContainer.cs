using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordContainer : MonoBehaviour
{
    [SerializeField] private LetterContainer[] letterContainers;

    [SerializeField] private int currentLetterIndex;

    private void Awake()
    {
        Initialize();
    }

    public void Initialize()
    {
        currentLetterIndex = 0;
        foreach (var letterContainer in letterContainers)
        {
            letterContainer.Initialize();
        }
    }

    public void Add(char letter)
    {
        letterContainers[currentLetterIndex].SetLetter(letter);
        //currentLetterIndex++;
    }

    public bool RemoveLetter()
    {
        if (currentLetterIndex <= 0)
            return false;

        currentLetterIndex--;
        letterContainers[currentLetterIndex].Initialize();

        return true;
    }

    public bool IsComplete()
    {
        return currentLetterIndex >= 3;
    }
}
