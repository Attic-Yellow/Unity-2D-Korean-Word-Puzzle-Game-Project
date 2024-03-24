using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Firestore;
using Firebase.Extensions;
using System;
using Firebase.Auth;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class FirebaseManager : MonoBehaviour
{
    // 랭킹 데이터를 위한 클래스
    [System.Serializable]
    public class RankingEntry
    {
        public string userID;
        public string nickname;
        public int score;
    }

    public FirebaseAuth auth { get; private set; }

    FirebaseFirestore db;
    public bool IsFirebaseInitialized = false; // 초기화 상태 플래그

    private void Awake()
    {
        GameManager.Instance.firebaseManager = this;

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => 
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + task.Exception);
                return;
            }

            var dependencyStatus = task.Result;

            if (dependencyStatus == DependencyStatus.Available)
            {
                db = FirebaseFirestore.DefaultInstance;
                auth = FirebaseAuth.DefaultInstance;
                IsFirebaseInitialized = true; // 초기화 완료
                Debug.Log("Firebase를 사용할 준비가 됨");
            }
            else
            {
                Debug.LogError($"모든 Firebase 종속성을 해결할 수 없음: {dependencyStatus}");
            }
        });
    }

    // 게스트 로그인
    public void SignInAnonymously(Action<bool> onCompletion)
    {
        auth.SignInAnonymouslyAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("게스트 로그인 실패: " + task.Exception);
                onCompletion(false);
            }
            else
            {
                
                FirebaseUser newUser = task.Result.User; // 게스트 로그인 성공
                InitializeUserData(newUser.UserId, success =>
                {
                    if (success)
                    {
                        Debug.Log("게스트 사용자 데이터 초기화 성공");
                    }
                    else
                    {
                        Debug.LogError("게스트 사용자 데이터 초기화 실패");
                    }
                    onCompletion(true);
                });
            }
        });
    }

    // 닉네임 업데이트
    public void UpdateNickname(string userId, string userNickname, Action<bool> onCompletion)
    {
        var docRef = db.Collection("users").Document(userId);

        docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("문서 조회 실패: " + task.Exception);
                onCompletion(false);
                return;
            }

            DocumentSnapshot snapshot = task.Result;
            if (snapshot.Exists)
            {
                // 문서가 존재하면, 업데이트 수행
                Dictionary<string, object> updates = new Dictionary<string, object>
            {
                { "nickname", userNickname }
            };
                docRef.UpdateAsync(updates).ContinueWithOnMainThread(updateTask =>
                {
                    if (updateTask.IsFaulted)
                    {
                        Debug.LogError("닉네임 업데이트 실패: " + updateTask.Exception);
                        onCompletion(false);
                    }
                    else
                    {
                        Debug.Log("닉네임 업데이트 성공");
                        onCompletion(true);
                    }
                });
            }
        });
    }

    // 사용자 데이터를 업데이트하거나 새로운 데이터를 생성하는 메소드
    public void SaveOrUpdateUserData(string userId, int score, int coins, Dictionary<string, int> correctAnswers, Dictionary<string, int> wrongAnswers, Action<bool> onCompletion)
    {
        var docRef = db.Collection("users").Document(userId);

        // 문서가 존재하는지 확인
        docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("문서 조회 실패: " + task.Exception);
                onCompletion(false);
                return;
            }

            DocumentSnapshot snapshot = task.Result;
            if (snapshot.Exists)
            {
                // 문서가 존재하면, 업데이트 수행
                Dictionary<string, object> updates = new Dictionary<string, object>
            {
                { "score", score },
                { "coins", coins }
            };

                // 맞춘 문제와 틀린 문제에 대한 업데이트를 준비
                foreach (var entry in correctAnswers)
                {
                    updates[$"correctAnswers.{entry.Key}"] = entry.Value;
                }
                foreach (var entry in wrongAnswers)
                {
                    updates[$"wrongAnswers.{entry.Key}"] = entry.Value;
                }

                docRef.UpdateAsync(updates).ContinueWithOnMainThread(updateTask =>
                {
                    if (updateTask.IsFaulted)
                    {
                        Debug.LogError("데이터 업데이트 실패: " + updateTask.Exception);
                        onCompletion(false);
                    }
                    else
                    {
                        Debug.Log("데이터 업데이트 성공");
                        onCompletion(true);
                    }
                });
            }
            else
            {
                // 문서가 존재하지 않으면, 새로운 문서 생성
                Dictionary<string, object> newUser = new Dictionary<string, object>
            {
                { "score", score },
                { "coins", coins },
                { "correctAnswers", correctAnswers },
                { "wrongAnswers", wrongAnswers }
            };

                docRef.SetAsync(newUser).ContinueWithOnMainThread(createTask =>
                {
                    if (createTask.IsFaulted)
                    {
                        Debug.LogError("새 문서 생성 실패: " + createTask.Exception);
                        onCompletion(false);
                    }
                    else
                    {
                        Debug.Log("새 문서 생성 성공");
                        onCompletion(true);
                    }
                });
            }
        });
    }

    // 사용자 데이터 로드
    public async void LoadUserData(string userId, Action<Dictionary<string, object>> onCompletion)
    {
        var docRef = db.Collection("users").Document(userId);
        try
        {
            var snapshot = await docRef.GetSnapshotAsync();
            if (!snapshot.Exists)
            {
                Debug.LogError("유저 데이터 로드 실패");
                onCompletion(null);
                return;
            }

            var userData = snapshot.ToDictionary();
            onCompletion(userData);
        }
        catch (Exception ex)
        {
            Debug.LogError($"유저 데이터 로드 중 오류 발생: {ex.Message}");
            onCompletion(null);
        }
    }

    // 사용자 데이터 초기화
    public void InitializeUserData(string userId, Action<bool> onCompletion)
    {
        var docRef = db.Collection("users").Document(userId);
        var user = new Dictionary<string, object>
    {
        {"Guest" , GameManager.Instance.GetIsUserGuest()},
        {"nickname", null},
        { "score", 0 },
        { "coins", 500 },
        { "correctAnswers", new Dictionary<string, int> {
            { "한글 3 레벨", 0 },
            { "한글 4 레벨", 0 },
            { "한글 5 레벨", 0 }
        }},
        { "wrongAnswers", new Dictionary<string, int> {
            { "한글 3 레벨", 0 },
            { "한글 4 레벨", 0 },
            { "한글 5 레벨", 0 }
        }}
    };
        docRef.SetAsync(user).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("사용자 데이터 초기화 실패");
                onCompletion?.Invoke(false);
            }
            else
            {
                onCompletion?.Invoke(true);
            }
        });
    }

    // 문서 필드 값 존재 여부 확인
    public void CheckFieldValueExists(string document, string userValue, System.Action<bool> onResult)
    {
        if (!IsFirebaseInitialized)
        {
            Debug.LogError("Firebase가 아직 초기화되지 않았음");
            onResult(false);
            return;
        }

        DocumentReference docRef = db.Collection("answer").Document(document);
        docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("문서 조회 중 오류 발생");
                onResult(false);
                return;
            }

            var documentSnapshot = task.Result;
            if (!documentSnapshot.Exists)
            {
                Debug.Log("korean 문서가 존재하지 않습니다.");
                onResult(false);
                return;
            }

            
            if (documentSnapshot.ContainsField(userValue)) // 사용자 입력 값이 필드로 존재하는지 확인
            {
                onResult(true); // print($"필드 '{userValue}'가 존재함");
            }
            else
            {
                onResult(false); // print($"필드 '{userValue}'가 존재하지 않음);
            }
        });
    }

    // Guest 상태 업데이트
    public void UpdateGuestStatus(string userId, bool isGuest, Action<bool> onCompletion)
    {
        var docRef = db.Collection("users").Document(userId);
        Dictionary<string, object> updates = new Dictionary<string, object>
    {
        { "Guest", isGuest }
    };
        docRef.UpdateAsync(updates).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Guest 상태 업데이트 실패: " + task.Exception);
                onCompletion(false);
            }
            else
            {
                Debug.Log("Guest 상태 업데이트 성공");
                onCompletion(true);
            }
        });
    }

    //  로그아웃
    public void SignOut()
    {
        if (auth.CurrentUser != null)
        {
            auth.SignOut();
            Debug.Log("로그아웃 성공");
        }
    }

    // 랭킹 데이터 요청
    public IEnumerator GetRankingData(Action<List<RankingEntry>> onCompletion)
    {
        string url = "https://us-central1-project-radle-6a6dc.cloudfunctions.net/generateRanking"; // Cloud Functions의 URL을 입력하세요.
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                Debug.LogError("랭킹 데이터 요청 실패: " + webRequest.error);
                onCompletion(null);
            }
            else
            {
                // Newtonsoft.Json을 사용하여 JSON 응답을 RankingEntry 리스트로 변환
                string jsonResponse = webRequest.downloadHandler.text;
                List<RankingEntry> ranking = JsonConvert.DeserializeObject<List<RankingEntry>>(jsonResponse);
                onCompletion(ranking);
            }
        }
    }
}
