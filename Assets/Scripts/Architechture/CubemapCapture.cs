using UnityEngine;

public class CubemapCapture : MonoBehaviour {
    public Camera captureCamera; // Камера для захвата кубической карты
    public CubemapSize cubemapSize = CubemapSize._1024; // Размер кубической карты
    public string savePath = "Assets/Art/"; // Путь для сохранения кубической карты

    // Перечисление для размеров кубической карты
    public enum CubemapSize {
        _64 = 64,
        _128 = 128,
        _256 = 256,
        _512 = 512,
        _1024 = 1024,
        _2048 = 2048,
        _4096 = 4096
    }

    void Start() {
        // Создаем кубическую карту
        Cubemap cubemap = new Cubemap((int)cubemapSize, TextureFormat.RGB24, false);
        // Захватываем кубическую карту с камеры
        captureCamera.RenderToCubemap(cubemap);

        // Сохраняем кубическую карту как asset
        #if UNITY_EDITOR
        string cubemapAssetPath = savePath + "CapturedDynamicCubemap.cubemap";
        UnityEditor.AssetDatabase.CreateAsset(cubemap, cubemapAssetPath);
        UnityEditor.AssetDatabase.SaveAssets();
        #endif

        Debug.Log("Cubemap captured and saved to: " + savePath + "CapturedDynamicCubemap.cubemap");
    }
}