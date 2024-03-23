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

    // 버튼컴포넌트의 클릭 이벤트에 메서드 추가
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(SendKeyPressedEvent);
    }

    // 키보드 입력 시 호출
    private void SendKeyPressedEvent()
    {
        onKeyPressed?.Invoke(letterText.text[0]);
    }

    // 상태 설정 메서드 추가
    public void SetLetterState(LetterState letterState, ColorState colorState)
    {
        
        this.letterState = letterState;
        if (this.colorState != ColorState.Green)
        {
            this.colorState = colorState;
        }
        UpdateColorBasedOnState(); // 상태에 따른 색상 업데이트
    }

    // 상태에 따른 색상 업데이트 메서드
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