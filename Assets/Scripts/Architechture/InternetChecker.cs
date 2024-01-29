using UnityEngine;

public static class InternetChecker {

    private static bool _isConnected = false;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize() {
        UpdateInternetConnectionStatus();
        if(!_isConnected) {
            Debug.LogError("No internet connection available.");
        }
    }

    public static void UpdateInternetConnectionStatus() {
        _isConnected = CheckInternetConnection();
    }

    public static bool CheckInternetConnection() {
        return Application.internetReachability != NetworkReachability.NotReachable;
    }

    public static bool IsConnected {
        get { return _isConnected; }
    }
}



/* IEnumerator, time-refresh system. обновляется через определенные промежутки времени
using System.Collections;
using UnityEngine;

public static class InternetChecker {

    private static float checkInterval = 1; // Интервал проверки в секундах

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize() {
        MonoBehaviour singleton = new GameObject(nameof(InternetChecker)).AddComponent<InternetCheckerBehaviour>();
        GameObject.DontDestroyOnLoad(singleton);
    }

    private class InternetCheckerBehaviour : MonoBehaviour {
        private void Start() {
            StartCoroutine(CheckInternetPeriodically());
        }

        private IEnumerator CheckInternetPeriodically() {
            while(true) {
                CheckInternetConnection();
                yield return new WaitForSeconds(checkInterval);
            }
        }

        private void CheckInternetConnection() {
            if (Application.internetReachability == NetworkReachability.NotReachable) {
                Debug.LogError("No internet connection available.");
            } else {
                Debug.Log("Internet connection is available.");
            }
        }
    }
}
*/