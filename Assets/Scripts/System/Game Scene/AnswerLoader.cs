using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.IO;

public class AnswerLoader : MonoBehaviour
{
    public int difficulty; // 사용자가 선택한 난이도를 저장하는 변수
    private Dictionary<string, string> answers = new Dictionary<string, string>();
    private string currentAnswerKey; // 현재 게임에서의 정답 단어
    private string[] currentAnswerValue; // 현재 게임에서의 정답의 단어 글자(초성, 중성, 종성) 조합

    private void Start()
    {
        difficulty = GameManager.Instance.GetLevel(); // 사용자가 선택한 난이도를 가져옴
        LoadAnswerFromBundle(difficulty); // 게임 시작 시, 선택된 난이도에 맞는 정답 파일 로드
    }

    private void LoadAnswerFromBundle(int difficulty)
    {
        // "StreamingAssets/AssetBundles" 폴더 안의 에셋 번들 경로
        string bundleUrl = Path.Combine(Application.streamingAssetsPath, "AssetBundles", "answer");
        string assetName = $"korean{difficulty}"; // 로드할 에셋의 이름

        StartCoroutine(LoadAssetFromBundle(bundleUrl, assetName)); // 에셋 번들 로드 시작
    }

    private IEnumerator LoadAssetFromBundle(string bundleUrl, string assetName)
    {
        AssetBundleCreateRequest bundleLoadRequest = AssetBundle.LoadFromFileAsync(bundleUrl);
        yield return bundleLoadRequest;

        AssetBundle bundle = bundleLoadRequest.assetBundle;

        if (bundle == null)
        {
            Debug.LogError("Failed to load AssetBundle!");
            yield break;
        }

        AssetBundleRequest assetLoadRequest = bundle.LoadAssetAsync<TextAsset>(assetName);
        yield return assetLoadRequest;

        TextAsset loadedAsset = assetLoadRequest.asset as TextAsset;

        if (loadedAsset != null)
        {
            
            answers = JsonConvert.DeserializeObject<Dictionary<string, string>>(loadedAsset.text); // JSON 데이터 파싱
            
            ChooseRandomAnswer(); // 랜덤한 키를 정답으로 선택
        }
        else
        {
            Debug.LogError($"Failed to load asset: {assetName}");
        }

        
        bundle.Unload(false); // 에셋 번들 언로드
    }

    private void ChooseRandomAnswer()
    {
        List<string> keys = new List<string>(answers.Keys);
        int randomIndex = Random.Range(0, keys.Count);
        currentAnswerKey = keys[randomIndex];
        currentAnswerValue = answers[currentAnswerKey].Split('.'); // 정답 문자열에서 '.'을 기준으로 분리하여 currentAnswerValue에 할당
        Debug.Log($"{currentAnswerKey} - {currentAnswerValue[0]}, {currentAnswerValue[1]}, {currentAnswerValue[2]}"); // 여기서 currentAnswerKey가 게임에서의 정답임
    }

    public string GetCurrentAnswerKey()
    {
        return currentAnswerKey;
    }

    public string[] GetCurrentAnswerValue()
    {
        return currentAnswerValue;
    }
}
