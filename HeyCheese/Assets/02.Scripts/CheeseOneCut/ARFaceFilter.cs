using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARFaceFilter : MonoBehaviour
{
    private ARFaceManager arFaceManager;

    private GameObject leftEyePrefab;
    private GameObject rightEyePrefab;
    private GameObject nosePrefab;
    private GameObject foreheadPrefab;

    private string filterName;

    const int leftEyeIndex = 133;
    const int rightEyeIndex = 362;
    const int noseIndex = 2;
    const int foreheadIndex = 10;


    void Update()
    {
        // 필터가 선택되지 않았다면
        if (filterName == null)
        {
            return;
        }
        // 감지된 얼굴이 없으면
        if (arFaceManager.trackables.count == 0)
        {
            SetPrefabVisibility(false);
            return;
        }

        // 처음으로 얼굴이 감지될 때 프리팹 생성
        if (leftEyePrefab == null || rightEyePrefab == null || nosePrefab == null || foreheadPrefab == null)
        {
            InstantiateFilterPrefabs();
        }

        ApplyFilter();
    }

    public void SetFilterName(string name)
    {
        filterName = name;
    }

    public void OnClick_Filter()
    {
        if(arFaceManager == null)
        {
            arFaceManager = GetComponent<ARFaceManager>();
        }
        SetFilterName(UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name);
    }

    private void InstantiateFilterPrefabs()
    {
        string basePath = $"Arts/5AR/{filterName}/{filterName}";

        InstantiatePart($"{basePath}_LeftEye", out leftEyePrefab);
        InstantiatePart($"{basePath}_RightEye", out rightEyePrefab);
        InstantiatePart($"{basePath}_Nose", out nosePrefab);
        InstantiatePart($"{basePath}_Forehead", out foreheadPrefab);
    }

    private void InstantiatePart(string path, out GameObject part)
    {
        GameObject prefab = Resources.Load<GameObject>(path);
        if (prefab != null)
        {
            part = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            part.transform.SetParent(transform);
        }
        else
        {
            part = null;
            Debug.LogWarning($"프리팹을 찾을 수 없습니다: {path}");
        }
    }

    void SetPrefabVisibility(bool isVisible)
    {
        GameObject[] allPartPrefab = { leftEyePrefab, rightEyePrefab, nosePrefab, foreheadPrefab };

        foreach(var part in allPartPrefab)
        {
            if(part != null)
            {
                part.SetActive(isVisible);
            }
        }
    }

    private void ApplyFilter()
    {
        foreach (ARFace face in arFaceManager.trackables)
        {
            // ARFace의 렌더러를 비활성화 (페이스 마스크 숨기기)
            MeshRenderer faceRenderer = face.GetComponent<MeshRenderer>();
            if (faceRenderer != null && faceRenderer.enabled == true)
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
            Vector3 leftEyeLocal = face.vertices[leftEyeIndex];
            Vector3 rightEyeLocal = face.vertices[rightEyeIndex];
            Vector3 noseLocal = face.vertices[noseIndex];
            Vector3 foreheadLocal = face.vertices[foreheadIndex];

            // 위치 보정
            leftEyeLocal.x -= 0.02f;
            rightEyeLocal.x += 0.02f;

            Vector3 leftEyeWorldPos = face.transform.TransformPoint(leftEyeLocal);
            Vector3 rightEyeWorldPos = face.transform.TransformPoint(rightEyeLocal);
            Vector3 noseWorldPos = face.transform.TransformPoint(noseLocal);
            Vector3 foreheadWorldPos = face.transform.TransformPoint(foreheadLocal);

            Quaternion faceRotation = face.transform.rotation;

            // 얼굴이 바라보는 방향
            Vector3 faceForward = face.transform.forward;
            Vector3 faceUp = face.transform.up;

            // 카메라 회전 보정 (카메라가 회전해도 스프라이트가 이상하지 않도록)
            Quaternion inverseCameraRotation = Quaternion.Inverse(Camera.main.transform.rotation);
            Quaternion adjustedRotation = inverseCameraRotation * faceRotation;

            GameObject[] allPartPrefab = { leftEyePrefab, rightEyePrefab, nosePrefab, foreheadPrefab };
            Vector3[] allPartWorldPos = {leftEyeWorldPos, rightEyeWorldPos, noseWorldPos, foreheadWorldPos};

            int partIndex = 0;
            foreach (var part in allPartPrefab)
            {
                if (part != null)
                {
                    part.transform.position = allPartWorldPos[partIndex];
                    part.transform.rotation = adjustedRotation;
                }
                partIndex++;
            }

            SetPrefabVisibility(true);
        }
    }
}
