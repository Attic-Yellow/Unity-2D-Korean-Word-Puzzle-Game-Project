using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Firestore;
using Firebase.Extensions;

public class FirebaseManager : MonoBehaviour
{
    FirebaseFirestore db;
    public bool IsFirebaseInitialized = false; // �ʱ�ȭ ���� �÷���

    private void Awake()
    {
        GameManager.Instance.firebaseManager = this;

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                db = FirebaseFirestore.DefaultInstance;
                IsFirebaseInitialized = true; // �ʱ�ȭ �Ϸ�
                Debug.Log("Firebase�� ����� �غ� ��");
            }
            else
            {
                Debug.LogError($"��� Firebase ���Ӽ��� �ذ��� �� ����: {dependencyStatus}");
            }
        });
    }

    public void CheckFieldValueExists(string userValue, System.Action<bool> onResult)
    {
        if (!IsFirebaseInitialized)
        {
            Debug.LogError("Firebase�� ���� �ʱ�ȭ���� �ʾ���");
            onResult(false);
            return;
        }

        DocumentReference docRef = db.Collection("answer").Document("korean3");
        docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("���� ��ȸ �� ���� �߻�");
                onResult(false);
                return;
            }

            var documentSnapshot = task.Result;
            if (!documentSnapshot.Exists)
            {
                Debug.Log("korean ������ �������� �ʽ��ϴ�.");
                onResult(false);
                return;
            }

            // ����� �Է� ���� �ʵ�� �����ϴ��� Ȯ��
            if (documentSnapshot.ContainsField(userValue))
            {
                // print($"�ʵ� '{userValue}'�� ������");
                onResult(true);
            }
            else
            {
                // print($"�ʵ� '{userValue}'�� �������� ����);
                onResult(false);
            }
        });
    }
}
