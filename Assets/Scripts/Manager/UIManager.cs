using Firebase.Auth;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("로그인 씬")]
    [SerializeField] private GameObject logIn;
    [SerializeField] private GameObject signUp;
    [SerializeField] private GameObject nickname;
    [SerializeField] private GameObject emailVerification;
    [SerializeField] private TMP_InputField nicknameInputField;
    [SerializeField] private string email;
    [SerializeField] private string password;

    [Header("메인 씬")]
    [SerializeField] private GameObject profile;

    [Header("게임 씬")]
    [SerializeField] private AnswerLoader answerLoader;
    [SerializeField] private GameObject line1;
    [SerializeField] private GameObject shiftLine1;
    [SerializeField] private GameObject success;
    [SerializeField] private GameObject fail;
    [SerializeField] private bool isShifted = false;
    [SerializeField] private TextMeshProUGUI successGetScore;

    [Header("공용")]
    [SerializeField] private GameObject options;
    [SerializeField] private GameObject[] checkMessage;

    [Header("유저 정보")]
    [SerializeField] private TextMeshProUGUI nicknameText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI coinsText;
    [SerializeField] private TextMeshProUGUI[] correctAnswersText;
    [SerializeField] private TextMeshProUGUI[] wrongAnswersText;

    private void Awake()
    {
        GameManager.Instance.uiManager = this;
    }

    private void Start()
    {

        if (logIn != null)
        {
            logIn.SetActive(false);
        }

        if (signUp != null) 
        {
            signUp.SetActive(false);
        }

        if (emailVerification != null)
        {
            emailVerification.SetActive(false);
        }

        if (nickname != null)
        {
            nickname.SetActive(false);
        }

        if (line1 != null)
        {
            line1.SetActive(true);
        }

        if (shiftLine1 != null)
        {
            shiftLine1.SetActive(false);
        }

        if (options != null)
        {
            options.SetActive(false);
        }

        if (checkMessage.Length > 0)
        {
            for (int i = 0; i < checkMessage.Length; i++)
            {
                checkMessage[i].SetActive(false);
            }
        }

        if (success!= null)
        {
            success.SetActive(false);
        }

        if (fail != null)
        {
            fail.SetActive(false);
        }

        if (profile != null)
        {
            profile.SetActive(false);
        }

        GameManager.Instance.UpdateDisplayUserData();
    }

    // 로그인 패널 활성화/비활성화
    public void OnLogInButton()
    {
        logIn.SetActive(!logIn.activeSelf);
    }

    // 로그인 버튼 콜백
    public void OnLogInButtonCallBack(string email, string password)
    {
        StartCoroutine(SignInAndLoadScene(email, password));
    }

    // 회원가입 패널 활성화/비활성화
    public void OnSignUpButton()
    {
        signUp.SetActive(!signUp.activeSelf);
    }

    // 회원가입 버튼 콜백
    public void OnSignUpButtonCallBack(string email, string password)
    {
        GameManager.Instance.authManager.SignUpWithEmail(email, password, (signUpSuccess, emailSent) =>
        {
            if (signUpSuccess && emailSent)
            {
                this.email = email;
                this.password = password;
                Debug.Log("회원가입 성공 및 이메일 인증 링크 전송 완료");
                OnEmailVerificationButton(); // 이메일 인증 안내 UI 활성화
                OnSignUpButton(); // 회원가입 UI 비활성화
            }
            else if (signUpSuccess)
            {
                Debug.LogError("회원가입은 성공했지만 이메일 인증 링크 전송에 실패했습니다.");
            }
            else
            {
                Debug.LogError("회원가입 실패");
            }
        });
    }

    // 이메일 인증 패널 활성화/비활성화
    public void OnEmailVerificationButton()
    {
        emailVerification.SetActive(!emailVerification.activeSelf);
    }

    // 이메일 인증 확인 버튼 콜백
    public void OnEmailVerificationCheckCallback()
    {
        GameManager.Instance.authManager.CheckEmailVerification((isVerified) =>
        {
            if (isVerified)
            {
                Debug.Log("이메일 인증 성공");
                StartCoroutine(SignInAndLoadScene(email, password));
            }
            else
            {
                Debug.LogError("이메일 인증이 아직 완료되지 않았습니다. 이메일을 확인해주세요.");
            }
        });
    }

    // 게스트 로그인 버튼
    public void OnGuestButton()
    {
        StartCoroutine(SignInAnonymouslyAndLoadScene());
    }

    public IEnumerator SignInAndLoadScene(string email, string password)
    {
        var signInTask = FirebaseAuth.DefaultInstance.SignInWithEmailAndPasswordAsync(email, password);
        yield return new WaitUntil(() => signInTask.IsCompleted);

        if (signInTask.Exception != null)
        {
            Debug.LogError("로그인 실패: " + signInTask.Exception);
        }
        else
        {
            GameManager.Instance.OnLoginSuccess();
        }
    }

    public IEnumerator SignInAnonymouslyAndLoadScene()
    {
        // 게스트 로그인 요청
        Action<bool> onCompletion = success =>
        {
            if (success)
            {
                GameManager.Instance.LoadCurrentUserProfile(); // 사용자 데이터 로드
                GameManager.Instance.sceneManager.LoadSceneForMain(); // 게스트 로그인 성공, 씬 전환
            }
            else
            {
                Debug.LogError("게스트 로그인 실패"); // 게스트 로그인 실패
            }
        };

        GameManager.Instance.firebaseManager.SignInAnonymously(onCompletion); 
        yield return null; // 이 부분은 SignInAnonymously 메서드가 비동기로 완료될 때까지 기다리지 않고,콜백에서 모든 처리

    }

    // 닉네임 패널 활성화/비활성화
    public void OnNicknameButton()
    {
        nickname.SetActive(!nickname.activeSelf);
    }

    // 닉네임 버튼 콜백
    public void OnNicknameButtonCallBack()
    {
        GameManager.Instance.UpdateNickname(nicknameInputField.text);
        GameManager.Instance.sceneManager.LoadSceneForMain();
    }

    // 로그아웃 버튼 콜백
    public void OnLogoutButtonClick()
    {
        GameManager.Instance.LogOut();
    }

    public void OnGameSeletedButton()
    {
        GameManager.Instance.sceneManager.LoadSceneForSelectedLevel();
    }

    // Shift 버튼 콜백
    public void OnShiftButton()
    {
        isShifted = !isShifted;
        line1.SetActive(!isShifted);
        shiftLine1.SetActive(isShifted);
    }

    // Shift 버튼 상태 반환
    public bool GetIsShifted()
    {
        return isShifted;
    }

    // 프로필 패널 활성화/비활성화
    public void OnProfileButton()
    {
        if (profile != null)
        {
            profile.SetActive(!profile.activeSelf);
        }
    }

    // 한글 레벨 선택 버튼 콜백
    public void OnKoreanLevelButtonCallBack(int level)
    {
        GameManager.Instance.SetLevel(level);
        GameManager.Instance.sceneManager.LoadSceneForLevel(level);
        GameManager.Instance.UpdateDisplayUserData();
    }

    // 옵션 패널 활성화/비활성화
    public void OnOptionsButton()
    {
        if (options != null)
        {
            options.SetActive(!options.activeSelf);
        }
    }

    // 메시지 패널 활성화/비활성화
    public void CheckMessageController(int index)
    {
        if (checkMessage[index] != null)
        {
            checkMessage[index].SetActive(!checkMessage[index].activeSelf);
        }
    }

    // 성공 패널 활성화/비활성화
    public void SuccessController()
    {
        if (success != null)
        {
            success.SetActive(!success.activeSelf);
            success.transform.Find("Success").transform.Find("Answer Area").GetComponentInChildren<TextMeshProUGUI>().text = FindAnyObjectByType<AnswerLoader>().GetCurrentAnswerKey();
            successGetScore.text = GameManager.Instance.GetScore().ToString();
        }
    }

    // 실패 패널 활성화/비활성화
    public void FailController()
    {
        if (fail != null)
        {
            fail.SetActive(!fail.activeSelf);
            success.transform.Find("Fail").transform.Find("Answer Area").GetComponentInChildren<TextMeshProUGUI>().text = FindAnyObjectByType<AnswerLoader>().GetCurrentAnswerKey();
        }
    }

    // 메인으로 가는 버튼
    public void OnBackToMainButton()
    {
        GameManager.Instance.sceneManager.LoadSceneForMain();
    }

    // 레벨 선택으로 가는 버튼
    public void OnBackButton()
    {
        GameManager.Instance.sceneManager.LoadSceneForSelectedLevel();
    }

    // 게임 종료 버튼
    public void OnExitButton()
    {
        Application.Quit();
    }

    // 유저 정보 표시
    public void DisplayUserProfile(int score, int coins, Dictionary<string, int> correctAnswers, Dictionary<string, int> wrongAnswers)
    {
        if (scoreText != null)
        {
            scoreText.text = score.ToString();
        }

        if (coinsText != null)
        {
            coinsText.text = coins.ToString();
        }

        if (correctAnswersText != null && correctAnswersText.Length > 0 && GameManager.Instance.sceneManager.GetMainSceneBoolean() && correctAnswers != null)
        {
            if (nicknameText != null)
            { 
                nicknameText.text = GameManager.Instance.GetNickname();
            }

            for (int i = 0; i < correctAnswersText.Length; i++)
            {
                string levelKey = "한글 " + (i + 3).ToString() + " 레벨";
                correctAnswersText[i].text = correctAnswers.ContainsKey(levelKey) ? correctAnswers[levelKey].ToString() : "0";
            }
        }

        if (correctAnswersText != null && wrongAnswersText.Length > 0 && GameManager.Instance.sceneManager.GetMainSceneBoolean() && wrongAnswers != null)
        {
            for (int i = 0; i < wrongAnswersText.Length; i++)
            {
                string levelKey = "한글 " + (i + 3).ToString() + " 레벨";
                wrongAnswersText[i].text = wrongAnswers.ContainsKey(levelKey) ? wrongAnswers[levelKey].ToString() : "0";
            }
        }
    }
}
