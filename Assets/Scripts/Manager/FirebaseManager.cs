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
    // ��ŷ �����͸� ���� Ŭ����
    [System.Serializable]
    public class RankingEntry
    {
        public string userID;
        public string nickname;
        public int score;
    }

    public FirebaseAuth auth { get; private set; }

    FirebaseFirestore db;
    public bool IsFirebaseInitialized = false; // �ʱ�ȭ ���� �÷���

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
                IsFirebaseInitialized = true; // �ʱ�ȭ �Ϸ�
                Debug.Log("Firebase�� ����� �غ� ��");
            }
            else
            {
                Debug.LogError($"��� Firebase ���Ӽ��� �ذ��� �� ����: {dependencyStatus}");
            }
        });
    }

    // �Խ�Ʈ �α���
    public void SignInAnonymously(Action<bool> onCompletion)
    {
        auth.SignInAnonymouslyAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("�Խ�Ʈ �α��� ����: " + task.Exception);
                onCompletion(false);
            }
            else
            {
                
                FirebaseUser newUser = task.Result.User; // �Խ�Ʈ �α��� ����
                InitializeUserData(newUser.UserId, success =>
                {
                    if (success)
                    {
                        Debug.Log("�Խ�Ʈ ����� ������ �ʱ�ȭ ����");
                    }
                    else
                    {
                        Debug.LogError("�Խ�Ʈ ����� ������ �ʱ�ȭ ����");
                    }
                    onCompletion(true);
                });
            }
        });
    }

    // �г��� ������Ʈ
    public void UpdateNickname(string userId, string userNickname, Action<bool> onCompletion)
    {
        var docRef = db.Collection("users").Document(userId);

        docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("���� ��ȸ ����: " + task.Exception);
                onCompletion(false);
                return;
            }

            DocumentSnapshot snapshot = task.Result;
            if (snapshot.Exists)
            {
                // ������ �����ϸ�, ������Ʈ ����
                Dictionary<string, object> updates = new Dictionary<string, object>
            {
                { "nickname", userNickname }
            };
                docRef.UpdateAsync(updates).ContinueWithOnMainThread(updateTask =>
                {
                    if (updateTask.IsFaulted)
                    {
                        Debug.LogError("�г��� ������Ʈ ����: " + updateTask.Exception);
                        onCompletion(false);
                    }
                    else
                    {
                        Debug.Log("�г��� ������Ʈ ����");
                        onCompletion(true);
                    }
                });
            }
        });
    }

    // ����� �����͸� ������Ʈ�ϰų� ���ο� �����͸� �����ϴ� �޼ҵ�
    public void SaveOrUpdateUserData(string userId, int score, int coins, Dictionary<string, int> correctAnswers, Dictionary<string, int> wrongAnswers, Action<bool> onCompletion)
    {
        var docRef = db.Collection("users").Document(userId);

        // ������ �����ϴ��� Ȯ��
        docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("���� ��ȸ ����: " + task.Exception);
                onCompletion(false);
                return;
            }

            DocumentSnapshot snapshot = task.Result;
            if (snapshot.Exists)
            {
                // ������ �����ϸ�, ������Ʈ ����
                Dictionary<string, object> updates = new Dictionary<string, object>
            {
                { "score", score },
                { "coins", coins }
            };

                // ���� ������ Ʋ�� ������ ���� ������Ʈ�� �غ�
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
                        Debug.LogError("������ ������Ʈ ����: " + updateTask.Exception);
                        onCompletion(false);
                    }
                    else
                    {
                        Debug.Log("������ ������Ʈ ����");
                        onCompletion(true);
                    }
                });
            }
            else
            {
                // ������ �������� ������, ���ο� ���� ����
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
                        Debug.LogError("�� ���� ���� ����: " + createTask.Exception);
                        onCompletion(false);
                    }
                    else
                    {
                        Debug.Log("�� ���� ���� ����");
                        onCompletion(true);
                    }
                });
            }
        });
    }

    // ����� ������ �ε�
    public async void LoadUserData(string userId, Action<Dictionary<string, object>> onCompletion)
    {
        var docRef = db.Collection("users").Document(userId);
        try
        {
            var snapshot = await docRef.GetSnapshotAsync();
            if (!snapshot.Exists)
            {
                Debug.LogError("���� ������ �ε� ����");
                onCompletion(null);
                return;
            }

            var userData = snapshot.ToDictionary();
            onCompletion(userData);
        }
        catch (Exception ex)
        {
            Debug.LogError($"���� ������ �ε� �� ���� �߻�: {ex.Message}");
            onCompletion(null);
        }
    }

    // ����� ������ �ʱ�ȭ
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
            { "�ѱ� 3 ����", 0 },
            { "�ѱ� 4 ����", 0 },
            { "�ѱ� 5 ����", 0 }
        }},
        { "wrongAnswers", new Dictionary<string, int> {
            { "�ѱ� 3 ����", 0 },
            { "�ѱ� 4 ����", 0 },
            { "�ѱ� 5 ����", 0 }
        }}
    };
        docRef.SetAsync(user).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("����� ������ �ʱ�ȭ ����");
                onCompletion?.Invoke(false);
            }
            else
            {
                onCompletion?.Invoke(true);
            }
        });
    }

    // ���� �ʵ� �� ���� ���� Ȯ��
    public void CheckFieldValueExists(string document, string userValue, System.Action<bool> onResult)
    {
        if (!IsFirebaseInitialized)
        {
            Debug.LogError("Firebase�� ���� �ʱ�ȭ���� �ʾ���");
            onResult(false);
            return;
        }

        DocumentReference docRef = db.Collection("answer").Document(document);
        docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("���� ��ȸ �� ���� �߻�");
                onResult(false);
                return;
            }

            var documentSnapshot = task.Result;
            if (!documentSnapshot.Exists)
            {
                Debug.Log("korean ������ �������� �ʽ��ϴ�.");
                onResult(false);
                return;
            }

            
            if (documentSnapshot.ContainsField(userValue)) // ����� �Է� ���� �ʵ�� �����ϴ��� Ȯ��
            {
                onResult(true); // print($"�ʵ� '{userValue}'�� ������");
            }
            else
            {
                onResult(false); // print($"�ʵ� '{userValue}'�� �������� ����);
            }
        });
    }

    // Guest ���� ������Ʈ
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
                Debug.LogError("Guest ���� ������Ʈ ����: " + task.Exception);
                onCompletion(false);
            }
            else
            {
                Debug.Log("Guest ���� ������Ʈ ����");
                onCompletion(true);
            }
        });
    }

    //  �α׾ƿ�
    public void SignOut()
    {
        if (auth.CurrentUser != null)
        {
            auth.SignOut();
            Debug.Log("�α׾ƿ� ����");
        }
    }

    // ��ŷ ������ ��û
    public IEnumerator GetRankingData(Action<List<RankingEntry>> onCompletion)
    {
        string url = "https://us-central1-project-radle-6a6dc.cloudfunctions.net/generateRanking"; // Cloud Functions�� URL�� �Է��ϼ���.
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                Debug.LogError("��ŷ ������ ��û ����: " + webRequest.error);
                onCompletion(null);
            }
            else
            {
                // Newtonsoft.Json�� ����Ͽ� JSON ������ RankingEntry ����Ʈ�� ��ȯ
                string jsonResponse = webRequest.downloadHandler.text;
                List<RankingEntry> ranking = JsonConvert.DeserializeObject<List<RankingEntry>>(jsonResponse);
                onCompletion(ranking);
            }
        }
    }
}
