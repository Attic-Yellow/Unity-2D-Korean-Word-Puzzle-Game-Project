using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class LetterContainer : MonoBehaviour
{
    [SerializeField] private TextMeshPro letterText;
    [SerializeField] private char[] IncompleteLetter = new char[6]; // 글자가 만들어지는 과정을 저장하는 배열(삭제할 때 용이)
    private char lastChar = '\0'; // 마지막 문자를 저장하기 위한 변수
    public int currentIndex = 0; // 글자들의 인덱스를 저장하기 위한 변수
    public bool isNextLetter = false;
    public bool isChangedFinal = false; 

    // 초성, 중성, 종성 리스트를 클래스 레벨의 멤버 변수로 선언
    private char[] consonants = new char[] { 'ㄱ', 'ㄲ', 'ㄴ', 'ㄷ', 'ㄸ', 'ㄹ', 'ㅁ', 'ㅂ', 'ㅃ', 'ㅅ', 'ㅆ', 'ㅇ', 'ㅈ', 'ㅉ', 'ㅊ', 'ㅋ', 'ㅌ', 'ㅍ', 'ㅎ' };
    private char[] vowels = new char[] { 'ㅏ', 'ㅐ', 'ㅑ', 'ㅒ', 'ㅓ', 'ㅔ', 'ㅕ', 'ㅖ', 'ㅗ', 'ㅘ', 'ㅙ', 'ㅚ', 'ㅛ', 'ㅜ', 'ㅝ', 'ㅞ', 'ㅟ', 'ㅠ', 'ㅡ', 'ㅢ', 'ㅣ' };
    private char[] finals = new char[] { '\0', 'ㄱ', 'ㄲ', 'ㄳ', 'ㄴ', 'ㄵ', 'ㄶ', 'ㄷ', 'ㄹ', 'ㄺ', 'ㄻ', 'ㄼ', 'ㄽ', 'ㄾ', 'ㄿ', 'ㅀ', 'ㅁ', 'ㅂ', 'ㅄ', 'ㅅ', 'ㅆ', 'ㅇ', 'ㅈ', 'ㅊ', 'ㅋ', 'ㅌ', 'ㅍ', 'ㅎ' };
    private char[] doubleFinals = new char[] { 'ㄲ', 'ㄳ', 'ㄵ', 'ㄶ', 'ㄺ', 'ㄻ', 'ㄼ', 'ㄽ', 'ㄾ', 'ㄿ', 'ㅀ', 'ㅄ', 'ㅆ' };
    private char[] firtstFinals = new char[] { 'ㄴ', 'ㄹ', 'ㅂ' };

    private void Start()
    {
        Initialize();
    }

    // 글자 입력 칸 초기화 메서드
    public void Initialize()
    {
        letterText.text = "";
        lastChar = '\0';
    }

    // 글자 생성 메서드
    public void SetLetter(char letter)
    {
        if (currentIndex < IncompleteLetter.Length)
        {
            // 마지막 글자가 한글 범위 내에 있는지 확인
            if (lastChar >= 0xAC00 && lastChar <= 0xD7A3)
            {
                // 초성, 중성, 종성 분리
                int lastCharCode = lastChar - 0xAC00;
                int lastCharInitial = lastCharCode / (21 * 28);
                int lastCharMedial = (lastCharCode % (21 * 28)) / 28;
                int lastCharFinal = lastCharCode % 28;

                // 이미 조합된 글자의 중성이 있고, 입력된 글자가 복합 중성이 될 수 있는 경우
                if (IsVowel(letter) && currentIndex == 2)
                {
                    CreateDoubleVowel(letter, lastCharMedial, lastCharInitial);
                    return;
                }

                // 이미 조합된 글자의 종성이 없고, 입력된 글자가 종성이 될 수 없는 경우
                if (lastCharFinal == 0 && !IsFinalConsonant(letter))
                {
                    CreateNewConsonant(letter);
                    return;
                }

                // 이미 조합된 글자의 종성이 없고, 입력된 글자가 종성이 될 수 있는 경우
                if (IsFinalConsonant(letter) && lastCharFinal == 0)
                {
                    CreateFinal(letter, lastCharInitial, lastCharMedial);
                    return;
                }

                // 이미 조합된 글자의 종성이 있고, 입력된 글자가 중성이 될 수 있는 경우
                if (IsVowel(letter) && lastCharFinal != 0 && (currentIndex == 3 || currentIndex == 4 || currentIndex == 5))
                {
                    CreateNewVowel(letter, lastCharFinal);
                    return;
                }

                // 이미 조합된 글자의 종성이 있고, 입력된 글자가 복합 종성이 될 수 있는 경우
                if (IsFinalConsonant(letter) && IsDoubleFirstFinalConsonant(lastCharFinal, letter) && (currentIndex == 3 || currentIndex == 4))
                {
                    CreateDoubleFinal(letter, lastCharFinal, lastCharInitial, lastCharMedial);
                    return;
                }

                // 이미 조합된 글자의 초성, 중성, 종성이 있고, 입력된 글자가 초성이 될 수 있는 경우
                if (IsConsonant(letter) && lastCharFinal != 0 && (currentIndex == 3 || currentIndex == 4 || currentIndex == 5))
                {
                    CreateNewConsonant(letter);
                    return;
                }
            }

            // 글자의 초성이 없고, 입력된 글자가 초성이 될 수 있는 경우
            if (currentIndex == 0 && IsConsonant(letter))
            {
                letterText.text += letter;
                lastChar = letter;
                IncompleteLetter[currentIndex] = lastChar;
                currentIndex++;
                // print($"{currentIndex - 1}, {IncompleteLetter[currentIndex - 1]} 초성, 중성");
                return;
            }

            // 글자의 중성이 없고, 입력된 글자가 중성이 될 수 있는 경우
            if (currentIndex == 1 && IsVowel(letter))
            {
                char newChar = CombineHangul(lastChar, letter);
                letterText.text = letterText.text.Substring(0, letterText.text.Length - 1) + newChar;
                lastChar = newChar;
                IncompleteLetter[currentIndex] = lastChar;
                currentIndex++;
                // print($"{currentIndex - 1}, {IncompleteLetter[currentIndex - 1]} 초성, 중성");
                return;
            }
        }
    }

    // 글자 삭제 메서드
    public void RemoveLetter()
    {
        switch (currentIndex)
        {
            case 0:
                print("입력된 값 없음 패스");
                break;
            case 1:
                letterText.text = null;
                lastChar = '\0';
                currentIndex--;
                // print($"{currentIndex}, {IncompleteLetter[0]}");
                break;
            case 2:
                letterText.text = null;
                letterText.text += IncompleteLetter[0];
                lastChar = IncompleteLetter[0];
                currentIndex--;
                // print($"{currentIndex}, {IncompleteLetter[1]}");
                break;
            case 3:
                letterText.text = null;
                letterText.text += IncompleteLetter[1];
                lastChar = IncompleteLetter[1];
                currentIndex--;
                // print($"{currentIndex}, {IncompleteLetter[2]}");
                break;
            case 4:
                letterText.text = null;
                letterText.text += IncompleteLetter[2];
                lastChar = IncompleteLetter[2];
                currentIndex--;
                // print($"{currentIndex}, {IncompleteLetter[3]}");
                break;
            case 5:
                letterText.text = null;
                letterText.text += IncompleteLetter[3];
                lastChar = IncompleteLetter[3];
                currentIndex--;
                // print($"{currentIndex}, {IncompleteLetter[4]}");
                break;
            case 6:
                letterText.text = null;
                letterText.text += IncompleteLetter[4];
                lastChar = IncompleteLetter[4];
                currentIndex--;
                // print($"{currentIndex}, {IncompleteLetter[5]}");
                break;
        }
    }

    // 올바른 초성인지 확인하는 메서드
    private bool IsConsonant(char ch)
    {
        // 한글 초성 범위
        return (ch >= 'ㄱ' && ch <= 'ㅎ');
    }

    // 올바른 중성인지 확인하는 메서드
    private bool IsVowel(char ch)
    {
        // 한글 중성 범위
        return (ch >= 'ㅏ' && ch <= 'ㅣ');
    }

    // 올바른 종성인지 확인하는 메서드
    private bool IsFinalConsonant(char ch)
    {
        return Array.IndexOf(finals, ch) >= 0;
    }

    // 복합 종성인지 확인하는 메서드
    private bool IsDoubleFinalConsonant(char ch)
    {
        return Array.IndexOf(doubleFinals, ch) >= 0;
    }

    // 복합 종성 압자리가 될 수 있는지 확인하는 메서드
    public bool IsFirstFinal(char ch)
    {   
        return Array.IndexOf(firtstFinals, ch) >= 0;
    }

    // 복합 종성이 될 수 있는지 확인하는 메서드
    private bool IsDoubleFirstFinalConsonant(int lastCharFinal, char letter)
    {
        switch (lastCharFinal)
        {
            case 1: // 'ㄱ'의 경우
                if (letter == 'ㅅ')
                {
                    return true;
                }
                break;
            case 4: // 'ㄴ'의 경우
                if (letter == 'ㅈ' || letter == 'ㅎ')
                {
                    return true;
                }
                break;
            case 8: // 'ㄹ'의 경우
                if (letter == 'ㄱ' || letter == 'ㅁ' || letter == 'ㅂ' || letter == 'ㅅ' || letter == 'ㅌ' || letter == 'ㅍ' || letter == 'ㅎ')
                {
                    return true;
                }
                break;
            case 17: // 'ㅂ'의 경우
                if (letter == 'ㅅ')
                {
                    return true;
                }
                break;
        }

        return false;
    }

    // 다음 letter로 글자를 넘겨주는 메서드
    public char GetChangedFinal()
    {
        return lastChar;
    }

    // 글자를 넘겨 받은 다음 인덱스의 letter에서 글자를 저장 및 출력하는 메서드
    public void SetChangedFinal(char changeWord)
    {
        if (changeWord >= 0xAC00 && changeWord <= 0xD7A3)
        {
            // 초성, 중성, 종성 분리
            int lastCharCode = changeWord - 0xAC00;
            int lastCharInitial = lastCharCode / (21 * 28);

            letterText.text += consonants[lastCharInitial];
            IncompleteLetter[currentIndex] = consonants[lastCharInitial];
            currentIndex++;
            // print($"{currentIndex - 1}, {IncompleteLetter[currentIndex - 1]}");
            char newChar = changeWord;
            letterText.text = letterText.text.Substring(0, letterText.text.Length - 1) + newChar;
            lastChar = newChar;
            IncompleteLetter[currentIndex] = lastChar;
            currentIndex++;
            // print($"{currentIndex - 1}, {IncompleteLetter[currentIndex - 1]}");
        }
    }

    // 초성과 중성을 조합하는 메서드
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

    // 복합 중성을 생성하는 메서드
    private void CreateDoubleVowel(char letter, int lastCharMedial, int lastCharInitial)
    {
        // 'letter'에 해당하는 중성의 인덱스를 찾기
        int letterMedialIndex = Array.IndexOf(vowels, letter);
        int newMedialIndex = -1; // 초기값 설정, 유효하지 않은 값으로 초기화

        switch (lastCharMedial)
        {
            case 8: // 'ㅗ'의 경우
                if (letterMedialIndex == Array.IndexOf(vowels, 'ㅏ')) // 'ㅏ'
                {
                    newMedialIndex = Array.IndexOf(vowels, 'ㅘ'); // ㅘ의 인덱스
                }
                else if (letterMedialIndex == Array.IndexOf(vowels, 'ㅐ')) // 'ㅐ'
                {
                    newMedialIndex = Array.IndexOf(vowels, 'ㅙ'); // ㅙ의 인덱스
                }
                else if (letterMedialIndex == Array.IndexOf(vowels, 'ㅣ')) // 'ㅣ'
                {
                    newMedialIndex = Array.IndexOf(vowels, 'ㅚ'); // ㅚ의 인덱스
                }
                break;
            case 13: // 'ㅜ'의 경우
                if (letterMedialIndex == Array.IndexOf(vowels, 'ㅓ')) // 'ㅓ'
                {
                    newMedialIndex = Array.IndexOf(vowels, 'ㅝ'); // ㅝ의 인덱스
                }
                else if (letterMedialIndex == Array.IndexOf(vowels, 'ㅔ')) // 'ㅔ'
                {
                    newMedialIndex = Array.IndexOf(vowels, 'ㅞ'); // ㅞ의 인덱스
                }
                else if (letterMedialIndex == Array.IndexOf(vowels, 'ㅣ')) // 'ㅣ'
                {
                    newMedialIndex = Array.IndexOf(vowels, 'ㅟ'); // ㅟ의 인덱스
                }
                break;
            case 18: // 'ㅡ'의 경우
                if (letterMedialIndex == Array.IndexOf(vowels, 'ㅣ')) // 'ㅣ'
                {
                    newMedialIndex = Array.IndexOf(vowels, 'ㅢ'); // ㅢ의 인덱스
                }
                break;
        }

        if (newMedialIndex != -1)
        {
            char newChar = (char)(0xAC00 + (lastCharInitial * 21 * 28) + (newMedialIndex * 28));
            letterText.text = letterText.text.Substring(0, letterText.text.Length - 1) + newChar;
            lastChar = newChar;
            IncompleteLetter[currentIndex] = lastChar;
            currentIndex++;
            // print($"{currentIndex - 1}, {IncompleteLetter[currentIndex - 1]} 복합 중성");
            return;
        }
    }

    // 종성을 생성하는 메서드
    private void CreateFinal(char letter, int lastCharInitial, int lastCharMedial)
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
            // print($"{currentIndex - 1}, {IncompleteLetter[currentIndex - 1]} 종성");
            return;
        }
    }

    // 복합 종성을 생성하는 메서드
    private void CreateDoubleFinal(char letter, int lastCharFinal, int lastCharInitial, int lastCharMedial)
    {
        int letterFinalIndex = Array.IndexOf(finals, letter);
        int newFinalIndex = -1; // 초기값 설정, 유효하지 않은 값으로 초기화

        switch (lastCharFinal)
        {
            case 1: // 'ㄱ'의 경우
                if (letterFinalIndex == Array.IndexOf(finals, 'ㅅ')) // 'ㅅ'
                {
                    newFinalIndex = Array.IndexOf(finals, 'ㄳ'); // ㄳ의 인덱스
                }
                break;
            case 4: // 'ㄴ'의 경우
                if (letterFinalIndex == Array.IndexOf(finals, 'ㅈ')) // 'ㅈ'
                {
                    newFinalIndex = Array.IndexOf(finals, 'ㄵ'); // ㄵ의 인덱스
                }
                else if (letterFinalIndex == Array.IndexOf(finals, 'ㅎ')) // 'ㅎ'
                {
                    newFinalIndex = Array.IndexOf(finals, 'ㄶ'); // ㄶ의 인덱스
                }
                break;
            case 8: // 'ㄹ'의 경우
                if (letterFinalIndex == Array.IndexOf(finals, 'ㄱ')) // 'ㄱ'
                {
                    newFinalIndex = Array.IndexOf(finals, 'ㄺ'); // ㄺ의 인덱스
                }
                else if (letterFinalIndex == Array.IndexOf(finals, 'ㅁ')) // 'ㅁ'
                {
                    newFinalIndex = Array.IndexOf(finals, 'ㄻ'); // ㄻ의 인덱스
                }
                else if (letterFinalIndex == Array.IndexOf(finals, 'ㅂ')) // 'ㅂ'
                {
                    newFinalIndex = Array.IndexOf(finals, 'ㄼ'); // ㄼ의 인덱스
                }
                else if (letterFinalIndex == Array.IndexOf(finals, 'ㅅ')) // 'ㅅ'
                {
                    newFinalIndex = Array.IndexOf(finals, 'ㄽ'); // ㄽ의 인덱스
                }
                else if (letterFinalIndex == Array.IndexOf(finals, 'ㅌ')) // 'ㅌ'
                {
                    newFinalIndex = Array.IndexOf(finals, 'ㄾ'); // ㄾ의 인덱스
                }
                else if (letterFinalIndex == Array.IndexOf(finals, 'ㅍ')) // 'ㅍ'
                {
                    newFinalIndex = Array.IndexOf(finals, 'ㄿ'); // ㄿ의 인덱스
                }
                else if (letterFinalIndex == Array.IndexOf(finals, 'ㅎ')) // 'ㅎ'
                {
                    newFinalIndex = Array.IndexOf(finals, 'ㅀ'); // ㅀ의 인덱스
                }
                break;
            case 17: // 'ㅂ'의 경우
                if (letterFinalIndex == Array.IndexOf(finals, 'ㅅ')) // 'ㅅ'
                {
                    newFinalIndex = Array.IndexOf(finals, 'ㅄ'); // ㅄ의 인덱스
                }
                break;
        }

        if (newFinalIndex != -1)
        {
            char newChar = (char)(0xAC00 + (lastCharInitial * 21 * 28) + (lastCharMedial * 28) + newFinalIndex);
            letterText.text = letterText.text.Substring(0, letterText.text.Length - 1) + newChar;
            lastChar = newChar;
            IncompleteLetter[currentIndex] = lastChar;
            currentIndex++;
            // print($"{currentIndex - 1}, {IncompleteLetter[currentIndex - 1]} 복합 종성");
            return;
        }
    }

    // 다음 letter로 초성으로 변한 종성 값을 넘겨주는 메서드
    private void CreateNewVowel(char letter, int lastCharFinal)
    {
        if (IsDoubleFinalConsonant(finals[lastCharFinal]) && finals[lastCharFinal] != finals[2] && finals[lastCharFinal] != finals[20]) // 복합 종성인 경우
        {
            switch (lastCharFinal)
            {
                case 3: // 'ㄳ'의 경우
                    CreateNewConsonant(finals[19], letter);
                    break;
                case 5: // 'ㄵ'의 경우
                    CreateNewConsonant(finals[22], letter);
                    break;
                case 6: // 'ㄶ'의 경우
                    CreateNewConsonant(finals[27], letter);
                    break;
                case 9: // 'ㄺ'의 경우
                    CreateNewConsonant(finals[1], letter);
                    break;
                case 10: // 'ㄻ'의 경우
                    CreateNewConsonant(finals[16], letter);
                    break;
                case 11: // 'ㄼ'의 경우
                    CreateNewConsonant(finals[17], letter);
                    break;
                case 12: // 'ㄽ'의 경우
                    CreateNewConsonant(finals[19], letter);
                    break;
                case 13: // 'ㄾ'의 경우
                    CreateNewConsonant(finals[25], letter);
                    break;
                case 14: // 'ㄿ'의 경우
                    CreateNewConsonant(finals[26], letter);
                    break;
                case 15: // 'ㅀ'의 경우
                    CreateNewConsonant(finals[27], letter);
                    break;
                case 18: // 'ㅄ'의 경우
                    CreateNewConsonant(finals[19], letter);
                    break;
            }

            return;
        }
        else // 단일 종성인 경우
        {
            RemoveLetter();
            CheckCreateComplete();

            int newConsonantIndex = Array.IndexOf(consonants, finals[lastCharFinal]);
            // 다음 칸의 중성 추가
            char newChar = CombineHangul(consonants[newConsonantIndex], letter);
            lastChar = newChar;
            return;
        }
    }

    // 다음 letter로 복합 종성으로 변한 초성 값을 저장하는 메서드
    private void CreateNewConsonant(char final, char letter)
    {
        RemoveLetter();
        CheckCreateComplete();
        char newChar = CombineHangul(final, letter);
        lastChar = newChar;
        return;
    }

    // 입력된 값이 자음일 때 현재 글자가 완성된 경우, 다음 letter로 넘겨주는 메서드
    private void CreateNewConsonant(char letter)
    {
        isNextLetter = true;
        lastChar = letter;
        return;
    }

    // 글자가 완성된 경우 다음 letter로 넘어가기 위한 메서드
    private void CheckCreateComplete()
    {
        isNextLetter = true;
        isChangedFinal = true;
    }

    // 완성된 글자를 반환하는 메서드
    public char GetLetter()
    {
        return IncompleteLetter[currentIndex - 1];
    }

    // 종성을 반환하는 메서드
    public char GetFinal()
    {
        int lastCharFinal = (GetLetter() - 0xAC00) % 28;
        return finals[lastCharFinal];
    }
}
