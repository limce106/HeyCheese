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
    private string filterName;

    public GameObject filterPanel;
    public GameObject bottomButtons;

    private Dictionary<string, int> faceLandmarkIndices = new Dictionary<string, int>
    {
        { "LeftCheek", 436 },
        { "RightCheek", 216 },
        { "Nose", 2 },
        { "Forehead", 10 }
    };

    private Dictionary<string, GameObject> filterParts = new Dictionary<string, GameObject>();

    void Update()
    {
        // 필터가 선택되지 않았다면
        if (string.IsNullOrEmpty(filterName))
        {
            return;
        }
        // 감지된 얼굴이 없으면
        if (arFaceManager.trackables.count == 0)
        {
            SetPrefabVisibility(false);
            return;
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
        filterPanel.SetActive(false);
        bottomButtons.SetActive(true);
    }

    private void InstantiateFilterPrefabs()
    {

        InstantiatePart("LeftCheek");
        InstantiatePart("RightCheek");
        InstantiatePart("Forehead");

        if(filterName == "Ep2")
        {
            InstantiatePart("Nose");
        }
    }

    private void InstantiatePart(string partName)
    {
        string path = $"5AR/{filterName}_{partName}";
        GameObject prefab = Resources.Load<GameObject>(path);

        if (prefab != null)
        {
            GameObject part = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            part.transform.SetParent(transform);
            filterParts[partName] = part;
        }
        else
        {
            Debug.LogWarning($"프리팹을 찾을 수 없습니다: {path}");
        }
    }

    void SetPrefabVisibility(bool isVisible)
    {
        foreach(var part in filterParts.Values)
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

            // 카메라 회전 보정 (카메라가 회전해도 스프라이트가 이상하지 않도록)
            Quaternion faceRotation = face.transform.rotation;
            Quaternion inverseCameraRotation = Quaternion.Inverse(Camera.main.transform.rotation);
            Quaternion adjustedRotation = inverseCameraRotation * faceRotation;

            foreach(var part in filterParts)
            {
                string partName = part.Key;
                GameObject partPrefab = part.Value;

                if(faceLandmarkIndices.TryGetValue(partName, out int vertexIndex) && partPrefab != null)
                {
                    Vector3 localPos = face.vertices[vertexIndex];
                    Vector3 worldPos = face.transform.TransformPoint(localPos);

                    partPrefab.transform.position = worldPos;
                    partPrefab.transform.rotation = adjustedRotation;
                }
            }

            SetPrefabVisibility(true);
        }
    }

    public void RemoveFilter()
    {
        filterName = null;
        SetPrefabVisibility(false);
        filterPanel.SetActive(false);
        bottomButtons.SetActive(true);
    }
}
