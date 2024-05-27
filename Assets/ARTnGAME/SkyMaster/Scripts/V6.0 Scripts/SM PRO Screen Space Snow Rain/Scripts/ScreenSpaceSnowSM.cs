using UnityEngine;
using System.Collections;
namespace Artngame.SKYMASTER
{
    [ExecuteInEditMode]
    public class ScreenSpaceSnowSM : MonoBehaviour
    {
        public Texture2D SnowTexture;

        //v0.1b
        public bool performanceMode = false;
        public float downScale = 1;
        public bool recreateTexture = false;

        //v0.1a
        public float shadowPower = 0.1f;
        public float shadowPowerA = 1f;
        public float screenRainPower = 1;
        public float screenBrightness = 1;
        public float objectRainPower = 0;

        public Color SnowColor = Color.white;

        public float SnowTextureScale = 0.1f;

        [Range(0, 1)]
        public float BottomThreshold = 0f;
        [Range(0, 1)]
        public float TopThreshold = 1f;

        private Material _material;

        //v0.1
        public Texture2D SnowBumpTex;
        public Vector2 SnowBumpPowerScale = new Vector2(1, 1);
        public float Shineness = 1;
        public float specularPower = 1;

        //OUTLINE
        public float OutlineThickness = 0;
        public float DepthSensitivity = 0;
        public float NormalsSensitivity = 0;
        public float ColorSensitivity = 0;
        public Vector4 OutlineColor = new Vector4(0, 0, 0, 1);
        public Vector4 OutlineControls = new Vector4(1, 1, 1, 1);

        //RAIN
        public Vector4 interactPointRadius = new Vector4(0, 0, 0, 0);
        public Vector4 radialControls = new Vector4(0, 0, 0, 0);
        public Vector4 directionControls = new Vector4(0, 0, 0, 0);
        public Vector4 wipeControls = new Vector4(0, 0, 0, 0);
        //MASKED
        public Vector4 mainTexTilingOffset = new Vector4(1, 1, 0, 0);
        public float maskPower = 0;
        public float _Size = 1;
        public float _Distortion = 0;
        public float _Blur = 0;
        public Vector4 _TimeOffset = new Vector4(0, 0, 0, 0);
        public Vector4 _EraseCenterRadius = new Vector4(0, 0, 0, 0);
        public float erasePower = 0;
        public float _TileNumCausticRotMin = 0;
        public Vector4 _RainSmallDirection = new Vector4(0, 0, 0, 0);


        //RIPPLES
        public Texture2D RainRipples;
        public float RainIntensity = 0;
        public float RippleAnimSpeed = 0.2f;
        public float RippleTiling = 1;
        public float WaterBumpDistance = 1000;


        void OnEnable()
        {
            _material = new Material(Shader.Find("SkyMaster/ScreenSpaceSnowSM"));
            GetComponent<Camera>().depthTextureMode |= DepthTextureMode.DepthNormals;
        }

        RenderTexture middleA;
        void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            //v0.1a
            _material.SetFloat("shadowPower", shadowPower);
            _material.SetFloat("shadowPowerA", shadowPowerA);
            _material.SetFloat("screenRainPower", screenRainPower);
            _material.SetFloat("screenBrightness", screenBrightness);
            _material.SetFloat("objectRainPower", objectRainPower);

            // set 
            _material.SetMatrix("_CamToWorld", GetComponent<Camera>().cameraToWorldMatrix);
            _material.SetColor("_SnowColor", SnowColor);
            _material.SetFloat("_BottomThreshold", BottomThreshold);
            _material.SetFloat("_TopThreshold", TopThreshold);
            _material.SetTexture("_SnowTex", SnowTexture);
            _material.SetFloat("_SnowTexScale", SnowTextureScale);// * Camera.main.Far);

            //v0.1
            _material.SetTexture("_SnowBumpTex", SnowBumpTex);
            _material.SetFloat("Shineness", Shineness);
            _material.SetVector("SnowBumpPowerScale", SnowBumpPowerScale);
            _material.SetFloat("specularPower", specularPower);

            //OUTLINE
            _material.SetFloat("OutlineThickness", OutlineThickness);
            _material.SetFloat("DepthSensitivity", DepthSensitivity);
            _material.SetFloat("NormalsSensitivity", NormalsSensitivity);
            _material.SetFloat("ColorSensitivity", ColorSensitivity);
            _material.SetVector("OutlineColor", OutlineColor);
            _material.SetVector("OutlineControls", OutlineControls);
            _material.SetVector("_MainTex_TexelSize", new Vector4(1.0f / src.width, 1.0f / src.height, src.width, src.height));


            //RAIN
            _material.SetVector("interactPointRadius", interactPointRadius);
            _material.SetVector("radialControls", radialControls);
            _material.SetVector("directionControls", directionControls);
            _material.SetVector("wipeControls", wipeControls);
            //MASKED
            _material.SetVector("mainTexTilingOffset", mainTexTilingOffset);
            _material.SetFloat("maskPower", maskPower);
            _material.SetFloat("_Size", _Size);
            _material.SetFloat("_Distortion", _Distortion);
            _material.SetFloat("_Blur", _Blur);
            //v0.6
            _material.SetVector("_TimeOffset", _TimeOffset);
            _material.SetVector("_EraseCenterRadius", _EraseCenterRadius);
            _material.SetFloat("erasePower", erasePower);
            //v0.3
            _material.SetFloat("_TileNumCausticRotMin", _TileNumCausticRotMin);
            _material.SetVector("_RainSmallDirection", _RainSmallDirection);


            //RIPPLES
            _material.SetTexture("_Lux_RainRipples", RainRipples);
            _material.SetFloat("_Lux_RainIntensity", RainIntensity);
            _material.SetFloat("_Lux_RippleAnimSpeed", RippleAnimSpeed);
            _material.SetFloat("_Lux_RippleTiling", RippleTiling);
            _material.SetFloat("_Lux_WaterBumpDistance", WaterBumpDistance);


            // execute the shader on input texture (src) and write to output (dest)
            if (middleA == null || recreateTexture)
            {
                recreateTexture = false;
                middleA = new RenderTexture((int)(src.width/ downScale), (int)(src.height/ downScale), 24, RenderTextureFormat.ARGB32);// ARGBFloat);
            }
            //Graphics.Blit(src, dest, _material);
            _material.SetFloat("doOutline", 0);
            Graphics.Blit(src, middleA, _material);
            _material.SetFloat("doOutline", 1);
            if (performanceMode)
            {
                Graphics.Blit(middleA, dest);
            }
            else
            {
                Graphics.Blit(middleA, dest, _material);
            }
        }
    }
}