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
            Debug.LogError("특수문자 포함해서 8자리 이상 만들 것"); // 특수문자가 포함되지 않은 경우 에러 메시지 출력
            return; // 여기서 함수 실행을 중단
        }

        string email = userEmail.text;
        string password = userPassword.text;

        GameManager.Instance.uiManager.OnSignUpButtonCallBack(email, password);

        userEmail.text = "";
        userPassword.text = "";
    }
}
