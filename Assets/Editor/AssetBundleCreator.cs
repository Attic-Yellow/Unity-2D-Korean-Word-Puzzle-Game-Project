#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;

public class AssetBundleCreator : Editor
{
    [MenuItem("Firestore/Build Answer AssetBundle")]
    static void BuildAllAssetBundles()
    {
        // "Assets/StreamingAssets/AssetBundles" 폴더로 경로 변경
        string assetBundleDirectory = Path.Combine(Application.streamingAssetsPath, "AssetBundles");
        if (!Directory.Exists(assetBundleDirectory))
        {
            Directory.CreateDirectory(assetBundleDirectory);
        }

        AssignAssetBundleName("Assets/Answer", "answer"); // 소문자로 변경하여 일관성 유지

        BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);

        Debug.Log("AssetBundle has been created at: " + assetBundleDirectory);
        AssetDatabase.Refresh();
    }

    static void AssignAssetBundleName(string path, string assetBundleName)
    {
        var assetPaths = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
        foreach (var assetPath in assetPaths)
        {
            if (assetPath.EndsWith(".meta")) continue; // 메타 파일 제외

            var importer = AssetImporter.GetAtPath(assetPath);
            if (importer != null)
            {
                importer.assetBundleName = assetBundleName;
            }
        }
    }
}
#endif
