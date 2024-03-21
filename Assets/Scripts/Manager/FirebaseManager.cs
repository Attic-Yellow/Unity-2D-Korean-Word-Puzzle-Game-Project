using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Firestore;
using Firebase.Extensions;

public class FirebaseManager : MonoBehaviour
{
    FirebaseFirestore db;
    public bool IsFirebaseInitialized = false; // 초기화 상태 플래그

    private void Awake()
    {
        GameManager.Instance.firebaseManager = this;

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                db = FirebaseFirestore.DefaultInstance;
                IsFirebaseInitialized = true; // 초기화 완료
                Debug.Log("Firebase를 사용할 준비가 됨");
            }
            else
            {
                Debug.LogError($"모든 Firebase 종속성을 해결할 수 없음: {dependencyStatus}");
            }
        });
    }

    public void CheckFieldValueExists(string userValue, System.Action<bool> onResult)
    {
        if (!IsFirebaseInitialized)
        {
            Debug.LogError("Firebase가 아직 초기화되지 않았음");
            onResult(false);
            return;
        }

        DocumentReference docRef = db.Collection("answer").Document("korean3");
        docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("문서 조회 중 오류 발생");
                onResult(false);
                return;
            }

            var documentSnapshot = task.Result;
            if (!documentSnapshot.Exists)
            {
                Debug.Log("korean 문서가 존재하지 않습니다.");
                onResult(false);
                return;
            }

            // 사용자 입력 값이 필드로 존재하는지 확인
            if (documentSnapshot.ContainsField(userValue))
            {
                // print($"필드 '{userValue}'가 존재함");
                onResult(true);
            }
            else
            {
                // print($"필드 '{userValue}'가 존재하지 않음);
                onResult(false);
            }
        });
    }
}
