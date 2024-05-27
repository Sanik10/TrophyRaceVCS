using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Artngame.SKYMASTER
{
    public class volumeCloudsTornadoControl : MonoBehaviour
    {
        public bool copyCloudParamsToRefl = false; //copy main camera cloud props to reflect camera clouds

        public CloudScript volumeclouds; //connectSuntoFullVolumeCloudsURP volumeclouds;
        public CloudScriptTRANSP volumecloudsTRANSP;
        public CloudScript CloudReflection;

        public Material grassMat;
        public bool applyGrassVortex = false;

        public Material waterMat;
        public bool applyWaterVortex = false;
        public float vortexRadius = 115;
        public Vector4 vortexInteractAmpFreqRadial = new Vector4(-107, 11,39,318);

        public Transform tornadoTop;
        public Transform tornadoBottom;
        public float posMultiplierX = 1;
        public float posMultiplierZ = 1;

        public float posMultiplierXW = 1;
        public float posMultiplierZW = 1;

        public float posOffsetX = 0;
        public float posOffsetZ = 0;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (copyCloudParamsToRefl)
            {
                if (CloudReflection != null)
                {
                    CloudReflection.density = volumeclouds.density;
                    CloudReflection.coverage = volumeclouds.coverage;

                    CloudReflection.scale = volumeclouds.scale;
                    CloudReflection.detailScale = volumeclouds.detailScale;
                    CloudReflection.lowFreqMin = volumeclouds.lowFreqMin;
                    CloudReflection.lowFreqMax = volumeclouds.lowFreqMax;
                    CloudReflection.highFreqModifier = volumeclouds.highFreqModifier;
                    CloudReflection.weatherScale = volumeclouds.weatherScale;
                    CloudReflection.startHeight = volumeclouds.startHeight;
                    CloudReflection.thickness = volumeclouds.thickness;
                    CloudReflection.planetSize = volumeclouds.planetSize;
                    CloudReflection.cloudSampleMultiplier = volumeclouds.cloudSampleMultiplier;
                    CloudReflection.globalMultiplier = volumeclouds.globalMultiplier;
                    CloudReflection.windSpeed = volumeclouds.windSpeed;
                    CloudReflection.windDirection = volumeclouds.windDirection;
                    CloudReflection.coverageWindSpeed = volumeclouds.coverageWindSpeed;
                    CloudReflection.coverageWindDirection = volumeclouds.coverageWindDirection;
                }
            }


            if (volumeclouds != null && tornadoTop != null)
            {
                volumeclouds.vortexPosition = new Vector3(tornadoTop.position.x * posMultiplierX + posOffsetX, 0 , tornadoTop.position.z * posMultiplierZ + posOffsetZ);
            }
            if (volumecloudsTRANSP != null && tornadoTop != null)
            {
                volumecloudsTRANSP.vortexPosition = new Vector3(tornadoTop.position.x * posMultiplierX + posOffsetX, 0, tornadoTop.position.z * posMultiplierZ + posOffsetZ);
            }
            if (CloudReflection != null && tornadoTop != null)
            {
                CloudReflection.vortexPosition = new Vector3(tornadoTop.position.x * posMultiplierX + posOffsetX, 0, tornadoTop.position.z * posMultiplierZ + posOffsetZ);
            }

            if(applyWaterVortex && waterMat != null)
            {
                waterMat.SetVector("vortexPosScale", new Vector4(
                    tornadoBottom.position.x * posMultiplierXW + posOffsetX, 
                    0,
                    tornadoBottom.position.z * posMultiplierZW + posOffsetZ, vortexRadius));
                waterMat.SetVector("_InteractAmpFreqRad", vortexInteractAmpFreqRadial);                
            }

        }
    }
}