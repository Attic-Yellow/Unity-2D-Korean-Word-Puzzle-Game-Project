using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class LetterContainer : MonoBehaviour
{
    [SerializeField] private TextMeshPro letterText;
    private char lastChar = '\0'; // ������ ���ڸ� �����ϱ� ���� ����
    private char[] IncompleteLetter = new char[5];
    private int currentIndex = 0;

    // �ʼ�, �߼�, ���� ����Ʈ�� Ŭ���� ������ ��� ������ ����
    private char[] consonants = new char[] { '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��' };
    private char[] vowels = new char[] { '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��' };
    private char[] finals = new char[] { '\0', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��', '��' };

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
        // ������ ���ڰ� �ѱ� ���� ���� �ִ��� Ȯ��
        if (lastChar >= 0xAC00 && lastChar <= 0xD7A3)
        {
            // �ʼ�, �߼�, ���� �и�
            int lastCharCode = lastChar - 0xAC00;
            int lastCharInitial = lastCharCode / (21 * 28);
            int lastCharMedial = (lastCharCode % (21 * 28)) / 28;
            int lastCharFinal = lastCharCode % 28;

            // �̹� ���յ� ������ ������ �ְ�, �Էµ� ���ڰ� �ʼ��� �� �� �ִ� ���
            if (lastCharFinal > 0 && IsConsonant(letter))
            {
                // ���ο� ���� ����
                char newChar = letter;
                // �ؽ�Ʈ ������Ʈ
                letterText.text += newChar;
                lastChar = newChar;
                IncompleteLetter[currentIndex] = lastChar;
                currentIndex++;
                print($"{currentIndex - 1}, {IncompleteLetter[currentIndex - 1]}");
                return;
            }

            // �̹� ���յ� ������ �߼��� �ְ�, �Էµ� ���ڰ� ���� �߼��� �� �� �ִ� ���
            if (lastCharFinal == 0 && IsVowel(letter))
            {
                // ���� �߼��� ������ ���� ����
                char newChar = CombineHangul(consonants[lastCharInitial], lastChar, letter);
                // �ؽ�Ʈ ������Ʈ
                letterText.text = letterText.text.Substring(0, letterText.text.Length - 1) + newChar;
                lastChar = newChar;
                IncompleteLetter[currentIndex] = lastChar;
                currentIndex++;
                print($"{currentIndex - 1}, {IncompleteLetter[currentIndex - 1]}");
                return;
            }

            // �̹� ���յ� ������ ������ ����, �Էµ� ���ڰ� ������ �� �� �ִ� ���
            if (lastCharFinal == 0 && IsFinalConsonant(letter))
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
                    print($"{currentIndex - 1}, {IncompleteLetter[currentIndex - 1]}");
                    return;
                }
            }

            // �̹� ���յ� ������ ������ �ְ�, �Էµ� ���ڰ� ���� ������ �� �� �ִ� ���
            if (IsFinalConsonant(lastChar) && IsFinalConsonant(letter))
            {
                int lastFinalIndex = Array.IndexOf(finals, lastChar);
                int newFinalIndex = Array.IndexOf(finals, letter);
                if (lastFinalIndex > 0 && newFinalIndex > 0) // '\0'�� ������ ��ȿ�� ���� �ε���
                {
                    // ���ο� ���� ������ ������ ���� ����
                    char newChar = (char)(0xAC00 + (lastCharInitial * 21 * 28) + (lastCharMedial * 28) + newFinalIndex);
                    // �ؽ�Ʈ ������Ʈ
                    letterText.text = letterText.text.Substring(0, letterText.text.Length - 1) + newChar;
                    lastChar = newChar;
                    IncompleteLetter[currentIndex] = lastChar;
                    currentIndex++;
                    print($"{currentIndex - 1}, {IncompleteLetter[currentIndex - 1]}");
                    return;
                }
            }
        }

        // ���� ������ ���� ���� �߰�
        if (IsConsonant(lastChar) && IsVowel(letter))
        {
            // �ʼ�+�߼� ����
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
        // �ѱ� �ʼ� ����
        return (ch >= '��' && ch <= '��');
    }

    private bool IsVowel(char ch)
    {
        // �ѱ� �߼� ����
        return (ch >= '��' && ch <= '��');
    }

    private bool IsFinalConsonant(char ch)
    {
        return Array.IndexOf(finals, ch) >= 0;
    }


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
}
