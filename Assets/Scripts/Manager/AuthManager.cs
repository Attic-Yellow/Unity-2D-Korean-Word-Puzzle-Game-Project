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

    // 이메일로 회원가입
    public void SignUpWithEmail(string email, string password, Action<bool, bool> onCompletion)
    {
        if (GameManager.Instance.GetIsUserGuest())
        {
            LinkGuestAccountWithEmail(email, password);
            if (GameManager.Instance.GetIsUserGuest())
            {
                GameManager.Instance.firebaseManager.SignOut();
            }
            GameManager.Instance.authManager.SignInWithEmail(email, password, success => 
            {
                SendEmailVerification(emailVerificationSent =>
                {
                    onCompletion(true, emailVerificationSent); // onCompletion의 첫 번째 인자는 회원가입 성공 여부, 두 번째 인자는 이메일 인증 전송 성공 여부
                });
            });
        }
        else
        {
            FirebaseAuth.DefaultInstance.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled || task.IsFaulted)
                {
                    print("회원가입 실패: " + task.Exception);
                    // 회원가입 실패, onCompletion의 첫 번째 인자는 회원가입 성공 여부, 두 번째 인자는 이메일 인증 전송 여부
                    onCompletion(false, false);
                }
                else
                {
                    GameManager.Instance.SetIsChangedToEmailAccount(false);
                    GameManager.Instance.firebaseManager.UpdateChangedToEmailAccount(FirebaseAuth.DefaultInstance.CurrentUser.UserId, false, success => {});

                    print("회원가입 성공");
                    // 여기에서 이메일 인증을 요청합니다.
                    SendEmailVerification(emailVerificationSent =>
                    {
                        onCompletion(true, emailVerificationSent); // onCompletion의 첫 번째 인자는 회원가입 성공 여부, 두 번째 인자는 이메일 인증 전송 성공 여부
                    });
                }
            });
        }
    }

    // 이메일로 로그인
    public void SignInWithEmail(string email, string password, Action<bool> onCompletion)
    {
        GameManager.Instance.firebaseManager.auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                print("로그인 실패: " + task.Exception);
                onCompletion(false);
            }
            else
            {
                onCompletion(true); // 로그인 성공
            }
        });
    }

    // 인증 메일 전송
    public void SendEmailVerification(Action<bool> onCompletion)
    {
        var user = FirebaseAuth.DefaultInstance.CurrentUser;
        if (user != null)
        {
            user.SendEmailVerificationAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled || task.IsFaulted)
                {
                    print("이메일 인증 전송 실패: " + task.Exception);
                    onCompletion(false);
                }
                else
                {
                    print("이메일 인증 링크 전송 성공");
                    onCompletion(true);
                }
            });
        }
        else
        {
            print("사용자 로그인이 필요합니다");
            onCompletion(false);
        }
    }

    // 인증 여부 확인
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
                    print("사용자 정보 새로고침 실패: " + reloadTask.Exception);
                    onCompletion(false);
                }
                else
                {
                    // 사용자의 이메일 인증 상태를 확인합니다.
                    if (user.IsEmailVerified)
                    {
                        print("이메일 인증됨");

                        // 이메일 인증 상태 업데이트
                        GameManager.Instance.SetIsEmailAuthentication(true);
                        GameManager.Instance.firebaseManager.UpdateEmailAuthentication(user.UserId, true, success =>
                        {
                            if (success)
                            {
                                print("이메일 인증 상태 업데이트 성공");
                            }
                            else
                            {
                                print("이메일 인증 상태 업데이트 실패");
                            }
                        });

                        if (!GameManager.Instance.GetIsChangedToEmailAccount())
                        {
                            // 이메일 인증됨, 사용자 데이터 초기화
                            GameManager.Instance.firebaseManager.InitializeUserData(user.UserId, success =>
                            {
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
                                    print("Guest 상태 업데이트 성공");
                                }
                                else
                                {
                                    print("Guest 상태 업데이트 실패");
                                }
                            });
                            onCompletion(true);
                        }
                    }
                    else
                    {
                        print("이메일 미인증");
                        onCompletion(false);
                    }
                }
            });
        }
        else
        {
            print("사용자 로그인이 필요합니다.");
            onCompletion(false);
        }
    }

    // 게스트 로그인 유저의 가입 이메일로 계정 연결
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

            // 게스트 상태 업데이트
            GameManager.Instance.firebaseManager.UpdateGuestStatus(newUser.UserId, false, guestUpdated => { });
            GameManager.Instance.firebaseManager.UpdateChangedToEmailAccount(newUser.UserId, true, guestUpdated => { });
        });
    }
}
