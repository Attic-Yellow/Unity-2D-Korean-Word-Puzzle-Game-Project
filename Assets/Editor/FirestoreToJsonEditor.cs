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
    // ���� ID ����� �����մϴ�.
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

            // Answer ������ �ִ��� Ȯ���ϰ�, ������ �����մϴ�.
            string folderPath = Path.Combine(Application.dataPath, "Answer");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // JSON ������ ���� ID �̸����� Answer ������ �����մϴ�.
            string filePath = Path.Combine(folderPath, $"{documentId}.json");
            File.WriteAllText(filePath, json);

            UnityEngine.Debug.Log($"Data exported to {filePath}");
        });

        // Unity �����Ͱ� �� ������ �ν��ϵ��� �����մϴ�. ��ġ ������ �ʿ��� ��� ������ �̵�
        AssetDatabase.Refresh();
    }
}
#endif
