using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RankingEntryUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI rankText; // ��� �ؽ�Ʈ
    [SerializeField] private TextMeshProUGUI nicknameText; // �г��� �ؽ�Ʈ
    [SerializeField] private TextMeshProUGUI scoreText; // ���� �ؽ�Ʈ

    // Setup �޼��带 �����Ͽ� ����� ���� �� �ֵ��� ��
    public void Setup(int rank, string nickname, int score)
    {
        rankText.text = $"{rank}";
        nicknameText.text = nickname;
        scoreText.text = score.ToString();
    }
}
