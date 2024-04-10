using UnityEngine;

public class RenderTextureToImage : MonoBehaviour
{
    public RenderTexture renderTexture;
    public string savePath = "Assets/Scenes/Scene_name/Cubemaps/OutputImage.png";
    public bool convert = false;

    public void Update() {
        if(convert) {
            ConvertRenderTextureToImage();
            convert = false;
        }
    }

    public void ConvertRenderTextureToImage()
    {
        convert = false;
        // Создание новой текстуры для хранения данных из рендер текстуры
        Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);

        // Чтение пикселей из рендер текстуры
        RenderTexture.active = renderTexture;
        texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture.Apply();

        // Преобразование текстуры в PNG и сохранение в файл
        byte[] bytes = texture.EncodeToPNG();
        System.IO.File.WriteAllBytes(savePath, bytes);

        // Очистка
        RenderTexture.active = null;
        Destroy(texture);

        Debug.Log("RenderTexture converted to image and saved at: " + savePath);
    }
}