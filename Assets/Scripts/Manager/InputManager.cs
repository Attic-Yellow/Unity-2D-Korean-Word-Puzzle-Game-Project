using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Analytics;
using UnityEngine.Windows;
using UnityEngine.Rendering.LookDev;

public class InputManager : MonoBehaviour
{
    [SerializeField] private WordContainer[] wordContainers;
    [SerializeField] private Button enterButton;

    private int currentWordContaainerIndex;
    [SerializeField] private char[] inputWord;
    [SerializeField] private string word;
    [SerializeField] private bool isInput = false;
    [SerializeField] private string currentAnswerKey;
    [SerializeField] private bool isAnswer = false;
    [SerializeField] private string document;

    public static Action onLetterAdded;
    public static Action onLetterRemoved;

    public void Awake()
    {
        GameManager.Instance.inputManager = this;
    }

    void Start()
    {
        Initialize();

        KeyboardKey.onKeyPressed += KeyPressedCallback;
    }

    private void OnDestroy()
    {
        KeyboardKey.onKeyPressed -= KeyPressedCallback;
    }

    // ���� �ܾ� ���� �� ���� Ű ���� �ʱ�ȭ �޼���
    private void Initialize()
    {
        inputWord = new char[GameManager.Instance.GetLevel()];
        isInput = false;
        document = "korean" + GameManager.Instance.GetLevel();
        currentWordContaainerIndex = 0;

        EnterButtonController();

        for (int i = 0; i < wordContainers.Length; i++)
        {
            wordContainers[i].Initialize();
        }
    }

    // Ű���� �Է� �� ȣ��
    private void KeyPressedCallback(char letter)
    {
        isInput = true;

        wordContainers[currentWordContaainerIndex].Add(letter);

        if (GameManager.Instance.uiManager.GetIsShifted())
        {
            GameManager.Instance.uiManager.OnShiftButton();
        }

        EnterButtonController();

         onLetterAdded?.Invoke();
    }

    // ���� ����� ��ư Ŭ�� �� ȣ��
    public void BackspacePressedCallback()
    {
        wordContainers[currentWordContaainerIndex].RemoveLetter();

        EnterButtonController();

        onLetterAdded?.Invoke();
    }

    // ���� ��ư Ŭ�� �� ȣ��
    public void EnterPressedCallback()
    {
        currentAnswerKey = FindAnyObjectByType<AnswerLoader>().GetCurrentAnswerKey();

        if (word.Equals(currentAnswerKey, StringComparison.OrdinalIgnoreCase)) // ������ ����� ó�� ����
        {
            isAnswer = true;
            wordContainers[currentWordContaainerIndex].CompareLetters(isAnswer);
            GameManager.Instance.uiManager.SuccessController();

        }
        else // ������ ����� ó�� ����
        {
            isAnswer = false;
            wordContainers[currentWordContaainerIndex].CompareLetters(isAnswer);
            isInput = false;
            word = null;
            currentWordContaainerIndex++;
            EnterButtonController();
            
        }
    }

    // ���� ��ư Ȱ��ȭ/��Ȱ��ȭ �޼���
    private void EnterButtonController()
    {
        // �ܾ� ����
        CombinationLetter();

        if (isInput)
        {
            // �ܾ� ���� ���� Ȯ��
            GameManager.Instance.firebaseManager.CheckFieldValueExists(document, word, exists =>
            {
                if (exists && wordContainers[currentWordContaainerIndex].IsComplete()) // �ܾ �����ϸ�, �Է��� ������ ������ ��� ������ ���
                {
                    
                    
                    enterButton.interactable = true; // ���� ��ư Ȱ��ȭ
                }
                else // �ܾ �������� �ʰų�, ���� ������ �������� �ʴ� ���
                {
                    
                    
                    enterButton.interactable = false; // ���� ��ư ��Ȱ��ȭ
                    enterButton.image.color = new Color(0.5f, 0.5f, 0.5f, 1); // ��Ȱ��ȭ ���� ����
                }
            });
        }
        else
        {
            enterButton.interactable = false;
            enterButton.image.color = new Color(0.5f, 0.5f, 0.5f, 1); // ��Ȱ��ȭ ���� ����
        }
    }

    // �Էµ� ���� �ܾ� ���� �޼���
    private string CombinationLetter()
    {
        for (int i = 0; i < wordContainers[currentWordContaainerIndex].GetWord().Length; i++)
        {
            inputWord[i] = wordContainers[currentWordContaainerIndex].GetWord()[i];
        }

        
        word = string.Empty; // word ���� �ʱ�ȭ

        for (int i = 0; i < inputWord.Length; i++)
        {
            word += inputWord[i];
        }

        return word;
    }
}
