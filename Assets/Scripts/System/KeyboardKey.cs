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

}