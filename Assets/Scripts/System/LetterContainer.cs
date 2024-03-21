using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class LetterContainer : MonoBehaviour
{
    [SerializeField] private TextMeshPro letterText;
    [SerializeField] private char[] IncompleteLetter = new char[6]; // ���ڰ� ��������� ������ �����ϴ� �迭(������ �� ����)
    private char lastChar = '\0'; // ������ ���ڸ� �����ϱ� ���� ����
    public int currentIndex = 0; // ���ڵ��� �ε����� �����ϱ� ���� ����
    public bool isNextLetter = false;
    public bool isChangedFinal = false; 

    // �ʼ�, �߼�, ���� ����Ʈ�� Ŭ���� ������ ��� ������ ����
    private char[] consonants = new char[] { '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��' };
    private char[] vowels = new char[] { '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��' };
    private char[] finals = new char[] { '\0', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��' };
    private char[] doubleFinals = new char[] { '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��' };
    private char[] firtstFinals = new char[] { '��', '��', '��' };

    private void Start()
    {
        Initialize();
    }

    // ���� �Է� ĭ �ʱ�ȭ �޼���
    public void Initialize()
    {
        letterText.text = "";
        lastChar = '\0';
    }

    // ���� ���� �޼���
    public void SetLetter(char letter)
    {
        if (currentIndex < IncompleteLetter.Length)
        {
            // ������ ���ڰ� �ѱ� ���� ���� �ִ��� Ȯ��
            if (lastChar >= 0xAC00 && lastChar <= 0xD7A3)
            {
                // �ʼ�, �߼�, ���� �и�
                int lastCharCode = lastChar - 0xAC00;
                int lastCharInitial = lastCharCode / (21 * 28);
                int lastCharMedial = (lastCharCode % (21 * 28)) / 28;
                int lastCharFinal = lastCharCode % 28;

                // �̹� ���յ� ������ �߼��� �ְ�, �Էµ� ���ڰ� ���� �߼��� �� �� �ִ� ���
                if (IsVowel(letter) && currentIndex == 2)
                {
                    CreateDoubleVowel(letter, lastCharMedial, lastCharInitial);
                    return;
                }

                // �̹� ���յ� ������ ������ ����, �Էµ� ���ڰ� ������ �� �� ���� ���
                if (lastCharFinal == 0 && !IsFinalConsonant(letter))
                {
                    CreateNewConsonant(letter);
                    return;
                }

                // �̹� ���յ� ������ ������ ����, �Էµ� ���ڰ� ������ �� �� �ִ� ���
                if (IsFinalConsonant(letter) && lastCharFinal == 0)
                {
                    CreateFinal(letter, lastCharInitial, lastCharMedial);
                    return;
                }

                // �̹� ���յ� ������ ������ �ְ�, �Էµ� ���ڰ� �߼��� �� �� �ִ� ���
                if (IsVowel(letter) && lastCharFinal != 0 && (currentIndex == 3 || currentIndex == 4 || currentIndex == 5))
                {
                    CreateNewVowel(letter, lastCharFinal);
                    return;
                }

                // �̹� ���յ� ������ ������ �ְ�, �Էµ� ���ڰ� ���� ������ �� �� �ִ� ���
                if (IsFinalConsonant(letter) && IsDoubleFirstFinalConsonant(lastCharFinal, letter) && (currentIndex == 3 || currentIndex == 4))
                {
                    CreateDoubleFinal(letter, lastCharFinal, lastCharInitial, lastCharMedial);
                    return;
                }

                // �̹� ���յ� ������ �ʼ�, �߼�, ������ �ְ�, �Էµ� ���ڰ� �ʼ��� �� �� �ִ� ���
                if (IsConsonant(letter) && lastCharFinal != 0 && (currentIndex == 3 || currentIndex == 4 || currentIndex == 5))
                {
                    CreateNewConsonant(letter);
                    return;
                }
            }

            // ������ �ʼ��� ����, �Էµ� ���ڰ� �ʼ��� �� �� �ִ� ���
            if (currentIndex == 0 && IsConsonant(letter))
            {
                letterText.text += letter;
                lastChar = letter;
                IncompleteLetter[currentIndex] = lastChar;
                currentIndex++;
                // print($"{currentIndex - 1}, {IncompleteLetter[currentIndex - 1]} �ʼ�, �߼�");
                return;
            }

            // ������ �߼��� ����, �Էµ� ���ڰ� �߼��� �� �� �ִ� ���
            if (currentIndex == 1 && IsVowel(letter))
            {
                char newChar = CombineHangul(lastChar, letter);
                letterText.text = letterText.text.Substring(0, letterText.text.Length - 1) + newChar;
                lastChar = newChar;
                IncompleteLetter[currentIndex] = lastChar;
                currentIndex++;
                // print($"{currentIndex - 1}, {IncompleteLetter[currentIndex - 1]} �ʼ�, �߼�");
                return;
            }
        }
    }

    // ���� ���� �޼���
    public void RemoveLetter()
    {
        switch (currentIndex)
        {
            case 0:
                print("�Էµ� �� ���� �н�");
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

    // �ùٸ� �ʼ����� Ȯ���ϴ� �޼���
    private bool IsConsonant(char ch)
    {
        // �ѱ� �ʼ� ����
        return (ch >= '��' && ch <= '��');
    }

    // �ùٸ� �߼����� Ȯ���ϴ� �޼���
    private bool IsVowel(char ch)
    {
        // �ѱ� �߼� ����
        return (ch >= '��' && ch <= '��');
    }

    // �ùٸ� �������� Ȯ���ϴ� �޼���
    private bool IsFinalConsonant(char ch)
    {
        return Array.IndexOf(finals, ch) >= 0;
    }

    // ���� �������� Ȯ���ϴ� �޼���
    private bool IsDoubleFinalConsonant(char ch)
    {
        return Array.IndexOf(doubleFinals, ch) >= 0;
    }

    // ���� ���� ���ڸ��� �� �� �ִ��� Ȯ���ϴ� �޼���
    public bool IsFirstFinal(char ch)
    {   
        return Array.IndexOf(firtstFinals, ch) >= 0;
    }

    // ���� ������ �� �� �ִ��� Ȯ���ϴ� �޼���
    private bool IsDoubleFirstFinalConsonant(int lastCharFinal, char letter)
    {
        switch (lastCharFinal)
        {
            case 1: // '��'�� ���
                if (letter == '��')
                {
                    return true;
                }
                break;
            case 4: // '��'�� ���
                if (letter == '��' || letter == '��')
                {
                    return true;
                }
                break;
            case 8: // '��'�� ���
                if (letter == '��' || letter == '��' || letter == '��' || letter == '��' || letter == '��' || letter == '��' || letter == '��')
                {
                    return true;
                }
                break;
            case 17: // '��'�� ���
                if (letter == '��')
                {
                    return true;
                }
                break;
        }

        return false;
    }

    // ���� letter�� ���ڸ� �Ѱ��ִ� �޼���
    public char GetChangedFinal()
    {
        return lastChar;
    }

    // ���ڸ� �Ѱ� ���� ���� �ε����� letter���� ���ڸ� ���� �� ����ϴ� �޼���
    public void SetChangedFinal(char changeWord)
    {
        if (changeWord >= 0xAC00 && changeWord <= 0xD7A3)
        {
            // �ʼ�, �߼�, ���� �и�
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

    // �ʼ��� �߼��� �����ϴ� �޼���
    private char CombineHangul(char consonant, char vowel, char? finalConsonant = null)
    {
        // �ʼ�, �߼�, ���� �ε��� ã��
        int consonantIndex = Array.IndexOf(consonants, consonant);
        int vowelIndex = Array.IndexOf(vowels, vowel);
        int finalIndex = finalConsonant.HasValue ? Array.IndexOf(finals, finalConsonant.Value) : 0;

        // ��ȿ�� �˻�
        if (consonantIndex < 0 || vowelIndex < 0 || finalIndex < 0)
        {
            throw new ArgumentException("Invalid consonant, vowel, or final consonant.");
        }

        // �ѱ� �����ڵ� ���
        int combinedCode = 0xAC00 + (consonantIndex * 21 * 28) + (vowelIndex * 28) + finalIndex;

        return (char)combinedCode;
    }

    // ���� �߼��� �����ϴ� �޼���
    private void CreateDoubleVowel(char letter, int lastCharMedial, int lastCharInitial)
    {
        // 'letter'�� �ش��ϴ� �߼��� �ε����� ã��
        int letterMedialIndex = Array.IndexOf(vowels, letter);
        int newMedialIndex = -1; // �ʱⰪ ����, ��ȿ���� ���� ������ �ʱ�ȭ

        switch (lastCharMedial)
        {
            case 8: // '��'�� ���
                if (letterMedialIndex == Array.IndexOf(vowels, '��')) // '��'
                {
                    newMedialIndex = Array.IndexOf(vowels, '��'); // ���� �ε���
                }
                else if (letterMedialIndex == Array.IndexOf(vowels, '��')) // '��'
                {
                    newMedialIndex = Array.IndexOf(vowels, '��'); // ���� �ε���
                }
                else if (letterMedialIndex == Array.IndexOf(vowels, '��')) // '��'
                {
                    newMedialIndex = Array.IndexOf(vowels, '��'); // ���� �ε���
                }
                break;
            case 13: // '��'�� ���
                if (letterMedialIndex == Array.IndexOf(vowels, '��')) // '��'
                {
                    newMedialIndex = Array.IndexOf(vowels, '��'); // ���� �ε���
                }
                else if (letterMedialIndex == Array.IndexOf(vowels, '��')) // '��'
                {
                    newMedialIndex = Array.IndexOf(vowels, '��'); // ���� �ε���
                }
                else if (letterMedialIndex == Array.IndexOf(vowels, '��')) // '��'
                {
                    newMedialIndex = Array.IndexOf(vowels, '��'); // ���� �ε���
                }
                break;
            case 18: // '��'�� ���
                if (letterMedialIndex == Array.IndexOf(vowels, '��')) // '��'
                {
                    newMedialIndex = Array.IndexOf(vowels, '��'); // ���� �ε���
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
            // print($"{currentIndex - 1}, {IncompleteLetter[currentIndex - 1]} ���� �߼�");
            return;
        }
    }

    // ������ �����ϴ� �޼���
    private void CreateFinal(char letter, int lastCharInitial, int lastCharMedial)
    {
        int newFinalIndex = Array.IndexOf(finals, letter);

        if (newFinalIndex > 0) // '\0'�� ������ ��ȿ�� ���� �ε���
        {
            // ���ο� ������ ������ ���� ����
            char newChar = (char)(0xAC00 + (lastCharInitial * 21 * 28) + (lastCharMedial * 28) + newFinalIndex);
            // �ؽ�Ʈ ������Ʈ
            letterText.text = letterText.text.Substring(0, letterText.text.Length - 1) + newChar;
            lastChar = newChar;
            IncompleteLetter[currentIndex] = lastChar;
            currentIndex++;
            // print($"{currentIndex - 1}, {IncompleteLetter[currentIndex - 1]} ����");
            return;
        }
    }

    // ���� ������ �����ϴ� �޼���
    private void CreateDoubleFinal(char letter, int lastCharFinal, int lastCharInitial, int lastCharMedial)
    {
        int letterFinalIndex = Array.IndexOf(finals, letter);
        int newFinalIndex = -1; // �ʱⰪ ����, ��ȿ���� ���� ������ �ʱ�ȭ

        switch (lastCharFinal)
        {
            case 1: // '��'�� ���
                if (letterFinalIndex == Array.IndexOf(finals, '��')) // '��'
                {
                    newFinalIndex = Array.IndexOf(finals, '��'); // ���� �ε���
                }
                break;
            case 4: // '��'�� ���
                if (letterFinalIndex == Array.IndexOf(finals, '��')) // '��'
                {
                    newFinalIndex = Array.IndexOf(finals, '��'); // ���� �ε���
                }
                else if (letterFinalIndex == Array.IndexOf(finals, '��')) // '��'
                {
                    newFinalIndex = Array.IndexOf(finals, '��'); // ���� �ε���
                }
                break;
            case 8: // '��'�� ���
                if (letterFinalIndex == Array.IndexOf(finals, '��')) // '��'
                {
                    newFinalIndex = Array.IndexOf(finals, '��'); // ���� �ε���
                }
                else if (letterFinalIndex == Array.IndexOf(finals, '��')) // '��'
                {
                    newFinalIndex = Array.IndexOf(finals, '��'); // ���� �ε���
                }
                else if (letterFinalIndex == Array.IndexOf(finals, '��')) // '��'
                {
                    newFinalIndex = Array.IndexOf(finals, '��'); // ���� �ε���
                }
                else if (letterFinalIndex == Array.IndexOf(finals, '��')) // '��'
                {
                    newFinalIndex = Array.IndexOf(finals, '��'); // ���� �ε���
                }
                else if (letterFinalIndex == Array.IndexOf(finals, '��')) // '��'
                {
                    newFinalIndex = Array.IndexOf(finals, '��'); // ���� �ε���
                }
                else if (letterFinalIndex == Array.IndexOf(finals, '��')) // '��'
                {
                    newFinalIndex = Array.IndexOf(finals, '��'); // ���� �ε���
                }
                else if (letterFinalIndex == Array.IndexOf(finals, '��')) // '��'
                {
                    newFinalIndex = Array.IndexOf(finals, '��'); // ���� �ε���
                }
                break;
            case 17: // '��'�� ���
                if (letterFinalIndex == Array.IndexOf(finals, '��')) // '��'
                {
                    newFinalIndex = Array.IndexOf(finals, '��'); // ���� �ε���
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
            // print($"{currentIndex - 1}, {IncompleteLetter[currentIndex - 1]} ���� ����");
            return;
        }
    }

    // ���� letter�� �ʼ����� ���� ���� ���� �Ѱ��ִ� �޼���
    private void CreateNewVowel(char letter, int lastCharFinal)
    {
        if (IsDoubleFinalConsonant(finals[lastCharFinal]) && finals[lastCharFinal] != finals[2] && finals[lastCharFinal] != finals[20]) // ���� ������ ���
        {
            switch (lastCharFinal)
            {
                case 3: // '��'�� ���
                    CreateNewConsonant(finals[19], letter);
                    break;
                case 5: // '��'�� ���
                    CreateNewConsonant(finals[22], letter);
                    break;
                case 6: // '��'�� ���
                    CreateNewConsonant(finals[27], letter);
                    break;
                case 9: // '��'�� ���
                    CreateNewConsonant(finals[1], letter);
                    break;
                case 10: // '��'�� ���
                    CreateNewConsonant(finals[16], letter);
                    break;
                case 11: // '��'�� ���
                    CreateNewConsonant(finals[17], letter);
                    break;
                case 12: // '��'�� ���
                    CreateNewConsonant(finals[19], letter);
                    break;
                case 13: // '��'�� ���
                    CreateNewConsonant(finals[25], letter);
                    break;
                case 14: // '��'�� ���
                    CreateNewConsonant(finals[26], letter);
                    break;
                case 15: // '��'�� ���
                    CreateNewConsonant(finals[27], letter);
                    break;
                case 18: // '��'�� ���
                    CreateNewConsonant(finals[19], letter);
                    break;
            }

            return;
        }
        else // ���� ������ ���
        {
            RemoveLetter();
            CheckCreateComplete();

            int newConsonantIndex = Array.IndexOf(consonants, finals[lastCharFinal]);
            // ���� ĭ�� �߼� �߰�
            char newChar = CombineHangul(consonants[newConsonantIndex], letter);
            lastChar = newChar;
            return;
        }
    }

    // ���� letter�� ���� �������� ���� �ʼ� ���� �����ϴ� �޼���
    private void CreateNewConsonant(char final, char letter)
    {
        RemoveLetter();
        CheckCreateComplete();
        char newChar = CombineHangul(final, letter);
        lastChar = newChar;
        return;
    }

    // �Էµ� ���� ������ �� ���� ���ڰ� �ϼ��� ���, ���� letter�� �Ѱ��ִ� �޼���
    private void CreateNewConsonant(char letter)
    {
        isNextLetter = true;
        lastChar = letter;
        return;
    }

    // ���ڰ� �ϼ��� ��� ���� letter�� �Ѿ�� ���� �޼���
    private void CheckCreateComplete()
    {
        isNextLetter = true;
        isChangedFinal = true;
    }

    // �ϼ��� ���ڸ� ��ȯ�ϴ� �޼���
    public char GetLetter()
    {
        return IncompleteLetter[currentIndex - 1];
    }

    // ������ ��ȯ�ϴ� �޼���
    public char GetFinal()
    {
        int lastCharFinal = (GetLetter() - 0xAC00) % 28;
        return finals[lastCharFinal];
    }
}
