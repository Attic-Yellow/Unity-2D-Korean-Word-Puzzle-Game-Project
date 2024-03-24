using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.IO;

public class AnswerLoader : MonoBehaviour
{
    public int difficulty; // ����ڰ� ������ ���̵��� �����ϴ� ����
    private Dictionary<string, string> answers = new Dictionary<string, string>();
    private string currentAnswerKey; // ���� ���ӿ����� ���� �ܾ�
    private string[] currentAnswerValue; // ���� ���ӿ����� ������ �ܾ� ����(�ʼ�, �߼�, ����) ����

    private void Start()
    {
        difficulty = GameManager.Instance.GetLevel(); // ����ڰ� ������ ���̵��� ������
        LoadAnswerFromBundle(difficulty); // ���� ���� ��, ���õ� ���̵��� �´� ���� ���� �ε�
    }

    private void LoadAnswerFromBundle(int difficulty)
    {
        // "StreamingAssets/AssetBundles" ���� ���� ���� ���� ���
        string bundleUrl = Path.Combine(Application.streamingAssetsPath, "AssetBundles", "answer");
        string assetName = $"korean{difficulty}"; // �ε��� ������ �̸�

        StartCoroutine(LoadAssetFromBundle(bundleUrl, assetName)); // ���� ���� �ε� ����
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
            
            answers = JsonConvert.DeserializeObject<Dictionary<string, string>>(loadedAsset.text); // JSON ������ �Ľ�
            
            ChooseRandomAnswer(); // ������ Ű�� �������� ����
        }
        else
        {
            Debug.LogError($"Failed to load asset: {assetName}");
        }

        
        bundle.Unload(false); // ���� ���� ��ε�
    }

    private void ChooseRandomAnswer()
    {
        List<string> keys = new List<string>(answers.Keys);
        int randomIndex = Random.Range(0, keys.Count);
        currentAnswerKey = keys[randomIndex];
        currentAnswerValue = answers[currentAnswerKey].Split('.'); // ���� ���ڿ����� '.'�� �������� �и��Ͽ� currentAnswerValue�� �Ҵ�
        Debug.Log($"{currentAnswerKey} - {currentAnswerValue[0]}, {currentAnswerValue[1]}, {currentAnswerValue[2]}"); // ���⼭ currentAnswerKey�� ���ӿ����� ������
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
