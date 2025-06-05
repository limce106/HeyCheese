using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance { get; private set; }

    [SerializeField] private GameObject networkPanel; // Inspector에서 할당

    public bool IsWifiConnected => 
        Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // WiFi 연결 패널
    public void ShowNetworkPanel() => networkPanel?.SetActive(true);
    public void HideNetworkPanel() => networkPanel?.SetActive(false);

    // WiFi 연결 확인
    public void CheckNetworkAndRun(System.Action onConnected)
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        Debug.Log("PC 환경이므로 네트워크 체크 생략하고 바로 실행");
        onConnected?.Invoke();  // 에디터나 PC에선 바로 실행
#else
        StartCoroutine(WaitForNetworkCoroutine(onConnected));
#endif
    }

    private IEnumerator WaitForNetworkCoroutine(System.Action onConnected)
    {
        while (!IsWifiConnected)
        {
            Debug.LogWarning("No Wifi. 패널 표시.");
            ShowNetworkPanel();
            yield return new WaitForSeconds(3);
        }

        HideNetworkPanel();
        Debug.Log("Wifi Connected");
        onConnected?.Invoke();
    }
}
