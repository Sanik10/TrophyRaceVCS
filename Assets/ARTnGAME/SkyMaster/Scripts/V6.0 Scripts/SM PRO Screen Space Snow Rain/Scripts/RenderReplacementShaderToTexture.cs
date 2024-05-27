using UnityEngine;
namespace Artngame.SKYMASTER
{
    [ExecuteInEditMode]
    public class RenderReplacementShaderToTexture : MonoBehaviour
    {
        [SerializeField]
        Shader replacementShader;

        [SerializeField]
        RenderTextureFormat renderTextureFormat = RenderTextureFormat.ARGBFloat;// RenderTextureFormat.ARGB32;

        [SerializeField]
        FilterMode filterMode = FilterMode.Bilinear;// FilterMode.Point;

        [SerializeField]
        int renderTextureDepth = 24;

        [SerializeField]
        CameraClearFlags cameraClearFlags = CameraClearFlags.Color;

        [SerializeField]
        Color background = Color.black;

        [SerializeField]
        string targetTexture = "_CameraDepthNormalsTextureA";// "_RenderTexture";

        private RenderTexture renderTexture;
        //private new Camera camera;
        public Camera cameraA;

        //v0.1
        public LayerMask maskLayers;
        //public Camera createdCamera;
        public bool createEditorCamera = false;

        private void Start()
        {
            if (Application.isPlaying)
            {
                if (cameraA != null)
                {
                    Destroy(cameraA.gameObject);
                }
                createCamera();
            }
        }

        private void Update()
        {
            if (!Application.isPlaying)
            {

                if (renderTexture == null)
                {
                    Camera thisCamera = GetComponent<Camera>();
                    renderTexture = new RenderTexture(thisCamera.pixelWidth, thisCamera.pixelHeight, renderTextureDepth, renderTextureFormat);
                    renderTexture.filterMode = filterMode;
                }

                Shader.SetGlobalTexture(targetTexture, renderTexture);
                if (cameraA != null)
                {
                    cameraA.targetTexture = renderTexture;
                    cameraA.SetReplacementShader(replacementShader, "RenderType");

                    cameraA.clearFlags = cameraClearFlags;
                    cameraA.backgroundColor = background;

                    cameraA.cullingMask = maskLayers;
                    cameraA.renderingPath = RenderingPath.Forward;
                }
            }
            if (createEditorCamera && !Application.isPlaying)
            {
                if (cameraA != null)
                {
                    DestroyImmediate(cameraA.gameObject);
                }
                createCamera();
                createEditorCamera = false;
            }
        }

        private void createCamera()
        {
            //foreach (Transform t in transform)
            //{
            //    DestroyImmediate(t.gameObject);
            //}

            Camera thisCamera = GetComponent<Camera>();

            // Create a render texture matching the main camera's current dimensions.
            renderTexture = new RenderTexture(thisCamera.pixelWidth, thisCamera.pixelHeight, renderTextureDepth, renderTextureFormat);
            renderTexture.filterMode = filterMode;
            // Surface the render texture as a global variable, available to all shaders.
            Shader.SetGlobalTexture(targetTexture, renderTexture);

            // Setup a copy of the camera to render the scene using the normals shader.
            GameObject copy = new GameObject("Camera" + targetTexture);
            cameraA = copy.AddComponent<Camera>();
            cameraA.CopyFrom(thisCamera);
            cameraA.transform.SetParent(transform);
            cameraA.targetTexture = renderTexture;
            cameraA.SetReplacementShader(replacementShader, "RenderType");
            cameraA.depth = thisCamera.depth - 1;
            cameraA.clearFlags = cameraClearFlags;
            cameraA.backgroundColor = background;

            cameraA.cullingMask = maskLayers;
            cameraA.renderingPath = RenderingPath.Forward;
            //createdCamera = camera;
        }
    }
}