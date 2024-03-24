using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RankingEntryUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI rankText; // 등수 텍스트
    [SerializeField] private TextMeshProUGUI nicknameText; // 닉네임 텍스트
    [SerializeField] private TextMeshProUGUI scoreText; // 점수 텍스트

    // Setup 메서드를 수정하여 등수도 받을 수 있도록 함
    public void Setup(int rank, string nickname, int score)
    {
        rankText.text = $"{rank}";
        nicknameText.text = nickname;
        scoreText.text = score.ToString();
    }
}
