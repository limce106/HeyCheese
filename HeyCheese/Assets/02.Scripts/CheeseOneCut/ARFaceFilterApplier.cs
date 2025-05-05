using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARFaceFilterApplier : MonoBehaviour
{
    [SerializeField]
    private ARFaceManager arFaceManager;

    private GameObject leftCheekPrefab;
    private GameObject rightCheekPrefab;
    private GameObject nosePrefab;
    private GameObject foreheadPrefab;

    private string filterName;

    const int leftCheekIndex = 436;
    const int rightCheekIndex = 216;
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
            if(leftCheekPrefab.activeSelf || rightCheekPrefab.activeSelf || nosePrefab.activeSelf || foreheadPrefab.activeSelf)
            {
                SetPrefabVisibility(false);
                return;
            }
        }
        else if(!leftCheekPrefab.activeSelf || !rightCheekPrefab.activeSelf || !nosePrefab.activeSelf || !foreheadPrefab.activeSelf)
        {
            SetPrefabVisibility(true);
        }

        ApplyFilter();
    }

    public void SetFilterName(string name)
    {
        filterName = name;
    }

    public void OnClick_Filter()
    {
        SetFilterName(UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name);

        if (arFaceManager.trackables.count == 0)
        {
            SetPrefabVisibility(false);
        }

        InstantiateFilterPrefabs();
    }

    private void InstantiateFilterPrefabs()
    {
        string basePath = $"Arts/5AR/{filterName}/{filterName}";

        InstantiatePart($"{basePath}_LeftEye", out leftCheekPrefab);
        InstantiatePart($"{basePath}_RightEye", out rightCheekPrefab);
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
        GameObject[] allPartPrefab = { leftCheekPrefab, rightCheekPrefab, nosePrefab, foreheadPrefab };

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
            Vector3 leftEyeLocal = face.vertices[leftCheekIndex];
            Vector3 rightEyeLocal = face.vertices[rightCheekIndex];
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

            GameObject[] allPartPrefab = { leftCheekPrefab, rightCheekPrefab, nosePrefab, foreheadPrefab };
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

    public void RemoveFilter()
    {
        filterName = null;
        SetPrefabVisibility(false);
    }
}
