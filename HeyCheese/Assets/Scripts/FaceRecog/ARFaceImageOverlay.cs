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
    // 왼쪽 눈썹, 오른쪽 눈썹, 왼쪽 눈, 오른쪽 눈, 입 순서대로 값 설정할 것!
    public GameObject[] HappyPrefabs;
    public GameObject[] SadPrefabs;
    public GameObject[] AngryPrefabs;
    public GameObject[] SurprisePrefabs;

    private GameObject LeftEyebrowSprite;
    private GameObject RightEyebrowSprite;
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
            Vector3 LeftEyebrowLocal = face.vertices[223];
            Vector3 RightEyebrowLocal = face.vertices[443];
            Vector3 LeftEyeLocal = face.vertices[133];
            Vector3 RightEyeLocal = face.vertices[362];
            Vector3 MouthLocal = GetMouthCenterLocal(face);

            // 위치 보정
            LeftEyebrowLocal.y += 0.01f;
            RightEyebrowLocal.y += 0.01f;
            LeftEyeLocal.x -= 0.02f;
            RightEyeLocal.x += 0.02f;

            Vector3 LeftEyebrowWorldPos = face.transform.TransformPoint(LeftEyebrowLocal);
            Vector3 RightEyebrowWorldPos = face.transform.TransformPoint(RightEyebrowLocal);
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

            if (LeftEyebrowSprite != null)
            {
                LeftEyebrowSprite.transform.position = LeftEyebrowWorldPos;
                LeftEyebrowSprite.transform.rotation = adjustedRotation;
            }

            if (RightEyebrowSprite != null)
            {
                RightEyebrowSprite.transform.position = RightEyebrowWorldPos;
                RightEyebrowSprite.transform.rotation = adjustedRotation;
            }

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
                CurPrefabs = SadPrefabs;
                break;
            case EEmotion.Angry:
                CurPrefabs = AngryPrefabs;
                break;
            case EEmotion.Surprise:
                CurPrefabs = SurprisePrefabs;
                break;
            default:
                Debug.LogWarning("Invalid Emotion!");
                break;
        }

        if(CurPrefabs != null)
        {
            LeftEyebrowSprite = Instantiate(CurPrefabs[0], Vector3.zero, Quaternion.identity);
            RightEyebrowSprite = Instantiate(CurPrefabs[1], Vector3.zero, Quaternion.identity);
            LeftEyeSprite = Instantiate(CurPrefabs[2], Vector3.zero, Quaternion.identity);
            RightEyeSprite = Instantiate(CurPrefabs[3], Vector3.zero, Quaternion.identity);
            MouthSprite = Instantiate(CurPrefabs[4], Vector3.zero, Quaternion.identity);

            LeftEyeSprite.transform.SetParent(transform);
            RightEyeSprite.transform.SetParent(transform);
            MouthSprite.transform.SetParent(transform);
        }
    }

    void SetPrefabVisibility(bool isVisible)
    {
        if (LeftEyebrowSprite != null)
            LeftEyebrowSprite.SetActive(isVisible);
        if (RightEyebrowSprite != null)
            RightEyebrowSprite.SetActive(isVisible);
        if (LeftEyeSprite != null)
            LeftEyeSprite.SetActive(isVisible);
        if (RightEyeSprite != null) 
            RightEyeSprite.SetActive(isVisible);
        if (MouthSprite != null) 
            MouthSprite.SetActive(isVisible);
    }
}
