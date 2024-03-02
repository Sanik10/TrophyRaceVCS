using System;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class AnimatedCamerasController : MonoBehaviour {

    public static event Action AnimationDoneEvent;

    [SerializeField]
    private List<AnimatedCameraItem> animatedCameras = new List<AnimatedCameraItem>();
    private int currentCameraIndex = 0;
    private float lastSwitchedTime = 0;
    [SerializeField]
    private bool activateAnimations = true;
    [SerializeField]
    private bool stopAnimation = false;
    [SerializeField]
    private bool _shortAnimation = false;
    public bool shortAnimation {
        get {return this._shortAnimation;}
        set {if(this._shortAnimation != value) {
                this._shortAnimation = value;
            }
        }
    }

    private void Start() {
        FindVirtualCameras();
    }

    private void FindVirtualCameras() {
        foreach (var animatedCamera in animatedCameras) {
            CinemachineVirtualCamera vc = animatedCamera.DollyTrackGO.GetComponentInChildren<CinemachineVirtualCamera>();
            if (vc != null) {
                animatedCamera.virtualCamera = vc;
                animatedCamera.pathLength = animatedCamera.DollyTrackGO.GetComponent<CinemachineSmoothPath>()?.PathLength ?? 0;

                if (animatedCameras.IndexOf(animatedCamera) != currentCameraIndex) {
                    vc.gameObject.SetActive(false);
                }
            }
        }
    }

    private void Update() {
        if(animatedCameras[currentCameraIndex].virtualCamera.GetCinemachineComponent<CinemachineTrackedDolly>().m_PathPosition >= 1.0f && !activateAnimations) {
            SwitchToNextCamera();
        }

        if(activateAnimations) {
            ResetAnimations(); // Запуск анимаций сначала
        }

        TrackCamera();
    }

    private void SwitchToNextCamera() {
        // Отключаем текущую камеру
        animatedCameras[currentCameraIndex].virtualCamera.gameObject.SetActive(false);

        animatedCameras[currentCameraIndex].virtualCamera.GetCinemachineComponent<CinemachineTrackedDolly>().m_PathPosition = 0;

        if((stopAnimation && currentCameraIndex == animatedCameras.Count-1) || (this._shortAnimation && currentCameraIndex == 0)) {
            TurnOffAllCameras();
            AnimationDoneEvent?.Invoke();
            return;
        }

        // Переходим к следующей камере или к первой, если достигнут конец списка
        currentCameraIndex = (currentCameraIndex + 1) % animatedCameras.Count;

        // Включаем новую текущую камеру
        animatedCameras[currentCameraIndex].virtualCamera.gameObject.SetActive(true);

        ResetTime();
    }

    private void TurnOffAllCameras() {
        foreach (var animatedCamera in animatedCameras) {
            animatedCamera.virtualCamera.gameObject.SetActive(false);
        }
    }

    private void ResetAnimations() {
        currentCameraIndex = 0;
        animatedCameras[currentCameraIndex].virtualCamera.gameObject.SetActive(true);
        ResetTime();
        activateAnimations = false;
    }

    private void ResetTime() {
        lastSwitchedTime = Time.time;
    }

    private void TrackCamera() {
        float timePercentage = (Time.time - lastSwitchedTime) / animatedCameras[currentCameraIndex].routeTime;
        float clampedPercentage = Mathf.Clamp01(timePercentage);
        animatedCameras[currentCameraIndex].virtualCamera.GetCinemachineComponent<CinemachineTrackedDolly>().m_PathPosition = clampedPercentage;

        // Изменение фокусного расстояния
        float[] focalLength = animatedCameras[currentCameraIndex].focalLength;
        if (focalLength != null && focalLength.Length > 0) {
            int index = Mathf.FloorToInt(clampedPercentage * (focalLength.Length - 1));
            float t = clampedPercentage * (focalLength.Length - 1) - index;
            if (index < focalLength.Length - 1) {
                float interpolatedFocalLength = Mathf.Lerp(focalLength[index], focalLength[index + 1], t);
                SetFocalLength(animatedCameras[currentCameraIndex].virtualCamera, interpolatedFocalLength);
            }
        }
    }

    // Метод для установки фокусного расстояния камеры
    private void SetFocalLength(CinemachineVirtualCamera virtualCamera, float focalLength) {
        if (virtualCamera != null) {
            LensSettings lens = virtualCamera.m_Lens;
            lens.FieldOfView = CalculateFOV(focalLength);
            virtualCamera.m_Lens = lens;
        }
    }

    // Calculate the Field of View based on Focal Length
    private float CalculateFOV(float focalLength) {
        float sensorSize = animatedCameras[currentCameraIndex].virtualCamera.m_Lens.SensorSize.y;
        float fovRad = 2f * Mathf.Atan(sensorSize / (2f * focalLength));
        return fovRad * Mathf.Rad2Deg;
    }
}

[System.Serializable]
public class AnimatedCameraItem {
    public string cameraNameOrLocation;
    public GameObject DollyTrackGO;
    public float routeTime;
    public float pathLength;
    public CinemachineVirtualCamera virtualCamera;
    public float[] focalLength;
}