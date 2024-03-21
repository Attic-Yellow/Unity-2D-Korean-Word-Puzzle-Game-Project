using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private AnswerLoader answerLoader;

    [SerializeField] private GameObject line1;
    [SerializeField] private GameObject shiftLine1;

    [SerializeField] private bool isShifted = false;


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
    }

    public void OnShiftButton()
    {
        isShifted = !isShifted;
        line1.SetActive(!isShifted);
        shiftLine1.SetActive(isShifted);
    }

    public bool GetIsShifted()
    {
        return isShifted;
    }
}
