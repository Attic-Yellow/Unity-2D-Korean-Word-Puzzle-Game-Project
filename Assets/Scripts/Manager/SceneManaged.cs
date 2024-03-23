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

    // 레벨에 해당하는 씬을 로드하는 메소드
    public void LoadSceneForLevel(int level)
    {
        string sceneName = LevelToSceneName(level);
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError("해당 레벨에 맞는 씬 이름을 찾을 수 없습니다: " + level);
        }
    }

    // 레벨 번호를 씬 이름으로 변환
    private string LevelToSceneName(int level)
    {
        switch (level)
        {
            case 3: return "Korean Level 3 Game Scene";
            case 4: return "Korean Level 4 Game Scene";
            case 5: return "Korean Level 5 Game Scene";
            default:
                return ""; // 유효하지 않은 레벨 번호일 경우
        }
    }

    public bool GetMainSceneBoolean()
    {
        return SceneManager.GetActiveScene().name == "Main Scene";
    }
}