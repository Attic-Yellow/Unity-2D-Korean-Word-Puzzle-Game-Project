using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class SceneManaged : MonoBehaviour
{
    private void Awake()
    {
        GameManager.Instance.sceneManager = this;
    }

    public void LoadSceneForLogin()
    {
        SceneManager.LoadScene("Login Scene");
    }

    public void LoadSceneForMain()
    {
        SceneManager.LoadScene("Main Scene");
    }

    public void LoadSceneForSelectedLevel()
    {
        SceneManager.LoadScene("Languaged and Level Selected Scene");
    }

    // ������ �ش��ϴ� ���� �ε��ϴ� �޼ҵ�
    public void LoadSceneForLevel(int level)
    {
        string sceneName = LevelToSceneName(level);
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError("�ش� ������ �´� �� �̸��� ã�� �� �����ϴ�: " + level);
        }
    }

    // ���� ��ȣ�� �� �̸����� ��ȯ
    private string LevelToSceneName(int level)
    {
        switch (level)
        {
            case 3: return "Korean Level 3 Game Scene";
            case 4: return "Korean Level 4 Game Scene";
            case 5: return "Korean Level 5 Game Scene";
            default:
                return ""; // ��ȿ���� ���� ���� ��ȣ�� ���
        }
    }

    public bool GetMainSceneBoolean()
    {
        return SceneManager.GetActiveScene().name == "Main Scene";
    }
}