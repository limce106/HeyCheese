using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

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

    private GameObject LeftEyeSprite;
    private GameObject RightEyeSprite;
    private GameObject MouthSprite;

    private void Awake()
    {
        arFaceManager = GetComponent<ARFaceManager>();
    }

    void Update()
    {
        // 감지된 얼굴이 없으면
        if (arFaceManager.trackables.count == 0)
        {
            SetPrefabVisibility(false);
            return;
        }

        // 처음으로 얼굴이 감지될 때 프리팹 생성
        if (LeftEyeSprite == null || RightEyeSprite == null || MouthSprite == null)
        {
            InstantiateEmotionPrefabs();
        }

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

            // 얼굴 추적이 안 되면
            if (face.trackingState != TrackingState.Tracking)
            {
                SetPrefabVisibility(false);
                return;
            }

            // 얼굴의 로컬 정점 위치 가져오기
            Vector3 LeftEyeLocal = face.vertices[133];  // 왼쪽 눈
            Vector3 RightEyeLocal = face.vertices[362]; // 오른쪽 눈
            Vector3 MouthLocal = GetMouthCenterLocal(face);     // 입

            // 눈 위치 보정
            LeftEyeLocal.x -= 0.015f;
            RightEyeLocal.x += 0.015f;

            Vector3 LeftEyeWorldPos = face.transform.TransformPoint(LeftEyeLocal);
            Vector3 RightEyeWorldPos = face.transform.TransformPoint(RightEyeLocal);
            Vector3 MouthWorldPos = face.transform.TransformPoint(MouthLocal);

            Quaternion faceRotation = face.transform.rotation;

            // 얼굴이 바라보는 방향
            Vector3 faceForward = face.transform.forward;
            // 얼굴이 바라보는 방향
            Vector3 faceUp = face.transform.up;

            // 카메라 회전 보정 (카메라가 회전해도 스프라이트가 이상하지 않도록)
            Quaternion inverseCameraRotation = Quaternion.Inverse(Camera.main.transform.rotation);
            Quaternion adjustedRotation = inverseCameraRotation * faceRotation;

            if (LeftEyeSprite != null)
            {
                LeftEyeSprite.transform.position = LeftEyeWorldPos;
                LeftEyeSprite.transform.rotation = adjustedRotation;
            }

            if (RightEyeSprite != null)
            {
                RightEyeSprite.transform.position = RightEyeWorldPos;
                RightEyeSprite.transform.rotation = adjustedRotation;
            }

            if (MouthSprite != null)
            {
                MouthSprite.transform.position = MouthWorldPos;
                MouthSprite.transform.rotation = adjustedRotation;
            }

            SetPrefabVisibility(true);
        }
    }

    Vector3 GetMouthCenterLocal(ARFace face)
    {
        const int UpperLipIndex = 13;
        const int LowerLipIndex = 14;

        if (face.vertices.Length > LowerLipIndex)
        {
            Vector3 UpperLip = face.vertices[UpperLipIndex];
            Vector3 LowerLip = face.vertices[LowerLipIndex];
            return (UpperLip + LowerLip) / 2;
        }

        Debug.LogWarning("Can't Find Mouth Center!");
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
            LeftEyeSprite = Instantiate(CurPrefabs[0], Vector3.zero, Quaternion.identity);
            RightEyeSprite = Instantiate(CurPrefabs[1], Vector3.zero, Quaternion.identity);
            MouthSprite = Instantiate(CurPrefabs[2], Vector3.zero, Quaternion.identity);

            LeftEyeSprite.transform.SetParent(transform);
            RightEyeSprite.transform.SetParent(transform);
            MouthSprite.transform.SetParent(transform);
        }
    }

    void SetPrefabVisibility(bool isVisible)
    {
        if (LeftEyeSprite != null)
            LeftEyeSprite.SetActive(isVisible);
        if (RightEyeSprite != null) 
            RightEyeSprite.SetActive(isVisible);
        if (MouthSprite != null) 
            MouthSprite.SetActive(isVisible);
    }
}
