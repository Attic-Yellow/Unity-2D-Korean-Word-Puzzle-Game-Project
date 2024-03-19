using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class LetterContainer : MonoBehaviour
{
    [SerializeField] private TextMeshPro letterText;
    private char lastChar = '\0'; // 마지막 문자를 저장하기 위한 변수
    private char[] IncompleteLetter = new char[5];
    private int currentIndex = 0;

    // 초성, 중성, 종성 리스트를 클래스 레벨의 멤버 변수로 선언
    private char[] consonants = new char[] { 'ㄱ', 'ㄲ', 'ㄴ', 'ㄷ', 'ㄸ', 'ㄹ', 'ㅁ', 'ㅂ', 'ㅃ', 'ㅅ', 'ㅆ', 'ㅇ', 'ㅈ', 'ㅉ', 'ㅊ', 'ㅋ', 'ㅌ', 'ㅍ', 'ㅎ' };
    private char[] vowels = new char[] { 'ㅏ', 'ㅐ', 'ㅑ', 'ㅒ', 'ㅓ', 'ㅔ', 'ㅕ', 'ㅖ', 'ㅗ', 'ㅘ', 'ㅙ', 'ㅚ', 'ㅛ', 'ㅜ', 'ㅝ', 'ㅞ', 'ㅟ', 'ㅠ', 'ㅡ', 'ㅢ', 'ㅣ' };
    private char[] finals = new char[] { '\0', 'ㄱ', 'ㄲ', 'ㄳ', 'ㄴ', 'ㄵ', 'ㄶ', 'ㄷ', 'ㄹ', 'ㄺ', 'ㄻ', 'ㄼ', 'ㄽ', 'ㄾ', 'ㄿ', 'ㅀ', 'ㅁ', 'ㅂ', 'ㅄ', 'ㅅ', 'ㅆ', 'ㅇ', 'ㅈ', 'ㅊ', 'ㅋ', 'ㅌ', 'ㅍ', 'ㅎ' };

    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        letterText.text = "";
        lastChar = '\0';
    }

    public void SetLetter(char letter)
    {
        // 마지막 글자가 한글 범위 내에 있는지 확인
        if (lastChar >= 0xAC00 && lastChar <= 0xD7A3)
        {
            // 초성, 중성, 종성 분리
            int lastCharCode = lastChar - 0xAC00;
            int lastCharInitial = lastCharCode / (21 * 28);
            int lastCharMedial = (lastCharCode % (21 * 28)) / 28;
            int lastCharFinal = lastCharCode % 28;

            // 이미 조합된 글자의 종성이 있고, 입력된 글자가 초성이 될 수 있는 경우
            if (lastCharFinal > 0 && IsConsonant(letter))
            {
                // 새로운 글자 생성
                char newChar = letter;
                // 텍스트 업데이트
                letterText.text += newChar;
                lastChar = newChar;
                IncompleteLetter[currentIndex] = lastChar;
                currentIndex++;
                print($"{currentIndex - 1}, {IncompleteLetter[currentIndex - 1]}");
                return;
            }

            // 이미 조합된 글자의 중성이 있고, 입력된 글자가 복합 중성이 될 수 있는 경우
            if (lastCharFinal == 0 && IsVowel(letter))
            {
                // 복합 중성을 포함한 글자 생성
                char newChar = CombineHangul(consonants[lastCharInitial], lastChar, letter);
                // 텍스트 업데이트
                letterText.text = letterText.text.Substring(0, letterText.text.Length - 1) + newChar;
                lastChar = newChar;
                IncompleteLetter[currentIndex] = lastChar;
                currentIndex++;
                print($"{currentIndex - 1}, {IncompleteLetter[currentIndex - 1]}");
                return;
            }

            // 이미 조합된 글자의 종성이 없고, 입력된 글자가 종성이 될 수 있는 경우
            if (lastCharFinal == 0 && IsFinalConsonant(letter))
            {
                int newFinalIndex = Array.IndexOf(finals, letter);
                if (newFinalIndex > 0) // '\0'을 제외한 유효한 종성 인덱스
                {
                    // 새로운 종성을 포함한 글자 생성
                    char newChar = (char)(0xAC00 + (lastCharInitial * 21 * 28) + (lastCharMedial * 28) + newFinalIndex);
                    // 텍스트 업데이트
                    letterText.text = letterText.text.Substring(0, letterText.text.Length - 1) + newChar;
                    lastChar = newChar;
                    IncompleteLetter[currentIndex] = lastChar;
                    currentIndex++;
                    print($"{currentIndex - 1}, {IncompleteLetter[currentIndex - 1]}");
                    return;
                }
            }

            // 이미 조합된 글자의 종성이 있고, 입력된 글자가 복합 종성이 될 수 있는 경우
            if (IsFinalConsonant(lastChar) && IsFinalConsonant(letter))
            {
                int lastFinalIndex = Array.IndexOf(finals, lastChar);
                int newFinalIndex = Array.IndexOf(finals, letter);
                if (lastFinalIndex > 0 && newFinalIndex > 0) // '\0'을 제외한 유효한 종성 인덱스
                {
                    // 새로운 복합 종성을 포함한 글자 생성
                    char newChar = (char)(0xAC00 + (lastCharInitial * 21 * 28) + (lastCharMedial * 28) + newFinalIndex);
                    // 텍스트 업데이트
                    letterText.text = letterText.text.Substring(0, letterText.text.Length - 1) + newChar;
                    lastChar = newChar;
                    IncompleteLetter[currentIndex] = lastChar;
                    currentIndex++;
                    print($"{currentIndex - 1}, {IncompleteLetter[currentIndex - 1]}");
                    return;
                }
            }
        }

        // 기존 로직에 따라 글자 추가
        if (IsConsonant(lastChar) && IsVowel(letter))
        {
            // 초성+중성 조합
            char newChar = CombineHangul(lastChar, letter);
            letterText.text = letterText.text.Substring(0, letterText.text.Length - 1) + newChar;
            lastChar = newChar;
            IncompleteLetter[currentIndex] = lastChar;
            currentIndex++;
        }
        else
        {
            letterText.text += letter;
            lastChar = letter;
            IncompleteLetter[currentIndex] = lastChar;
            currentIndex++;
        }
        print($"{currentIndex - 1}, {IncompleteLetter[currentIndex - 1]}");
    }

    private bool IsConsonant(char ch)
    {
        // 한글 초성 범위
        return (ch >= 'ㄱ' && ch <= 'ㅎ');
    }

    private bool IsVowel(char ch)
    {
        // 한글 중성 범위
        return (ch >= 'ㅏ' && ch <= 'ㅣ');
    }

    private bool IsFinalConsonant(char ch)
    {
        return Array.IndexOf(finals, ch) >= 0;
    }


    private char CombineHangul(char consonant, char vowel, char? finalConsonant = null)
    {
        // 초성, 중성, 종성 인덱스 찾기
        int consonantIndex = Array.IndexOf(consonants, consonant);
        int vowelIndex = Array.IndexOf(vowels, vowel);
        int finalIndex = finalConsonant.HasValue ? Array.IndexOf(finals, finalConsonant.Value) : 0;

        // 유효성 검사
        if (consonantIndex < 0 || vowelIndex < 0 || finalIndex < 0)
        {
            throw new ArgumentException("Invalid consonant, vowel, or final consonant.");
        }

        // 한글 유니코드 계산
        int combinedCode = 0xAC00 + (consonantIndex * 21 * 28) + (vowelIndex * 28) + finalIndex;

        return (char)combinedCode;
    }
}
