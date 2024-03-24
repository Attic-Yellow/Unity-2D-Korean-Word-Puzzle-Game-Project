#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;

public class AssetBundleCreator : Editor
{
    [MenuItem("Firestore/Build Answer AssetBundle")]
    static void BuildAllAssetBundles()
    {
        // "Assets/StreamingAssets/AssetBundles" ������ ��� ����
        string assetBundleDirectory = Path.Combine(Application.streamingAssetsPath, "AssetBundles");
        if (!Directory.Exists(assetBundleDirectory))
        {
            Directory.CreateDirectory(assetBundleDirectory);
        }

        AssignAssetBundleName("Assets/Answer", "answer"); // �ҹ��ڷ� �����Ͽ� �ϰ��� ����

        BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);

        Debug.Log("AssetBundle has been created at: " + assetBundleDirectory);
        AssetDatabase.Refresh();
    }

    static void AssignAssetBundleName(string path, string assetBundleName)
    {
        var assetPaths = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
        foreach (var assetPath in assetPaths)
        {
            if (assetPath.EndsWith(".meta")) continue; // ��Ÿ ���� ����

            var importer = AssetImporter.GetAtPath(assetPath);
            if (importer != null)
            {
                importer.assetBundleName = assetBundleName;
            }
        }
    }
}
#endif
