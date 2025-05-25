using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using static FaceRecognition;

public class ARFaceFilterApplier : MonoBehaviour
{
    public ARFaceManager arFaceManager;
    public Camera arCamera;
    private string filterName;

    public GameObject filterPanel;
    public GameObject bottomButtons;

    private Dictionary<string, int> faceLandmarkIndices = new Dictionary<string, int>
    {
        { "LeftCheek", 205 },
        { "RightCheek", 425 },
        { "Nose", 2 },
        { "Forehead", 10 }
    };

    private Dictionary<ARFace, Dictionary<string, GameObject>> faceFilters = new();

    void Update()
    {
        foreach(ARFace face in arFaceManager.trackables)
        {
            DisableFaceRenderer(face);
        }

        // 필터가 선택되지 않았다면
        if (string.IsNullOrEmpty(filterName))
        {
            return;
        }

        ApplyFilter();
    }

    private void OnEnable()
    {
        arFaceManager.facesChanged += OnFacesChanged;
    }

    private void OnDisable()
    {
        arFaceManager.facesChanged -= OnFacesChanged;
    }


    private void OnFacesChanged(ARFacesChangedEventArgs args)
    {
        foreach (var addedFace in args.added)
        {
            if (!string.IsNullOrEmpty(filterName))
            {
                InstantiateFaceFilter(addedFace);
            }
        }

        foreach (var removedFace in args.removed)
        {
            RemoveFaceFilter(removedFace);
        }
    }

    private void InstantiateFaceFilter(ARFace face)
    {
        if (faceFilters.ContainsKey(face))
            return;

        Dictionary<string, GameObject> parts = new();
        AddPart(face, parts, "LeftCheek");
        AddPart(face, parts, "RightCheek");
        AddPart(face, parts, "Forehead");

        if (filterName == "Ep2")
        {
            AddPart(face, parts, "Nose");
        }
        faceFilters[face] = parts;
    }

    private void RemoveFaceFilter(ARFace face)
    {
        if (faceFilters.TryGetValue(face, out var parts))
        {
            foreach (var part in parts.Values)
            {
                if (part != null)
                    Destroy(part);
            }
            faceFilters.Remove(face);
        }
    }

    public void SetFilterName(string name)
    {
        filterName = name;
    }

    public void OnClick_Filter()
    {
        string selectedFilter = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name;
        StartCoroutine(ChangeFilter(selectedFilter));
    }

    private IEnumerator ChangeFilter(string selectedFilter)
    {
        if (filterName == selectedFilter)
            yield break;

        RemoveFilter();
        yield return null;

        SetFilterName(selectedFilter);
        
        if(bottomButtons)
        {
            SetActiveBottomButtons(true);
        }

        foreach (var face in arFaceManager.trackables)
        {
            InstantiateFaceFilter(face);
        }
    }

    void AddPart(ARFace face, Dictionary<string, GameObject> parts, string partName)
    {
        string path = $"5AR/{filterName}_{partName}";
        GameObject prefab = Resources.Load<GameObject>(path);

        if (prefab != null)
        {
            GameObject part = Instantiate(prefab);
            part.transform.SetParent(transform);
            parts[partName] = part;
        }
        else
        {
            Debug.LogWarning($"프리팹을 찾을 수 없습니다: {path}");
        }
    }

    private void ApplyFilter()
    {
        foreach (var kvp in faceFilters)
        {
            ARFace face = kvp.Key;
            Dictionary<string, GameObject> parts = kvp.Value;

            if (face.trackingState != TrackingState.Tracking)
            {
                foreach (var part in parts.Values)
                {
                    part.SetActive(false);
                }
                continue;
            }

            // 카메라 회전 보정 (카메라가 회전해도 스프라이트가 이상하지 않도록)
            Quaternion faceRotation = face.transform.rotation;
            Quaternion inverseCameraRotation = Quaternion.Inverse(arCamera.transform.rotation);
            Quaternion adjustedRotation = inverseCameraRotation * faceRotation;

            foreach (var pair in parts)
            {
                string partName = pair.Key;
                GameObject part = pair.Value;

                if (faceLandmarkIndices.TryGetValue(partName, out int vertexIndex))
                {
                    Vector3 localPos = face.vertices[vertexIndex];
                    Vector3 worldPos = face.transform.TransformPoint(localPos);

                    part.transform.position = worldPos;
                    part.transform.rotation = adjustedRotation;
                }
            }
        }
    }

    void DisableFaceRenderer(ARFace face)
    {
        var meshRenderer = face.GetComponent<MeshRenderer>();
        if(meshRenderer != null && meshRenderer.enabled )
        {
            meshRenderer.enabled = false;
        }
    }

    public void OnClick_RemoveFilter()
    {
        filterName = null;
        if (bottomButtons)
        {
            SetActiveBottomButtons(true);
        }

        RemoveFilter();
    }

    void RemoveFilter()
    {
        var faces = new List<ARFace>(faceFilters.Keys);
        foreach (var face in faces)
        {
            RemoveFaceFilter(face);
        }
    }

    public void SetActiveBottomButtons(bool active)
    {
        filterPanel.SetActive(!active);
        bottomButtons.SetActive(active);
    }
}
