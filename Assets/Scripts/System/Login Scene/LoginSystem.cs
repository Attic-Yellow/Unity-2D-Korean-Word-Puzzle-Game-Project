using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoginSystem : MonoBehaviour
{
    [SerializeField] private TMP_InputField userEmail;
    [SerializeField] private TMP_InputField userPassword;

    public void OnLoginButtonClicked()
    {
        string email = userEmail.text;
        string password = userPassword.text;

        GameManager.Instance.uiManager.OnLogInButtonCallBack(email, password);

        userEmail.text = "";
        userPassword.text = "";
    }
}
