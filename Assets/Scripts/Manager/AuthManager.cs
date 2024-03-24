using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using System;
using Firebase.Extensions;

public class AuthManager : MonoBehaviour
{
    private bool isChangedToEmailAccount = false;

    private void Awake()
    {
        GameManager.Instance.authManager = this;
    }

    // �̸��Ϸ� ȸ������
    public void SignUpWithEmail(string email, string password, Action<bool, bool> onCompletion)
    {
        if (GameManager.Instance.GetIsUserGuest())
        {
            LinkGuestAccountWithEmail(email, password);

            SendEmailVerification(emailVerificationSent =>
            {
                onCompletion(true, emailVerificationSent); // onCompletion�� ù ��° ���ڴ� ȸ������ ���� ����, �� ��° ���ڴ� �̸��� ���� ���� ���� ����
            });
        }
        else
        {
            FirebaseAuth.DefaultInstance.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled || task.IsFaulted)
                {
                    Debug.LogError("ȸ������ ����: " + task.Exception);
                    // ȸ������ ����, onCompletion�� ù ��° ���ڴ� ȸ������ ���� ����, �� ��° ���ڴ� �̸��� ���� ���� ����
                    onCompletion(false, false);
                }
                else
                {
                    isChangedToEmailAccount = false;

                    Debug.Log("ȸ������ ����");
                    // ���⿡�� �̸��� ������ ��û�մϴ�.
                    SendEmailVerification(emailVerificationSent =>
                    {
                        onCompletion(true, emailVerificationSent); // onCompletion�� ù ��° ���ڴ� ȸ������ ���� ����, �� ��° ���ڴ� �̸��� ���� ���� ���� ����
                    });
                }
            });
        }
    }

    // �̸��Ϸ� �α���
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

    // ���� ���� ����
    public void SendEmailVerification(Action<bool> onCompletion)
    {
        var user = FirebaseAuth.DefaultInstance.CurrentUser;
        if (user != null)
        {
            user.SendEmailVerificationAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled || task.IsFaulted)
                {
                    Debug.LogError("�̸��� ���� ���� ����: " + task.Exception);
                    onCompletion(false);
                }
                else
                {
                    Debug.Log("�̸��� ���� ��ũ ���� ����");
                    onCompletion(true);
                }
            });
        }
        else
        {
            Debug.LogError("����� �α����� �ʿ��մϴ�");
            onCompletion(false);
        }
    }

    // ���� ���� Ȯ��
    public void CheckEmailVerification(Action<bool> onCompletion)
    {
        var user = FirebaseAuth.DefaultInstance.CurrentUser;
        if (user != null)
        {
            user.ReloadAsync().ContinueWithOnMainThread(reloadTask =>
            {
                if (reloadTask.IsCanceled || reloadTask.IsFaulted)
                {
                    Debug.LogError("����� ���� ���ΰ�ħ ����: " + reloadTask.Exception);
                    onCompletion(false);
                }
                else
                {
                    // ������� �̸��� ���� ���¸� Ȯ���մϴ�.
                    if (user.IsEmailVerified)
                    {
                        Debug.Log("�̸��� ������");

                        if (!isChangedToEmailAccount)
                        {
                            // �̸��� ������, ����� ������ �ʱ�ȭ
                            GameManager.Instance.firebaseManager.InitializeUserData(user.UserId, success =>
                            {
                                onCompletion(success);
                            });
                        }
                        else
                        {
                            onCompletion(true);
                        }
                    }
                    else
                    {
                        Debug.Log("�̸��� ������");
                        onCompletion(false);
                    }
                }
            });
        }
        else
        {
            Debug.LogError("����� �α����� �ʿ��մϴ�.");
            onCompletion(false);
        }
    }

    // �Խ�Ʈ �α��� ������ ���� �̸��Ϸ� ���� ����
    public void LinkGuestAccountWithEmail(string email, string password)
    {
        var user = FirebaseAuth.DefaultInstance.CurrentUser;
        var credential = EmailAuthProvider.GetCredential(email, password);

        user.LinkWithCredentialAsync(credential).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("LinkWithCredentialAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("LinkWithCredentialAsync encountered an error: " + task.Exception);
                return;
            }

            FirebaseUser newUser = task.Result.User;
            Debug.LogFormat("User linked successfully. User ID: {0}", newUser.UserId);

            isChangedToEmailAccount = true;
            GameManager.Instance.SetIsUserGuest(false);

            // �Խ�Ʈ ���� ������Ʈ
            GameManager.Instance.firebaseManager.UpdateGuestStatus(task.Result.User.UserId, false, guestUpdated =>
            {
                if (guestUpdated)
                {
                    Debug.Log("Guest ���� ������Ʈ ����");
                }
                else
                {
                    Debug.LogError("Guest ���� ������Ʈ ����");
                }
            });
        });
    }

    public bool IsChangedToEmailAccount()
    {
        return isChangedToEmailAccount;
    }
}
