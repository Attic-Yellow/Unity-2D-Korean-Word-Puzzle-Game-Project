using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("매니저")]
    public FirebaseManager firebaseManager;
    public AuthManager authManager;
    public UIManager uiManager;
    public InputManager inputManager;
    public SceneManaged sceneManager;

    [Header("유저 데이터")]
    private bool isUserGuest = false;
    private bool isEmailAuthentication = false;
    private bool isChangedToEmailAccount = false;
    private string userId;
    private int score;
    private int coins;
    private string nickname;
    private Dictionary<string, int> correctAnswers;
    private Dictionary<string, int> wrongAnswers;

    [Header("게임 데이터")]
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
        StartCoroutine(CheckAutoLoginWhenReady()); // 앱 시작 시 로그인 상태 확인
    }

    // 로그인 성공 시 호출되는 콜백 메서드
    public void OnLoginSuccess()
    {
        StartCoroutine(WaitForUserData()); // 사용자 데이터 로드
    }

    // 자동 로그인 확인
    private void CheckAutoLogin()
    {
        if (firebaseManager.auth.CurrentUser != null)
        {
            Debug.Log($"자동 로그인 성공: {firebaseManager.auth.CurrentUser.Email}"); // 이미 로그인된 상태
            StartCoroutine(WaitForUserData()); // 사용자 데이터 로드
        }
        else
        {
            Debug.Log("사용자가 로그인되어 있지 않음"); // 로그인 화면으로 이동하거나 로그인을 유도하는 UI 표시
        }
    }

    // 파이어 베이스 매니저 초기화 확인 메서드
    private IEnumerator CheckAutoLoginWhenReady()
    {
        yield return new WaitUntil(() => firebaseManager != null && firebaseManager.IsFirebaseInitialized); // FirebaseManager가 초기화될 때까지 대기

        if (firebaseManager.auth != null) // FirebaseAuth가 준비되었는지 확인
        {
            CheckAutoLogin();
        }
        else
        {
            Debug.LogError("Firebase Auth is not initialized.");
        }
    }

    // 사용자 데이터 불러오기
    public void LoadCurrentUserProfile()
    {
        if (firebaseManager.auth.CurrentUser != null)
        {
            userId = firebaseManager.auth.CurrentUser.UserId;
            firebaseManager.LoadUserData(userId, OnUserDataLoaded);
        }
        else
        {
            Debug.LogError("사용자가 로그인되어 있지 않음");
        }
    }

    // 사용자 데이터 로드 후 호출되는 콜백 메서드
    private void OnUserDataLoaded(Dictionary<string, object> userData)
    {
        if (userData != null)
        {
            try
            {
                string json = JsonConvert.SerializeObject(userData); // userData 사전을 다시 JSON 문자열로 변환
                UserData deserializedUserData = JsonConvert.DeserializeObject<UserData>(json); // JSON 문자열을 UserData 클래스로 역직렬화

                if (deserializedUserData != null) // 역직렬화된 객체의 속성에 직접 접근
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
                    Debug.LogError("사용자 데이터 역직렬화 실패");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"사용자 데이터 처리 중 오류 발생: {ex.Message}");
            }
        }
        else
        {
            Debug.Log("유저 데이터가 존재하지 않음. 유저 데이터 초기화 시작");
            firebaseManager.InitializeUserData(userId, success =>
            {
                if (success)
                {
                    Debug.Log("유저 데이터 초기화 성공");
                    LoadCurrentUserProfile(); // 초기화 후 다시 사용자 데이터 로드
                }
                else
                {
                    Debug.LogError("유저 데이터 초기화 실패");
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
                uiManager.OnNicknameButton(); // 닉네임 패널 활성화
            }
            else
            {
                sceneManager.LoadSceneForMain(); // 메인 화면으로 자동 전환
            }
        }
        else
        {
            uiManager.OnEmailVerificationButton(); // 이메일 인증 패널 활성화

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
                Debug.Log("닉네임 업데이트 성공");
                OnLoginSuccess();
            }
            else
            {
                Debug.LogError("닉네임 업데이트 실패");
            }
        });
    }

    // 사용자 데이터 업데이트
    public void UpdateScoreAndCoins(bool success, int lineIndex)
    {
        string levelKey = "한글 " + level.ToString() + " 레벨";
        int level3CorrectAnswers;
        if (!success)
        {
            wrongAnswers[levelKey] += 1;
            level3CorrectAnswers = wrongAnswers[levelKey];
            firebaseManager.SaveOrUpdateUserData(userId, score, coins, new Dictionary<string, int>(), new Dictionary<string, int>() { { levelKey, level3CorrectAnswers } }, success =>
            {
                if (success)
                {
                    Debug.Log("Firebase에 점수와 코인 업데이트 성공");
                }
                else
                {
                    Debug.LogError("Firebase에 점수와 코인 업데이트 실패");
                }
            });

            UpdateDisplayUserData(); // 새 점수와 코인으로 UI 업데이트
            return; // 실패 시 점수나 코인을 주지 않고 반환
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
        
        print($"라인 {lineIndex + 1}에서 레벨 {level}에 {finalScore}점과 코인을 획득");
        
        correctAnswers[levelKey] += 1; // Firestore에 사용자 문제 해결 데이터 업데이트
        level3CorrectAnswers = correctAnswers[levelKey];

        print($"{level3CorrectAnswers}, {correctAnswers[levelKey]}");
        firebaseManager.SaveOrUpdateUserData(userId, score, coins, new Dictionary<string, int> { { levelKey, level3CorrectAnswers } }, new Dictionary<string, int>(), success =>
        {
            if (success)
            {
                print("Firebase에 점수와 코인 업데이트 성공");
            }
            else
            {
                Debug.LogError("Firebase에 점수와 코인 업데이트 실패");
            }
        });

        UpdateDisplayUserData(); // 새 점수와 코인으로 UI 업데이트
    }

    public void UpdateDisplayUserData()
    {
        // print($"score : {score}, coins : {coins} 디스플레이 업데이트"); 
        uiManager.DisplayUserProfile(score, coins, correctAnswers, wrongAnswers); // 데이터 표시를 위한 메서드 호출
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
