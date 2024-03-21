using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class AnswerLoader : MonoBehaviour
{
    public int difficulty = 3; // ����ڰ� ������ ���̵��� �����ϴ� ����
    private Dictionary<string, string> answers = new Dictionary<string, string>();
    private string currentAnswerKey; // ���� ���ӿ����� ���� Ű

    private void Start()
    {
        LoadAnswerFromBundle(difficulty); // ���� ���� ��, ���õ� ���̵��� �´� ���� ���� �ε�
    }

    private void LoadAnswerFromBundle(int difficulty)
    {
        string bundleUrl = $"Assets/AssetBundles/answer"; // ���� ������ ���
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
        Debug.Log($"Current answer key: {currentAnswerKey} - Value: {answers[currentAnswerKey]}"); // ���⼭ currentAnswerKey�� ���ӿ����� ������
    }

    public string GetCurrentAnswerKey()
    {
        return currentAnswerKey;
    }
}
