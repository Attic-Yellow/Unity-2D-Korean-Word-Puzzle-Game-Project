using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordContainer : MonoBehaviour
{
    [SerializeField] private LetterContainer[] letterContainers;
    [SerializeField] private int currentLetterIndex;
    [SerializeField] private char letter;
    [SerializeField] private char[] word;
    [SerializeField] private bool isComplete;

    private void Awake()
    {
        Initialize();
    }

    // 단어 라인 초기화
    public void Initialize()
    {
        word = new char[GameManager.Instance.GetLevel()];
        currentLetterIndex = 0;
        foreach (var letterContainer in letterContainers)
        {
            letterContainer.Initialize();
        }
    }

    // 글자 추가
    public void Add(char letter)
    {
        // 다음 글자로 넘어가야 하는 경우
        if (!letterContainers[currentLetterIndex].isNextLetter)
        {
            letterContainers[currentLetterIndex].SetLetter(letter);
        }

        // 다음 글자로 넘어온 경우
        if (letterContainers[currentLetterIndex].isNextLetter)
        {
            // 마지막 칸이 아닌 경우
            if (currentLetterIndex < letterContainers.Length - 1)
            {
                currentLetterIndex++;

                if (letterContainers[currentLetterIndex - 1].isChangedFinal) // 기존의 종성이 초성이 된 경우
                {
                    this.letter = letterContainers[currentLetterIndex - 1].GetChangedFinal();
                    letterContainers[currentLetterIndex].SetChangedFinal(this.letter);
                }
                else // 새로운 초성이 입력된 경우
                {
                    this.letter = letterContainers[currentLetterIndex - 1].GetChangedFinal();
                    letterContainers[currentLetterIndex].SetLetter(letter);
                }
            }

            letterContainers[currentLetterIndex - 1].isNextLetter = false;
            letterContainers[currentLetterIndex - 1].isChangedFinal = false;
            word[currentLetterIndex - 1] = letterContainers[currentLetterIndex - 1].GetLetter();
        }

        // 마지막 글자이며, 추가적으로 입력되는 것을 막는 경우
        if (letterContainers[currentLetterIndex].isNextLetter && !isComplete)
        {
            letterContainers[currentLetterIndex].currentIndex++;
            letterContainers[currentLetterIndex].RemoveLetter();
            isComplete = true;
        }

        word[currentLetterIndex] = letterContainers[currentLetterIndex].GetLetter();
    }

    // 글자 삭제
    public bool RemoveLetter()
    {
        // 입력된 값이 없는 경우
        if (currentLetterIndex == 0 && letterContainers[currentLetterIndex].currentIndex == 0)
        {
            return false;
        }

        if (letterContainers[currentLetterIndex].currentIndex > 0) // 0번째가 아닌 경우
        {
            letterContainers[currentLetterIndex].RemoveLetter();

            // 마지막 글자가 삭제된 경우
            if (isComplete)
            {
                letterContainers[currentLetterIndex].isNextLetter = false;
                isComplete = false;
            }
        }
        else if (letterContainers[currentLetterIndex].currentIndex == 0) // 0번째인 경우
        {
            currentLetterIndex--;
            letterContainers[currentLetterIndex].RemoveLetter();
        }

        if (IsComplete())
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    // 단어가 완성되었는지 확인
    public bool IsComplete() 
    {
        return currentLetterIndex == (letterContainers.Length - 1) && letterContainers[currentLetterIndex].currentIndex >= 2;
    }

    // 완성된 단어 반환
    public char[] GetWord()
    {
        return word;
    }

    public void CompareLetters(bool isAnswer)
    {
        for (int i = 0; i < letterContainers.Length; i++)
        {
            letterContainers[i].GetCompareLetters(i, isAnswer);
        }
    }
}
