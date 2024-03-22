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

    // �ܾ� ���� �ʱ�ȭ
    public void Initialize()
    {
        word = new char[GameManager.Instance.GetLevel()];
        currentLetterIndex = 0;
        foreach (var letterContainer in letterContainers)
        {
            letterContainer.Initialize();
        }
    }

    // ���� �߰�
    public void Add(char letter)
    {
        // ���� ���ڷ� �Ѿ�� �ϴ� ���
        if (!letterContainers[currentLetterIndex].isNextLetter)
        {
            letterContainers[currentLetterIndex].SetLetter(letter);
        }

        // ���� ���ڷ� �Ѿ�� ���
        if (letterContainers[currentLetterIndex].isNextLetter)
        {
            // ������ ĭ�� �ƴ� ���
            if (currentLetterIndex < letterContainers.Length - 1)
            {
                currentLetterIndex++;

                if (letterContainers[currentLetterIndex - 1].isChangedFinal) // ������ ������ �ʼ��� �� ���
                {
                    this.letter = letterContainers[currentLetterIndex - 1].GetChangedFinal();
                    letterContainers[currentLetterIndex].SetChangedFinal(this.letter);
                }
                else // ���ο� �ʼ��� �Էµ� ���
                {
                    this.letter = letterContainers[currentLetterIndex - 1].GetChangedFinal();
                    letterContainers[currentLetterIndex].SetLetter(letter);
                }
            }

            letterContainers[currentLetterIndex - 1].isNextLetter = false;
            letterContainers[currentLetterIndex - 1].isChangedFinal = false;
            word[currentLetterIndex - 1] = letterContainers[currentLetterIndex - 1].GetLetter();
        }

        // ������ �����̸�, �߰������� �ԷµǴ� ���� ���� ���
        if (letterContainers[currentLetterIndex].isNextLetter && !isComplete)
        {
            letterContainers[currentLetterIndex].currentIndex++;
            letterContainers[currentLetterIndex].RemoveLetter();
            isComplete = true;
        }

        word[currentLetterIndex] = letterContainers[currentLetterIndex].GetLetter();
    }

    // ���� ����
    public bool RemoveLetter()
    {
        // �Էµ� ���� ���� ���
        if (currentLetterIndex == 0 && letterContainers[currentLetterIndex].currentIndex == 0)
        {
            return false;
        }

        if (letterContainers[currentLetterIndex].currentIndex > 0) // 0��°�� �ƴ� ���
        {
            letterContainers[currentLetterIndex].RemoveLetter();

            // ������ ���ڰ� ������ ���
            if (isComplete)
            {
                letterContainers[currentLetterIndex].isNextLetter = false;
                isComplete = false;
            }
        }
        else if (letterContainers[currentLetterIndex].currentIndex == 0) // 0��°�� ���
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

    // �ܾ �ϼ��Ǿ����� Ȯ��
    public bool IsComplete() 
    {
        return currentLetterIndex == (letterContainers.Length - 1) && letterContainers[currentLetterIndex].currentIndex >= 2;
    }

    // �ϼ��� �ܾ� ��ȯ
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
