using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using System;
using Firebase.Extensions;

public class AuthManager : MonoBehaviour
{
    private void Awake()
    {
        GameManager.Instance.authManager = this;
    }

    public void SignUpWithEmail(string email, string password, Action<bool> onCompletion)
    {
        GameManager.Instance.firebaseManager.auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogError("ȸ������ ����: " + task.Exception);
                onCompletion(false);
            }
            else
            {
                FirebaseUser newUser = task.Result.User; // ȸ������ ����, ����� ������ �ʱ�ȭ
                GameManager.Instance.firebaseManager.InitializeUserData(newUser.UserId, success =>
                {
                    onCompletion(success);
                });
            }
        });
    }

    public void SignInWithEmail(string email, string password, Action<bool> onCompletion)
    {
        GameManager.Instance.firebaseManager.auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogError("�α��� ����: " + task.Exception);
                onCompletion(false);
            }
            else
            {
                onCompletion(true); // �α��� ����
            }
        });
    }
}
