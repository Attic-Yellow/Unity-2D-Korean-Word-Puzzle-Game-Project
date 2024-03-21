using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class KeyboardKey : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI letterText;

    public static Action<char> onKeyPressed;

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

}