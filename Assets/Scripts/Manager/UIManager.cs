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
    [Header("�α��� ��")]
    [SerializeField] private GameObject logIn;
    [SerializeField] private GameObject signUp;
    [SerializeField] private GameObject nickname;
    [SerializeField] private GameObject emailVerification;
    [SerializeField] private TMP_InputField nicknameInputField;
    [SerializeField] private string email;
    [SerializeField] private string password;

    [Header("���� ��")]
    [SerializeField] private GameObject profile;

    [Header("���� ��")]
    [SerializeField] private AnswerLoader answerLoader;
    [SerializeField] private GameObject line1;
    [SerializeField] private GameObject shiftLine1;
    [SerializeField] private GameObject success;
    [SerializeField] private GameObject fail;
    [SerializeField] private bool isShifted = false;
    [SerializeField] private TextMeshProUGUI successGetScore;

    [Header("����")]
    [SerializeField] private GameObject options;
    [SerializeField] private GameObject[] checkMessage;

    [Header("���� ����")]
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

    // �α��� �г� Ȱ��ȭ/��Ȱ��ȭ
    public void OnLogInButton()
    {
        logIn.SetActive(!logIn.activeSelf);
    }

    // �α��� ��ư �ݹ�
    public void OnLogInButtonCallBack(string email, string password)
    {
        StartCoroutine(SignInAndLoadScene(email, password));
    }

    // ȸ������ �г� Ȱ��ȭ/��Ȱ��ȭ
    public void OnSignUpButton()
    {
        signUp.SetActive(!signUp.activeSelf);
    }

    // ȸ������ ��ư �ݹ�
    public void OnSignUpButtonCallBack(string email, string password)
    {
        GameManager.Instance.authManager.SignUpWithEmail(email, password, (signUpSuccess, emailSent) =>
        {
            if (signUpSuccess && emailSent)
            {
                this.email = email;
                this.password = password;
                Debug.Log("ȸ������ ���� �� �̸��� ���� ��ũ ���� �Ϸ�");
                OnEmailVerificationButton(); // �̸��� ���� �ȳ� UI Ȱ��ȭ
                OnSignUpButton(); // ȸ������ UI ��Ȱ��ȭ
            }
            else if (signUpSuccess)
            {
                Debug.LogError("ȸ�������� ���������� �̸��� ���� ��ũ ���ۿ� �����߽��ϴ�.");
            }
            else
            {
                Debug.LogError("ȸ������ ����");
            }
        });
    }

    // �̸��� ���� �г� Ȱ��ȭ/��Ȱ��ȭ
    public void OnEmailVerificationButton()
    {
        emailVerification.SetActive(!emailVerification.activeSelf);
    }

    // �̸��� ���� Ȯ�� ��ư �ݹ�
    public void OnEmailVerificationCheckCallback()
    {
        GameManager.Instance.authManager.CheckEmailVerification((isVerified) =>
        {
            if (isVerified)
            {
                Debug.Log("�̸��� ���� ����");
                StartCoroutine(SignInAndLoadScene(email, password));
            }
            else
            {
                Debug.LogError("�̸��� ������ ���� �Ϸ���� �ʾҽ��ϴ�. �̸����� Ȯ�����ּ���.");
            }
        });
    }

    // �Խ�Ʈ �α��� ��ư
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
            Debug.LogError("�α��� ����: " + signInTask.Exception);
        }
        else
        {
            GameManager.Instance.OnLoginSuccess();
        }
    }

    public IEnumerator SignInAnonymouslyAndLoadScene()
    {
        // �Խ�Ʈ �α��� ��û
        Action<bool> onCompletion = success =>
        {
            if (success)
            {
                GameManager.Instance.LoadCurrentUserProfile(); // ����� ������ �ε�
                GameManager.Instance.sceneManager.LoadSceneForMain(); // �Խ�Ʈ �α��� ����, �� ��ȯ
            }
            else
            {
                Debug.LogError("�Խ�Ʈ �α��� ����"); // �Խ�Ʈ �α��� ����
            }
        };

        GameManager.Instance.firebaseManager.SignInAnonymously(onCompletion); 
        yield return null; // �� �κ��� SignInAnonymously �޼��尡 �񵿱�� �Ϸ�� ������ ��ٸ��� �ʰ�,�ݹ鿡�� ��� ó��

    }

    // �г��� �г� Ȱ��ȭ/��Ȱ��ȭ
    public void OnNicknameButton()
    {
        nickname.SetActive(!nickname.activeSelf);
    }

    // �г��� ��ư �ݹ�
    public void OnNicknameButtonCallBack()
    {
        GameManager.Instance.UpdateNickname(nicknameInputField.text);
        GameManager.Instance.sceneManager.LoadSceneForMain();
    }

    // �α׾ƿ� ��ư �ݹ�
    public void OnLogoutButtonClick()
    {
        GameManager.Instance.LogOut();
    }

    public void OnGameSeletedButton()
    {
        GameManager.Instance.sceneManager.LoadSceneForSelectedLevel();
    }

    // Shift ��ư �ݹ�
    public void OnShiftButton()
    {
        isShifted = !isShifted;
        line1.SetActive(!isShifted);
        shiftLine1.SetActive(isShifted);
    }

    // Shift ��ư ���� ��ȯ
    public bool GetIsShifted()
    {
        return isShifted;
    }

    // ������ �г� Ȱ��ȭ/��Ȱ��ȭ
    public void OnProfileButton()
    {
        if (profile != null)
        {
            profile.SetActive(!profile.activeSelf);
        }
    }

    // �ѱ� ���� ���� ��ư �ݹ�
    public void OnKoreanLevelButtonCallBack(int level)
    {
        GameManager.Instance.SetLevel(level);
        GameManager.Instance.sceneManager.LoadSceneForLevel(level);
        GameManager.Instance.UpdateDisplayUserData();
    }

    // �ɼ� �г� Ȱ��ȭ/��Ȱ��ȭ
    public void OnOptionsButton()
    {
        if (options != null)
        {
            options.SetActive(!options.activeSelf);
        }
    }

    // �޽��� �г� Ȱ��ȭ/��Ȱ��ȭ
    public void CheckMessageController(int index)
    {
        if (checkMessage[index] != null)
        {
            checkMessage[index].SetActive(!checkMessage[index].activeSelf);
        }
    }

    // ���� �г� Ȱ��ȭ/��Ȱ��ȭ
    public void SuccessController()
    {
        if (success != null)
        {
            success.SetActive(!success.activeSelf);
            success.transform.Find("Success").transform.Find("Answer Area").GetComponentInChildren<TextMeshProUGUI>().text = FindAnyObjectByType<AnswerLoader>().GetCurrentAnswerKey();
            successGetScore.text = GameManager.Instance.GetScore().ToString();
        }
    }

    // ���� �г� Ȱ��ȭ/��Ȱ��ȭ
    public void FailController()
    {
        if (fail != null)
        {
            fail.SetActive(!fail.activeSelf);
            success.transform.Find("Fail").transform.Find("Answer Area").GetComponentInChildren<TextMeshProUGUI>().text = FindAnyObjectByType<AnswerLoader>().GetCurrentAnswerKey();
        }
    }

    // �������� ���� ��ư
    public void OnBackToMainButton()
    {
        GameManager.Instance.sceneManager.LoadSceneForMain();
    }

    // ���� �������� ���� ��ư
    public void OnBackButton()
    {
        GameManager.Instance.sceneManager.LoadSceneForSelectedLevel();
    }

    // ���� ���� ��ư
    public void OnExitButton()
    {
        Application.Quit();
    }

    // ���� ���� ǥ��
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
                string levelKey = "�ѱ� " + (i + 3).ToString() + " ����";
                correctAnswersText[i].text = correctAnswers.ContainsKey(levelKey) ? correctAnswers[levelKey].ToString() : "0";
            }
        }

        if (correctAnswersText != null && wrongAnswersText.Length > 0 && GameManager.Instance.sceneManager.GetMainSceneBoolean() && wrongAnswers != null)
        {
            for (int i = 0; i < wrongAnswersText.Length; i++)
            {
                string levelKey = "�ѱ� " + (i + 3).ToString() + " ����";
                wrongAnswersText[i].text = wrongAnswers.ContainsKey(levelKey) ? wrongAnswers[levelKey].ToString() : "0";
            }
        }
    }
}
