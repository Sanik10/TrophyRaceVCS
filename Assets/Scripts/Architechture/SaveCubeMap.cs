using UnityEngine;

public class SaveCubeMap : MonoBehaviour
{
    public RenderTexture cubeMap;
    public string savePath = "Assets/CubeMapTexture.cubemap";
    public bool renderCubeMap = false;

    private void Update()
    {
        if (renderCubeMap)
        {
            RenderCubeMap();
            renderCubeMap = false; // Сбрасываем переменную после рендеринга
        }
    }

    private void RenderCubeMap()
    {
        Texture2D texture = new Texture2D(cubeMap.width, cubeMap.height, TextureFormat.RGBA32, false);
        RenderTexture.active = cubeMap;
        texture.ReadPixels(new Rect(0, 0, cubeMap.width, cubeMap.height), 0, 0);
        texture.Apply();

        byte[] bytes = texture.EncodeToPNG();
        System.IO.File.WriteAllBytes(savePath, bytes);

        RenderTexture.active = null;
        Debug.Log("CubeMap saved to: " + savePath);
    }
}