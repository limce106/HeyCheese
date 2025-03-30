using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public enum EEmotion
{
    Happy,
    Sad,
    Angry,
    Surprise
}

public class ARFaceImageOverlay : MonoBehaviour
{
    private ARFaceManager arFaceManager;
    public EEmotion CurEmotion = EEmotion.Happy;
    private ARFace trackedFace;

    // 표정 가이드라인 Material 배열
    // 왼쪽 눈, 오른쪽, 입 순서대로 값 설정할 것!
    public GameObject[] HappyPrefabs;
    //public GameObject[] SadPrefabs;
    //public GameObject[] AngryPrefabs;
    //public GameObject[] SurprisePrefabs;

    private GameObject LeftEyePrefab;
    private GameObject RightEyePrefab;
    private GameObject MouthPrefab;

    private void Awake()
    {
        arFaceManager = GetComponent<ARFaceManager>();
    }

    private void Start()
    {
        InstantiateEmotionPrefabs();
    }

    void Update()
    {
        ApplyFaceMaterial();
    }

    void ApplyFaceMaterial()
    {
        foreach (ARFace face in arFaceManager.trackables)
        {
            // ARFace의 렌더러를 비활성화 (페이스 마스크 숨기기)
            MeshRenderer faceRenderer = face.GetComponent<MeshRenderer>();
            if (faceRenderer != null)
            {
                faceRenderer.enabled = false;
            }

            Vector3 LeftEyePos = face.leftEye.position;
            Vector3 RightEyePos = face.rightEye.position;
            Vector3 MouthPos = GetMouthCenter(face);

            if(face.leftEye != null && LeftEyePrefab != null)
            {
                LeftEyePrefab.transform.position = LeftEyePos;
                LeftEyePrefab.transform.LookAt(Camera.main.transform);
            }
            if (face.rightEye != null && RightEyePrefab != null)
            {
                RightEyePrefab.transform.position = RightEyePos;
                RightEyePrefab.transform.LookAt(Camera.main.transform);
            }
            if (MouthPrefab != null)
            {
                MouthPrefab.transform.position = MouthPos;
                MouthPrefab.transform.LookAt(Camera.main.transform);
            }
        }
    }

    Vector3 GetMouthCenter(ARFace face)
    {
        const int upperLipIndex = 13;  // 입술 위쪽 중앙
        const int lowerLipIndex = 14;  // 입술 아래쪽 중앙

        if (face.vertices.Length > lowerLipIndex)
        {
            Vector3 upperLip = face.vertices[upperLipIndex];
            Vector3 lowerLip = face.vertices[lowerLipIndex];
            return (upperLip + lowerLip) / 2; // 평균값으로 입술 중앙 계산
        }

        return Vector3.zero;
    }

    void InstantiateEmotionPrefabs()
    {
        GameObject[] CurPrefabs = null;

        switch (CurEmotion)
        {
            case EEmotion.Happy:
                CurPrefabs = HappyPrefabs;
                break;
            case EEmotion.Sad:
                //CurPrefabs = SadPrefabs;
                break;
            case EEmotion.Angry:
                //CurPrefabs = AngryPrefabs;
                break;
            case EEmotion.Surprise:
                //CurPrefabs = SurprisePrefabs;
                break;
            default:
                Debug.LogWarning("Invalid Emotion!");
                break;
        }

        if(CurPrefabs != null)
        {
            LeftEyePrefab = Instantiate(CurPrefabs[0]);
            RightEyePrefab = Instantiate(CurPrefabs[1]);
            MouthPrefab = Instantiate(CurPrefabs[2]);
        }
    }
}
