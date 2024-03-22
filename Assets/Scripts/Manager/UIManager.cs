using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] private AnswerLoader answerLoader;
    [SerializeField] private GameObject line1;
    [SerializeField] private GameObject shiftLine1;
    [SerializeField] private bool isShifted = false;
    [SerializeField] private GameObject options;
    [SerializeField] private GameObject[] checkMessage;
    [SerializeField] private GameObject success;


    private void Awake()
    {
        GameManager.Instance.uiManager = this;
    }

    private void Start()
    {
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

    // 한글 레벨 선택 버튼 콜백
    public void OnKoreanLevelButtonCallBack(int level)
    {
        GameManager.Instance.SetLevel(level);
        GameManager.Instance.sceneManager.LoadSceneForLevel(level);
    }

    // 옵션 패널 활성화/비활성화
    public void OnOptionsButton()
    {
        options.SetActive(!options.activeSelf);
    }

    // 메시지 패널 활성화/비활성화
    public void CheckMessageController(int index)
    {
        checkMessage[index].SetActive(!checkMessage[index].activeSelf);
    }

    // 성공 패널 활성화/비활성화
    public void SuccessController()
    {
        success.SetActive(!success.activeSelf);
        success.transform.Find("Success").transform.Find("Answer Area").GetComponentInChildren<TextMeshProUGUI>().text = FindAnyObjectByType<AnswerLoader>().GetCurrentAnswerKey();
    }

    // 뒤로 가기 버튼
    public void OnBackButton()
    {
        GameManager.Instance.sceneManager.LoadSceneForSelectedLevel();
    }

    // 게임 종료 버튼
    public void OnExitButton()
    {
        Application.Quit();
    }
}
