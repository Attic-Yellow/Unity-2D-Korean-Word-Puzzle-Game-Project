using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Unity.VisualScripting;

public enum ColorState
{
    None,
    Green,
    Yellow,
    Gray
}
public class KeyboardKey : MonoBehaviour
{
    [SerializeField] private Image[] spriteRenderer;

    public TextMeshProUGUI letterText;
    public static Action<char> onKeyPressed;
    public LetterState letterState = LetterState.None;
    public ColorState colorState = ColorState.None;

    // ��ư������Ʈ�� Ŭ�� �̺�Ʈ�� �޼��� �߰�
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(SendKeyPressedEvent);
    }

    // Ű���� �Է� �� ȣ��
    private void SendKeyPressedEvent()
    {
        onKeyPressed?.Invoke(letterText.text[0]);
    }

    // ���� ���� �޼��� �߰�
    public void SetLetterState(LetterState letterState, ColorState colorState)
    {
        
        this.letterState = letterState;
        if (this.colorState != ColorState.Green)
        {
            this.colorState = colorState;
        }
        UpdateColorBasedOnState(); // ���¿� ���� ���� ������Ʈ
    }

    // ���¿� ���� ���� ������Ʈ �޼���
    private void UpdateColorBasedOnState()
    {
        switch (letterState)
        {
            case LetterState.Consonant:
                switch (colorState)
                {
                    case ColorState.Green:
                        spriteRenderer[0].color = Color.green;
                        break;
                    case ColorState.Yellow:
                        spriteRenderer[0].color = Color.yellow;
                        break;
                    case ColorState.Gray:
                        spriteRenderer[0].color = Color.gray;
                        break;
                    case ColorState.None:
                        spriteRenderer[0].color = Color.white;
                        break;
                    default:
                        break;
                }
                break;
            case LetterState.Vowel:
                switch (colorState)
                {
                    case ColorState.Green:
                        spriteRenderer[1].color = Color.green;
                        break;
                    case ColorState.Yellow:
                        spriteRenderer[1].color = Color.yellow;
                        break;
                    case ColorState.Gray:
                        spriteRenderer[1].color = Color.gray;
                        break;
                    case ColorState.None:
                        spriteRenderer[1].color = Color.white;
                        break;
                    default:
                        break;
                }
                break;
            case LetterState.Final:
                switch (colorState)
                {
                    case ColorState.Green:
                        spriteRenderer[2].color = Color.green;
                        break;
                    case ColorState.Yellow:
                        spriteRenderer[2].color = Color.yellow;
                        break;
                    case ColorState.Gray:
                        spriteRenderer[2].color = Color.gray;
                        break;
                    case ColorState.None:
                        spriteRenderer[2].color = Color.white;
                        break;
                    default:
                        break;
                }
                break;
        }
    }
}