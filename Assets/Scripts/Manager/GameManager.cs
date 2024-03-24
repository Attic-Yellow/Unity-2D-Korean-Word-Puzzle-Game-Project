using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("�Ŵ���")]
    public FirebaseManager firebaseManager;
    public AuthManager authManager;
    public UIManager uiManager;
    public InputManager inputManager;
    public SceneManaged sceneManager;

    [Header("���� ������")]
    private bool isUserGuest = false;
    private bool isEmailAuthentication = false;
    private bool isChangedToEmailAccount = false;
    private string userId;
    private int score;
    private int coins;
    private string nickname;
    private Dictionary<string, int> correctAnswers;
    private Dictionary<string, int> wrongAnswers;

    [Header("���� ������")]
    private int level;
    private int getScore;
    private bool isDataLoaded = false;

    [Serializable]
    private class UserData
    {
        public bool Guest { get; set; }
        public bool emailauthentication { get; set; }
        public bool ChangedToEmailAccount { get; set; }
        public string nickname { get; set; }
        public int score { get; set; }
        public int coins { get; set; }
        public Dictionary<string, int> correctAnswers { get; set; }
        public Dictionary<string, int> wrongAnswers { get; set; }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        StartCoroutine(CheckAutoLoginWhenReady()); // �� ���� �� �α��� ���� Ȯ��
    }

    // �α��� ���� �� ȣ��Ǵ� �ݹ� �޼���
    public void OnLoginSuccess()
    {
        StartCoroutine(WaitForUserData()); // ����� ������ �ε�
    }

    // �ڵ� �α��� Ȯ��
    private void CheckAutoLogin()
    {
        if (firebaseManager.auth.CurrentUser != null)
        {
            Debug.Log($"�ڵ� �α��� ����: {firebaseManager.auth.CurrentUser.Email}"); // �̹� �α��ε� ����
            StartCoroutine(WaitForUserData()); // ����� ������ �ε�
        }
        else
        {
            Debug.Log("����ڰ� �α��εǾ� ���� ����"); // �α��� ȭ������ �̵��ϰų� �α����� �����ϴ� UI ǥ��
        }
    }

    // ���̾� ���̽� �Ŵ��� �ʱ�ȭ Ȯ�� �޼���
    private IEnumerator CheckAutoLoginWhenReady()
    {
        yield return new WaitUntil(() => firebaseManager != null && firebaseManager.IsFirebaseInitialized); // FirebaseManager�� �ʱ�ȭ�� ������ ���

        if (firebaseManager.auth != null) // FirebaseAuth�� �غ�Ǿ����� Ȯ��
        {
            CheckAutoLogin();
        }
        else
        {
            Debug.LogError("Firebase Auth is not initialized.");
        }
    }

    // ����� ������ �ҷ�����
    public void LoadCurrentUserProfile()
    {
        if (firebaseManager.auth.CurrentUser != null)
        {
            userId = firebaseManager.auth.CurrentUser.UserId;
            firebaseManager.LoadUserData(userId, OnUserDataLoaded);
        }
        else
        {
            Debug.LogError("����ڰ� �α��εǾ� ���� ����");
        }
    }

    // ����� ������ �ε� �� ȣ��Ǵ� �ݹ� �޼���
    private void OnUserDataLoaded(Dictionary<string, object> userData)
    {
        if (userData != null)
        {
            try
            {
                string json = JsonConvert.SerializeObject(userData); // userData ������ �ٽ� JSON ���ڿ��� ��ȯ
                UserData deserializedUserData = JsonConvert.DeserializeObject<UserData>(json); // JSON ���ڿ��� UserData Ŭ������ ������ȭ

                if (deserializedUserData != null) // ������ȭ�� ��ü�� �Ӽ��� ���� ����
                {
                    isUserGuest = deserializedUserData.Guest;
                    isEmailAuthentication = deserializedUserData.emailauthentication;
                    isChangedToEmailAccount = deserializedUserData.ChangedToEmailAccount;
                    nickname = deserializedUserData.nickname;
                    score = deserializedUserData.score;
                    coins = deserializedUserData.coins;
                    correctAnswers = deserializedUserData.correctAnswers ?? new Dictionary<string, int>();
                    wrongAnswers = deserializedUserData.wrongAnswers ?? new Dictionary<string, int>();

                    UpdateDisplayUserData();
                    isDataLoaded = true;
                }
                else
                {
                    Debug.LogError("����� ������ ������ȭ ����");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"����� ������ ó�� �� ���� �߻�: {ex.Message}");
            }
        }
        else
        {
            Debug.Log("���� �����Ͱ� �������� ����. ���� ������ �ʱ�ȭ ����");
            firebaseManager.InitializeUserData(userId, success =>
            {
                if (success)
                {
                    Debug.Log("���� ������ �ʱ�ȭ ����");
                    LoadCurrentUserProfile(); // �ʱ�ȭ �� �ٽ� ����� ������ �ε�
                }
                else
                {
                    Debug.LogError("���� ������ �ʱ�ȭ ����");
                }
            });
        }
    }

    private IEnumerator WaitForUserData()
    {
        isDataLoaded = false;
        LoadCurrentUserProfile();
        yield return new WaitUntil(() => isDataLoaded);

        if(isEmailAuthentication || isUserGuest)
        {
            if (nickname == null)
            {
                uiManager.OnNicknameButton(); // �г��� �г� Ȱ��ȭ
            }
            else
            {
                sceneManager.LoadSceneForMain(); // ���� ȭ������ �ڵ� ��ȯ
            }
        }
        else
        {
            uiManager.OnEmailVerificationButton(); // �̸��� ���� �г� Ȱ��ȭ

            yield return new WaitUntil(() => isEmailAuthentication);
        }
    }

    public void UpdateNickname(string nickname)
    {
        this.nickname = nickname;
        firebaseManager.UpdateNickname(userId, nickname, success =>
        {
            if (success)
            {
                Debug.Log("�г��� ������Ʈ ����");
                OnLoginSuccess();
            }
            else
            {
                Debug.LogError("�г��� ������Ʈ ����");
            }
        });
    }

    // ����� ������ ������Ʈ
    public void UpdateScoreAndCoins(bool success, int lineIndex)
    {
        string levelKey = "�ѱ� " + level.ToString() + " ����";
        int level3CorrectAnswers;
        if (!success)
        {
            wrongAnswers[levelKey] += 1;
            level3CorrectAnswers = wrongAnswers[levelKey];
            firebaseManager.SaveOrUpdateUserData(userId, score, coins, new Dictionary<string, int>(), new Dictionary<string, int>() { { levelKey, level3CorrectAnswers } }, success =>
            {
                if (success)
                {
                    Debug.Log("Firebase�� ������ ���� ������Ʈ ����");
                }
                else
                {
                    Debug.LogError("Firebase�� ������ ���� ������Ʈ ����");
                }
            });

            UpdateDisplayUserData(); // �� ������ �������� UI ������Ʈ
            return; // ���� �� ������ ������ ���� �ʰ� ��ȯ
        }

        int baseScore = 0;

        switch (lineIndex)
        {
            case 0: 
                baseScore = 30; 
                break;
            case 1: 
                baseScore = 25; 
                break;
            case 2: 
                baseScore = 20; 
                break;
            case 3: 
                baseScore = 15; 
                break;
            case 4: 
                baseScore = 10; 
                break;
            case 5: 
                baseScore = 5; 
                break;
        }

        float levelMultiplier = level == 3 ? 1.0f : (level == 4 ? 1.5f : 2.0f);

        int finalScore = (int)(baseScore * levelMultiplier);
        score += finalScore;
        coins += finalScore;
        getScore = finalScore;
        
        print($"���� {lineIndex + 1}���� ���� {level}�� {finalScore}���� ������ ȹ��");
        
        correctAnswers[levelKey] += 1; // Firestore�� ����� ���� �ذ� ������ ������Ʈ
        level3CorrectAnswers = correctAnswers[levelKey];

        print($"{level3CorrectAnswers}, {correctAnswers[levelKey]}");
        firebaseManager.SaveOrUpdateUserData(userId, score, coins, new Dictionary<string, int> { { levelKey, level3CorrectAnswers } }, new Dictionary<string, int>(), success =>
        {
            if (success)
            {
                print("Firebase�� ������ ���� ������Ʈ ����");
            }
            else
            {
                Debug.LogError("Firebase�� ������ ���� ������Ʈ ����");
            }
        });

        UpdateDisplayUserData(); // �� ������ �������� UI ������Ʈ
    }

    public void UpdateDisplayUserData()
    {
        // print($"score : {score}, coins : {coins} ���÷��� ������Ʈ"); 
        uiManager.DisplayUserProfile(score, coins, correctAnswers, wrongAnswers); // ������ ǥ�ø� ���� �޼��� ȣ��
    }

    public void LogOut()
    {
        firebaseManager.SignOut();
        sceneManager.LoadSceneForLogin();
    }

    public int GetLevel()
    {
        return level;
    }

    public void SetLevel(int level)
    {
        this.level = level;
    }

    public int GetScore()
    {
        return getScore;
    }

    public string GetNickname()
    {
        return nickname;
    }

    public void SetIsUserGuest(bool isUserGuest)
    {
        this.isUserGuest = isUserGuest;
    }

    public bool GetIsUserGuest()
    {
        return isUserGuest;
    }

    public void SetIsEmailAuthentication(bool isEmailAuthentication)
    {
        this.isEmailAuthentication = isEmailAuthentication;
    }

    public bool GetIsEmailAuthentication()
    {
        return isEmailAuthentication;
    }

    public void SetIsChangedToEmailAccount(bool isChangedToEmailAccount)
    {
        this.isChangedToEmailAccount = isChangedToEmailAccount;
    }

    public bool GetIsChangedToEmailAccount()
    {
        return isChangedToEmailAccount;
    }
}
