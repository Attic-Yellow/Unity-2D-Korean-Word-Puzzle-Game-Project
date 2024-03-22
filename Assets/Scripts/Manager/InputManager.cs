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

    // 현재 단어 라인 및 엔터 키 상태 초기화 메서드
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

    // 키보드 입력 시 호출
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

    // 글자 지우기 버튼 클릭 시 호출
    public void BackspacePressedCallback()
    {
        wordContainers[currentWordContaainerIndex].RemoveLetter();

        EnterButtonController();

        onLetterAdded?.Invoke();
    }

    // 엔터 버튼 클릭 시 호출
    public void EnterPressedCallback()
    {
        currentAnswerKey = FindAnyObjectByType<AnswerLoader>().GetCurrentAnswerKey();

        if (word.Equals(currentAnswerKey, StringComparison.OrdinalIgnoreCase)) // 정답일 경우의 처리 로직
        {
            isAnswer = true;
            wordContainers[currentWordContaainerIndex].CompareLetters(isAnswer);
            GameManager.Instance.uiManager.SuccessController();

        }
        else // 오답일 경우의 처리 로직
        {
            isAnswer = false;
            wordContainers[currentWordContaainerIndex].CompareLetters(isAnswer);
            isInput = false;
            word = null;
            currentWordContaainerIndex++;
            EnterButtonController();
            
        }
    }

    // 엔터 버튼 활성화/비활성화 메서드
    private void EnterButtonController()
    {
        // 단어 조합
        CombinationLetter();

        if (isInput)
        {
            // 단어 존재 여부 확인
            GameManager.Instance.firebaseManager.CheckFieldValueExists(document, word, exists =>
            {
                if (exists && wordContainers[currentWordContaainerIndex].IsComplete()) // 단어가 존재하며, 입력한 글자의 조건이 모두 갖춰진 경우
                {
                    
                    
                    enterButton.interactable = true; // 엔터 버튼 활성화
                }
                else // 단어가 존재하지 않거나, 글자 조건이 충족되지 않는 경우
                {
                    
                    
                    enterButton.interactable = false; // 엔터 버튼 비활성화
                    enterButton.image.color = new Color(0.5f, 0.5f, 0.5f, 1); // 비활성화 상태 색상
                }
            });
        }
        else
        {
            enterButton.interactable = false;
            enterButton.image.color = new Color(0.5f, 0.5f, 0.5f, 1); // 비활성화 상태 색상
        }
    }

    // 입력된 글자 단어 조합 메서드
    private string CombinationLetter()
    {
        for (int i = 0; i < wordContainers[currentWordContaainerIndex].GetWord().Length; i++)
        {
            inputWord[i] = wordContainers[currentWordContaainerIndex].GetWord()[i];
        }

        
        word = string.Empty; // word 변수 초기화

        for (int i = 0; i < inputWord.Length; i++)
        {
            word += inputWord[i];
        }

        return word;
    }
}
