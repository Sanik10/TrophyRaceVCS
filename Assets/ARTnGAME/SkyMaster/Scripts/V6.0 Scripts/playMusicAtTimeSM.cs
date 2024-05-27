using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Artngame.SKYMASTER
{
    public class playMusicAtTimeSM : MonoBehaviour
    {

        //v0.1
        public bool reduceOtherSounds = false;
        public List<AudioSource> audioSourcesToPause = new List<AudioSource>();
        public float pauseSpeed = 10; //
        public float lowThres = 0.01f;

        public AudioSource audioDawn;
        public AudioSource audioMidday;
        public AudioSource audioDusk;
        public AudioSource audioNight;

        public float volumeIncreaseSpeed = 0.01f;

        public float maxVolumeDawn = 0.65f;
        public float maxVolumeMidday = 0.65f;
        public float maxVolumeDusk = 0.65f;
        public float maxVolumeNight = 0.65f;

        public SkyMasterManager skymanager;

        public bool changeWeatherPerMusic = false;
        //public enum Weather_types {Sunny, Foggy, HeavyFog, Tornado, SnowStorm, 
        //FreezeStorm, FlatClouds, LightningStorm, HeavyStorm,HeavyStormDark, Cloudy, RollingFog, VolcanoErupt, Rain}
        public SkyMasterManager.Volume_Weather_types dawnWeather = SkyMasterManager.Volume_Weather_types.Sunny;
        public SkyMasterManager.Volume_Weather_types middayWeather = SkyMasterManager.Volume_Weather_types.Cloudy;
        public SkyMasterManager.Volume_Weather_types duskWeather = SkyMasterManager.Volume_Weather_types.LightningStorm;
        public SkyMasterManager.Volume_Weather_types nightWeather = SkyMasterManager.Volume_Weather_types.Rain;

        public float dawnTime = 9;
        public float middayTime = 14;
        public float duskTime = 20;
        public float nightTime = 23.5f;

        // Start is called before the first frame update
        void Start()
        {

        }
        public bool playOnce = false; //play once without loop in AudioSource and reset to start over

        // Update is called once per frame
        void LateUpdate()
        {
            if (skymanager.Current_Time > dawnTime && skymanager.Current_Time < middayTime)
            {
                if (audioDawn.volume < maxVolumeDawn)
                {
                    if (playOnce)
                    {
                        if (!audioDawn.isPlaying)
                        {
                            audioDawn.Play();
                        }
                    }
                    audioDawn.volume += volumeIncreaseSpeed * Time.deltaTime;

                    
                }

                //v0.1
                if (reduceOtherSounds && audioDawn.isPlaying)
                {
                    for (int i = 0; i < audioSourcesToPause.Count; i++)
                    {
                        audioSourcesToPause[i].volume -= pauseSpeed * Time.deltaTime;
                        //Debug.Log(audioSourcesToPause[i].volume + ":"+i);
                        if (audioSourcesToPause[i].volume <= lowThres)
                        {
                            audioSourcesToPause[i].volume = lowThres;
                        }
                    }
                }

                audioMidday.volume -= volumeIncreaseSpeed * Time.deltaTime;
                audioDusk.volume -= volumeIncreaseSpeed * Time.deltaTime;
                audioNight.volume -= volumeIncreaseSpeed * Time.deltaTime;

                if (playOnce)
                {
                    if(audioMidday.volume < 0.01f)
                    {
                        audioMidday.Stop();
                    }
                    if (audioDusk.volume < 0.01f)
                    {
                        audioDusk.Stop();
                    }
                    if (audioNight.volume < 0.01f)
                    {
                        audioNight.Stop();
                    }
                }

                if (changeWeatherPerMusic)
                {
                    skymanager.currentWeatherName = dawnWeather;
                }
            }
            if (skymanager.Current_Time >= middayTime && skymanager.Current_Time < duskTime)
            {
                audioDawn.volume -= volumeIncreaseSpeed * Time.deltaTime;
                if (audioMidday.volume < maxVolumeMidday)
                {
                    if (playOnce)
                    {
                        if (!audioMidday.isPlaying)
                        {
                            audioMidday.Play();
                        }
                    }
                    audioMidday.volume += volumeIncreaseSpeed * Time.deltaTime;

                   
                }

                //v0.1
                if (reduceOtherSounds && audioMidday.isPlaying)
                {
                    for (int i = 0; i < audioSourcesToPause.Count; i++)
                    {
                        audioSourcesToPause[i].volume -= pauseSpeed * Time.deltaTime;
                        //Debug.Log(audioSourcesToPause[i].volume + ":" + i);
                        if (audioSourcesToPause[i].volume <= lowThres)
                        {
                            audioSourcesToPause[i].volume = lowThres;
                        }
                    }
                }

                audioDusk.volume -= volumeIncreaseSpeed * Time.deltaTime;
                audioNight.volume -= volumeIncreaseSpeed * Time.deltaTime;

                if (playOnce)
                {
                    if (audioDawn.volume < 0.01f)
                    {
                        audioDawn.Stop();
                    }
                    if (audioDusk.volume < 0.01f)
                    {
                        audioDusk.Stop();
                    }
                    if (audioNight.volume < 0.01f)
                    {
                        audioNight.Stop();
                    }
                }

                if (changeWeatherPerMusic)
                {
                    skymanager.currentWeatherName = middayWeather;
                }
            }
            if (skymanager.Current_Time >= duskTime && skymanager.Current_Time < nightTime)
            {
                audioDawn.volume -= volumeIncreaseSpeed * Time.deltaTime;
                audioMidday.volume -= volumeIncreaseSpeed * Time.deltaTime;
                if (audioDusk.volume < maxVolumeDusk)
                {
                    if (playOnce)
                    {
                        if (!audioDusk.isPlaying)
                        {
                            audioDusk.Play();
                        }
                    }
                    audioDusk.volume += volumeIncreaseSpeed * Time.deltaTime;

                    
                }

                //v0.1
                if (reduceOtherSounds && audioDusk.isPlaying)
                {
                    for (int i = 0; i < audioSourcesToPause.Count; i++)
                    {
                        audioSourcesToPause[i].volume -= pauseSpeed * Time.deltaTime;
                        //Debug.Log(audioSourcesToPause[i].volume + ":" + i);
                        if (audioSourcesToPause[i].volume <= lowThres)
                        {
                            audioSourcesToPause[i].volume = lowThres;
                        }
                    }
                }


                audioNight.volume -= volumeIncreaseSpeed * Time.deltaTime;

                if (playOnce)
                {
                    if (audioDawn.volume < 0.01f)
                    {
                        audioDawn.Stop();
                    }
                    if (audioMidday.volume < 0.01f)
                    {
                        audioMidday.Stop();
                    }
                    if (audioNight.volume < 0.01f)
                    {
                        audioNight.Stop();
                    }
                }

                if (changeWeatherPerMusic)
                {
                    skymanager.currentWeatherName = duskWeather;
                }
            }
            if ((skymanager.Current_Time >= nightTime && skymanager.Current_Time <= 24) || (skymanager.Current_Time >= 0 && skymanager.Current_Time <= dawnTime))
            {
                audioDawn.volume -= volumeIncreaseSpeed * Time.deltaTime;
                audioMidday.volume -= volumeIncreaseSpeed * Time.deltaTime;
                audioDusk.volume -= volumeIncreaseSpeed * Time.deltaTime;
                if (audioNight.volume < maxVolumeNight)
                {
                    if (playOnce)
                    {
                        if (!audioNight.isPlaying)
                        {
                            audioNight.Play();
                        }
                    }
                    audioNight.volume += volumeIncreaseSpeed * Time.deltaTime;

                    
                }

                //v0.1
                if (reduceOtherSounds && audioNight.isPlaying)
                {
                    for (int i = 0; i < audioSourcesToPause.Count; i++)
                    {
                        audioSourcesToPause[i].volume -= pauseSpeed * Time.deltaTime;
                        //Debug.Log(audioSourcesToPause[i].volume + ":" + i);
                        if (audioSourcesToPause[i].volume <= lowThres)
                        {
                            audioSourcesToPause[i].volume = lowThres;
                        }
                    }
                }

                if (playOnce)
                {
                    if (audioDawn.volume < 0.01f)
                    {
                        audioDawn.Stop();
                    }
                    if (audioMidday.volume < 0.01f)
                    {
                        audioMidday.Stop();
                    }
                    if (audioDusk.volume < 0.01f)
                    {
                        audioDusk.Stop();
                    }
                }

                if (changeWeatherPerMusic)
                {
                    skymanager.currentWeatherName = nightWeather;
                }
            }
        }
    }
}