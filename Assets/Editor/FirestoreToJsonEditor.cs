#if UNITY_EDITOR
using UnityEditor;
using Firebase.Firestore;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using Firebase.Extensions;
using UnityEngine;

public class FirestoreToJsonEditor : EditorWindow
{
    // 문서 ID 목록을 정의합니다.
    private static readonly string[] documentIds = { "korean3", "korean4", "korean5" };

    [MenuItem("Firestore/Export Data to JSON")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(FirestoreToJsonEditor));
        foreach (var documentId in documentIds)
        {
            ExportFirestoreDataToJson(documentId);
        }
    }

    private static void ExportFirestoreDataToJson(string documentId)
    {
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;

        db.Collection("answer").Document(documentId).GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                UnityEngine.Debug.LogError($"Error fetching document: {documentId}");
                return;
            }

            DocumentSnapshot snapshot = task.Result;
            if (!snapshot.Exists)
            {
                UnityEngine.Debug.Log($"Document does not exist: {documentId}");
                return;
            }

            Dictionary<string, object> documentData = snapshot.ToDictionary();
            string json = JsonConvert.SerializeObject(documentData, Formatting.Indented);

            // Answer 폴더가 있는지 확인하고, 없으면 생성합니다.
            string folderPath = Path.Combine(Application.dataPath, "Answer");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // JSON 파일을 문서 ID 이름으로 Answer 폴더에 저장합니다.
            string filePath = Path.Combine(folderPath, $"{documentId}.json");
            File.WriteAllText(filePath, json);

            UnityEngine.Debug.Log($"Data exported to {filePath}");
        });

        // Unity 에디터가 새 파일을 인식하도록 갱신합니다. 위치 조정이 필요할 경우 밖으로 이동
        AssetDatabase.Refresh();
    }
}
#endif
