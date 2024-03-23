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
                Debug.LogError("회원가입 실패: " + task.Exception);
                onCompletion(false);
            }
            else
            {
                FirebaseUser newUser = task.Result.User; // 회원가입 성공, 사용자 데이터 초기화
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
                Debug.LogError("로그인 실패: " + task.Exception);
                onCompletion(false);
            }
            else
            {
                onCompletion(true); // 로그인 성공
            }
        });
    }
}
