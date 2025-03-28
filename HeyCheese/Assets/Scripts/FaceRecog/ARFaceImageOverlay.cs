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

    // 표정 가이드라인 Material 배열
    public Material[] EmotionMaterials;

    private void Awake()
    {
        arFaceManager = GetComponent<ARFaceManager>();
    }

    void Update()
    {
        ApplyFaceMaterial();
    }

    void ApplyFaceMaterial()
    {
        Material FaceGuideMaterial = null;

        switch (CurEmotion)
        {
            case EEmotion.Happy:
                FaceGuideMaterial = EmotionMaterials[0];
                break;
            case EEmotion.Sad:
                FaceGuideMaterial = EmotionMaterials[1];
                break;
            case EEmotion.Angry:
                FaceGuideMaterial = EmotionMaterials[2];
                break;
            case EEmotion.Surprise:
                FaceGuideMaterial = EmotionMaterials[3];
                break;
            default:
                Debug.LogWarning("Invalid Emotion!");
                break;
        }

        if (FaceGuideMaterial != null)
        {
            foreach (ARFace face in arFaceManager.trackables)
            {
                face.GetComponent<MeshRenderer>().material = FaceGuideMaterial;
            }
        }
    }
}
