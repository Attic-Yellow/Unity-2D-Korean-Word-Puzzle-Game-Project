#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;

public class AssetBundleCreator : Editor
{
    [MenuItem("Firestore/Build Answer AssetBundle")]
    static void BuildAllAssetBundles()
    {
        string assetBundleDirectory = "Assets/AssetBundles";
        if (!Directory.Exists(assetBundleDirectory))
        {
            Directory.CreateDirectory(assetBundleDirectory);
        }

        // "Assets/Answer" ���� ���� ��� ���¿� ���� "Answer"�̶�� ���� ���� �̸��� �Ҵ�
        AssignAssetBundleName("Assets/Answer", "Answer");

        // ���� ������ ����
        BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);

        Debug.Log("AssetBundle has been created at: " + assetBundleDirectory);

        // ���� �����ͺ��̽��� ����
        AssetDatabase.Refresh();
    }

    static void AssignAssetBundleName(string path, string assetBundleName)
    {
        var assetPaths = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
        foreach (var assetPath in assetPaths)
        {
            // ��Ÿ ������ ����
            if (assetPath.EndsWith(".meta")) continue;

            var importer = AssetImporter.GetAtPath(assetPath);
            if (importer != null)
            {
                importer.assetBundleName = assetBundleName;
            }
        }
    }
}
#endif
