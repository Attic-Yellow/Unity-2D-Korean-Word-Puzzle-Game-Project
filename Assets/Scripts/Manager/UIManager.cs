using System.Collections;
using System.Collections.Generic;
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

    // �ѱ� ���� ���� ��ư �ݹ�
    public void OnKoreanLevelButtonCallBack(int level)
    {
        GameManager.Instance.SetLevel(level);
        GameManager.Instance.sceneManager.LoadSceneForLevel(level);
    }

    // �ɼ� �г� Ȱ��ȭ/��Ȱ��ȭ
    public void OnOptionsButton()
    {
        options.SetActive(!options.activeSelf);
    }

    // �޽��� �г� Ȱ��ȭ/��Ȱ��ȭ
    public void CheckMessageController(int index)
    {
        checkMessage[index].SetActive(!checkMessage[index].activeSelf);
    }

    // �ڷ� ���� ��ư
    public void OnBackButton()
    {
        GameManager.Instance.sceneManager.LoadSceneForSelectedLevel();
    }

    // ���� ���� ��ư
    public void OnExitButton()
    {
        Application.Quit();
    }
}
