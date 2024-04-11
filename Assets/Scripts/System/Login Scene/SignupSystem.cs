using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class SignupSystem : MonoBehaviour
{
    [SerializeField] private TMP_InputField userEmail;
    [SerializeField] private TMP_InputField userPassword;

    public void OnSignupButtonClicked()
    {

        if (!Regex.IsMatch(userPassword.text, @"[!@#$%^&*(),.?"":{}|<>]") || userPassword.text.Length < 8)
        {
            Debug.LogError("Ư������ �����ؼ� 8�ڸ� �̻� ���� ��"); // Ư�����ڰ� ���Ե��� ���� ��� ���� �޽��� ���
            return; // ���⼭ �Լ� ������ �ߴ�
        }

        string email = userEmail.text;
        string password = userPassword.text;

        GameManager.Instance.uiManager.OnSignUpButtonCallBack(email, password);

        userEmail.text = "";
        userPassword.text = "";
    }
}
