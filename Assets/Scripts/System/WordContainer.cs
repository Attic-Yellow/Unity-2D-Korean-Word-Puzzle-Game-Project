using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordContainer : MonoBehaviour
{
    [SerializeField] private LetterContainer[] letterContainers;

    [SerializeField] private int currentLetterIndex;

    [SerializeField] private char word;

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

        if (letterContainers[currentLetterIndex].isNextLetter)
        {
            if (currentLetterIndex < letterContainers.Length)
            {
                currentLetterIndex++;
            }

            if (letterContainers[currentLetterIndex - 1].isChangedFinal)
            {
                word = letterContainers[currentLetterIndex - 1].GetChangedFinal();
                letterContainers[currentLetterIndex].SetChangedFinal(word);
            }
            else
            {
                word = letterContainers[currentLetterIndex - 1].GetChangedFinal();
                letterContainers[currentLetterIndex].SetLetter(letter);
            }

            letterContainers[currentLetterIndex - 1].isNextLetter = false;
            letterContainers[currentLetterIndex - 1].isChangedFinal = false;
        }
    }

    public bool RemoveLetter()
    {
        if (currentLetterIndex == 0 && letterContainers[currentLetterIndex].currentIndex == 0)
        {
            return false;
        }

        if (letterContainers[currentLetterIndex].currentIndex > 0)
        {
            letterContainers[currentLetterIndex].RemoveLetter();
        }
        else if (letterContainers[currentLetterIndex].currentIndex == 0)
        {
            currentLetterIndex--;
            letterContainers[currentLetterIndex].RemoveLetter();
        }
        else
        {
            return false;
        }

        return true;
    }

    public bool IsComplete()
    {
        return currentLetterIndex >= 3;
    }
}
