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

        // "Assets/Answer" 폴더 내의 모든 에셋에 대해 "Answer"이라는 에셋 번들 이름을 할당
        AssignAssetBundleName("Assets/Answer", "Answer");

        // 에셋 번들을 빌드
        BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);

        Debug.Log("AssetBundle has been created at: " + assetBundleDirectory);

        // 에셋 데이터베이스를 갱신
        AssetDatabase.Refresh();
    }

    static void AssignAssetBundleName(string path, string assetBundleName)
    {
        var assetPaths = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
        foreach (var assetPath in assetPaths)
        {
            // 메타 파일을 제외
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
