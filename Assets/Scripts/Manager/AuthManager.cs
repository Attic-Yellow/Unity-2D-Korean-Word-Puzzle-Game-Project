using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using System;
using Firebase.Extensions;
using Unity.VisualScripting;

public class AuthManager : MonoBehaviour
{
    private bool isCompleteLink = false;

    private void Awake()
    {
        GameManager.Instance.authManager = this;
    }

    // �̸��Ϸ� ȸ������
    public void SignUpWithEmail(string email, string password, Action<bool, bool> onCompletion)
    {
        if (GameManager.Instance.GetIsUserGuest())
        {
            StartCoroutine(WaitForCompleteLink(email, password, onCompletion));
        }
        else
        {
            FirebaseAuth.DefaultInstance.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled || task.IsFaulted)
                {
                    print("ȸ������ ����: " + task.Exception);
                    // ȸ������ ����, onCompletion�� ù ��° ���ڴ� ȸ������ ���� ����, �� ��° ���ڴ� �̸��� ���� ���� ����
                    onCompletion(false, false);
                }
                else
                {
                    GameManager.Instance.SetIsChangedToEmailAccount(false);
                    GameManager.Instance.firebaseManager.UpdateChangedToEmailAccount(FirebaseAuth.DefaultInstance.CurrentUser.UserId, false, success => {});

                    print("ȸ������ ����");
                    // ���⿡�� �̸��� ������ ��û�մϴ�.
                    SendEmailVerification(emailVerificationSent =>
                    {
                        StartCoroutine(GameManager.Instance.uiManager.ResendEmailCooldown());
                        onCompletion(true, emailVerificationSent); // onCompletion�� ù ��° ���ڴ� ȸ������ ���� ����, �� ��° ���ڴ� �̸��� ���� ���� ���� ����
                    });
                }
                GameManager.Instance.uiManager.LoadingController();
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
                print("�α��� ����: " + task.Exception);
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
                    print("�̸��� ���� ���� ����: " + task.Exception);
                    onCompletion(false);
                }
                else
                {
                    print("�̸��� ���� ��ũ ���� ����");
                    onCompletion(true);
                }
            });
        }
        else
        {
            print("����� �α����� �ʿ��մϴ�");
            onCompletion(false);
        }
    }

    // ���� ���� Ȯ��
    public void CheckEmailVerification(Action<bool> onCompletion)
    {
        print($"{FirebaseAuth.DefaultInstance.CurrentUser}");
        var user = FirebaseAuth.DefaultInstance.CurrentUser;
        if (user != null)
        {
            user.ReloadAsync().ContinueWithOnMainThread(reloadTask =>
            {
                if (reloadTask.IsCanceled || reloadTask.IsFaulted)
                {
                    print("����� ���� ���ΰ�ħ ����: " + reloadTask.Exception);
                    onCompletion(false);
                }
                else
                {
                    // ������� �̸��� ���� ���¸� Ȯ���մϴ�.
                    if (user.IsEmailVerified)
                    {
                        print("�̸��� ������");

                        // �̸��� ���� ���� ������Ʈ
                        GameManager.Instance.SetIsEmailAuthentication(true);
                        GameManager.Instance.firebaseManager.UpdateEmailAuthentication(user.UserId, true, success =>
                        {
                            if (success)
                            {
                                print("�̸��� ���� ���� ������Ʈ ����");
                            }
                            else
                            {
                                print("�̸��� ���� ���� ������Ʈ ����");
                            }
                        });

                        if (!GameManager.Instance.GetIsChangedToEmailAccount())
                        {
                            // �̸��� ������, ����� ������ �ʱ�ȭ
                            GameManager.Instance.firebaseManager.InitializeUserData(user.UserId, success =>
                            {
                                GameManager.Instance.uiManager.LoadingController();
                                onCompletion(success);
                            });
                        }
                        else
                        {
                            GameManager.Instance.SetIsUserGuest(false);
                            GameManager.Instance.firebaseManager.UpdateGuestStatus(user.UserId, false, guestUpdated =>
                            {
                                if (guestUpdated)
                                {
                                    print("Guest ���� ������Ʈ ����");
                                }
                                else
                                {
                                    print("Guest ���� ������Ʈ ����");
                                }
                            });
                            onCompletion(true);
                        }
                    }
                    else
                    {
                        print("�̸��� ������");
                        onCompletion(false);
                    }
                }
            });
        }
        else
        {
            print("����� �α����� �ʿ��մϴ�.");
            onCompletion(false);
        }
    }

    private IEnumerator WaitForCompleteLink(string email, string password, Action<bool, bool> onCompletion)
    {
        isCompleteLink = false;
        LinkGuestAccountWithEmail(email, password);
        yield return new WaitUntil(() => isCompleteLink);

        if (isCompleteLink)
        {
            GameManager.Instance.firebaseManager.SignOut();
            GameManager.Instance.authManager.SignInWithEmail(email, password, success =>
            {
                if (success)
                {
                    SendEmailVerification(emailVerificationSent =>
                    {
                        if (emailVerificationSent)
                        {
                            StartCoroutine(GameManager.Instance.uiManager.ResendEmailCooldown());
                            GameManager.Instance.uiManager.LoadingController();
                            onCompletion(true, emailVerificationSent);
                        }
                    });
                }
            });
        }
        else
        {
            onCompletion(true, false); // ���� ��ũ ���� �Ǵ� ��Ÿ ������ �Ϸ���� �ʾҴٸ�, ���з� ó��
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
                print("LinkWithCredentialAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                print("LinkWithCredentialAsync encountered an error: " + task.Exception);
                return;
            }

            FirebaseUser newUser = task.Result.User;
            Debug.LogFormat("User linked successfully. User ID: {0}", newUser.UserId);
            
            GameManager.Instance.SetIsChangedToEmailAccount(true);
            GameManager.Instance.SetIsUserGuest(false);

            // �Խ�Ʈ ���� ������Ʈ
            GameManager.Instance.firebaseManager.UpdateGuestStatus(newUser.UserId, false, guestUpdated => { });
            GameManager.Instance.firebaseManager.UpdateChangedToEmailAccount(newUser.UserId, true, guestUpdated => { });
            isCompleteLink = true;
        });
    }
}
