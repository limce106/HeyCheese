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

            Vector3 LeftEyePos = face.transform.TransformPoint(face.vertices[133]);
            Vector3 RightEyePos = face.transform.TransformPoint(face.vertices[362]);
            Vector3 MouthPos = face.transform.TransformPoint(face.vertices[13]);

            // 눈 위치 보정
            LeftEyePos.x -= 0.02f;
            RightEyePos.x += 0.02f;

            Quaternion faceRotation = face.transform.rotation;

            if (LeftEyeSprite != null)
            {
                LeftEyeSprite.transform.position = LeftEyePos;
                // 얼굴 회전에 맞춰 회전
                LeftEyeSprite.transform.rotation = faceRotation;
            }
            if (RightEyeSprite != null)
            {
                RightEyeSprite.transform.position = RightEyePos;
                RightEyeSprite.transform.rotation = faceRotation;
            }
            if (MouthSprite != null)
            {
                MouthSprite.transform.position = MouthPos;
                MouthSprite.transform.rotation = faceRotation;
            }

            SetPrefabVisibility(true);
        }
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
