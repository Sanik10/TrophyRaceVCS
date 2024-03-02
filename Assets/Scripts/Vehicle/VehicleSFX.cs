using System;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

public class VehicleSFX : MonoBehaviour {
    // when using four channel engine crossfading, the four clips should be:
    // For proper crossfading, the clips pitches should all match, with an octave offset between low and high.

    private bool soundsOn;
    [Header("Отстрелы выхлопа")]
    public AudioClip burbleClip;
    public AudioClip backFireClip1;
    public AudioClip backFireClip2;
    [Header("Турбонаддув")]
    // public bool turboSound;
    // [HideInInspector]public AudioClip turbo, turbo1;
    [HideInInspector]public AudioClip changeGearClipTirbo1, changeGearClipTirbo2, changeGearClipTirbo3;
    [Header("Трансмиссия")]
    public AudioClip changeGearUpClip;
    public AudioClip changeGearDownClip;
    public AudioClip gearboxWhine;
    [Header("Звуки шин")]
    public AudioClip tireRollSand;
    [Header("Мастеринг")]
    // [Range(0, 2)]public float turboVolume = 0.25f;
    // [Range(0, 2)]public float changeGearTurboVolume = 1;
    public AudioMixerGroup audioMixer;
    public AudioMixerGroup tiresAudioMixer;
    [Range(0, 1)]public float globalFxVolume = 0.5f;
    [Range(0, 1)]public float geaboxWhineVolume = 1f;
    [Range(0, 1)]public float changeGearVolume = 0.25f;
    [Range(0, 2)]public float backFireVolume = 1;
    [Range(0, 2)]public float burbleVolume = 1;
    private float m_Burblevolume; //turboWaitTime = 0.75f
    public float maxVolWineAt = 150;
    public float maxVolSandRollAt = 30;
    public float pitchMultiplier = 0.75f;                                          // Used for altering the pitch of audio clips
    public float lowPitchMin = 1;                                           // The lowest possible pitch for the low sounds
    public float lowPitchMax = 1.5f;                                            // The highest possible pitch for the low sounds
    public float highPitchMultiplier = 1;                                   // Used for altering the pitch of high sounds
    public float maxEngineRolloffDistance = 120;                                // The maximum distance where rollof starts to take place
    public float maxRolloffDistance = 120;
    public float maxFXRolloffDistance = 50;                                             // The mount of doppler effect used in the audio
    // public bool turboShot = false;                                             // Toggle for using doppler

    private AudioSource m_Turbo, m_Turbo1;
    private AudioSource m_changeGearClipTirbo1, m_changeGearClipTirbo2, m_changeGearClipTirbo3;
    private AudioSource m_ChangeGearUp;  // Source for the change gears
    private AudioSource m_ChangeGearDown;
    private AudioSource m_BackFire1;
    private AudioSource m_BackFire2;
    private AudioSource m_gearboxWhine;
    private AudioSource m_Burble;
    private AudioSource m_tireRollSand;
    private bool m_StartedSound;       // flag for knowing if we have started sounds // trbFlag = false
    private VehicleManager VehicleManager;
    private Engine Engine;
    private PhysicsCalculation PC;
    private GameObject inputsManager;
    
    private void StartSound() {
        VehicleManager = GetComponent<VehicleManager>();
        Engine = VehicleManager.Engine;
        PC = VehicleManager.PhysicsCalculation;

        m_gearboxWhine = SetUpEngineAudioSource(gearboxWhine);
        // m_burnout = SetUpEngineAudioSource()
        m_tireRollSand = SetUpTireAudioSource(tireRollSand);
        // m_tireRollSand
        m_ChangeGearUp = SetUpFXAudioSource(changeGearUpClip);
        m_ChangeGearDown = SetUpFXAudioSource(changeGearDownClip);
        m_BackFire1 = SetUpFXAudioSource(backFireClip1);
        m_BackFire2 = SetUpFXAudioSource(backFireClip2);

        // if (turboSound == true) {
        //     setUpTurboSounds();
        // }
        m_Burble = SetUpFXAudioSource(burbleClip);
        applySoundsVolume();

        m_Burble.Play();

        // flag that we have started the sounds playing
        m_StartedSound = true;
    }


    private void StopSound() {
        //Destroy all audio sources on this object:
        foreach (var source in GetComponents<AudioSource>()) {
            Destroy(source);
        }
        m_StartedSound = false;
    }

    // Update is called once per frame
    private void FixedUpdate() {
        // get the distance to main camera
        float camDist = (Camera.main.transform.position - transform.position).sqrMagnitude;
        soundsOn = (camDist < maxRolloffDistance*maxRolloffDistance) ? true : false;
        // stop sound if the object is beyond the maximum roll off distance
        if (m_StartedSound && !soundsOn) {
            StopSound();
        }

        // start the sound if not playing and it is nearer than the maximum distance
        if (!m_StartedSound && soundsOn) {
            StartSound();
        }

        if (m_StartedSound) {
            // The pitch is interpolated between the min and max values, according to the car's revs.
            // float pitch = ULerp(lowPitchMin, lowPitchMax, Engine.Rpm / Engine.MaxRpm);
            // // clamp to minimum pitch (note, not clamped to max for high revs while burning out)
            // pitch = Mathf.Min(lowPitchMax, pitch);
            // if(turboSound == true) {
            //     m_Turbo.pitch = 0.75f + (Engine.Rpm / Engine.MaxRpm) / 2;
            //     m_Turbo1.pitch = 0.75f + (Engine.Rpm / Engine.MaxRpm) / 2;
            // }

            m_gearboxWhine.pitch = PC.kph / maxVolWineAt;
            // float accFade = 0;

            // // get values for fading the sounds based on the acceleration
            // accFade = Mathf.Abs((input.vertical > 0) ? input.vertical : 0);  // !Engine.test

            // float decFade = 1 - accFade;

            // get the high fade value based on the cars revs
            // float highFade = Mathf.InverseLerp(0.2f, 0.8f,  Engine.Rpm / 10000);
            // float medFade = highFade / 2;
            // float lowFade = 1 - highFade;

            // // adjust the values to be more realistic
            // highFade = 1 - ((1 - highFade)*(1 - highFade));
            // // lowFade = 1 - ((1 - lowFade)*(1 - lowFade));
            // accFade = 1 - ((1 - accFade)*(1 - accFade));
            // decFade = 1 - ((1 - decFade)*(1 - decFade));

            // adjust the source volumes based on the fade values
            // if(turboSound == true) {
            //     m_Turbo.volume = highFade * (accFade / 1) * turboVolume;
            //     m_Turbo1.volume = highFade * (accFade / 1) * turboVolume;
            //     if(Engine.throttle >= 0.5f) {
            //         trbFlag = true;
            //     }
            //     if(Engine.throttle == 0 && Time.time > turboWaitTime && Engine.Rpm > 5000 && trbFlag) {
            //         turboShot = true;
            //         trbFlag = false;
            //         turboWaitTime = Time.time + 1;
            //     }
            //     if(turboShot) {
            //         changeGearTurbo();
            //         turboShot = false;
            //     }
            // }

            m_gearboxWhine.volume = (PC.kph / maxVolWineAt)*geaboxWhineVolume;
            m_Burble.volume = (Engine.rpm > 5000 && VehicleManager.VehicleInputHandler.vertical <= 0) ? burbleVolume : 0;

            m_tireRollSand.volume = ((PC.kph / maxVolSandRollAt > 1) ? 1 : (PC.kph / maxVolSandRollAt));
        }
    }


    // sets up and adds new audio source to the gane object
    private AudioSource SetUpEngineAudioSource(AudioClip clip) {
        // create the new audio source component on the game object and set up its properties
        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = 0;
        source.spatialBlend = 1;
        source.loop = true;

        // start the clip from a random point
        source.time = Random.Range(0f, clip.length);
        source.Play();
        source.minDistance = 5;
        source.maxDistance = maxFXRolloffDistance;
        source.dopplerLevel = 0;
        if (audioMixer != null) {
            source.outputAudioMixerGroup = audioMixer;
        }
        return source;
    }

    private AudioSource SetUpTireAudioSource(AudioClip clip) {
        // create the new audio source component on the game object and set up its properties
        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = 0;
        source.spatialBlend = 1;
        source.loop = true;

        // start the clip from a random point
        source.time = Random.Range(0f, clip.length);
        source.pitch = 1;
        source.Play();
        source.minDistance = 5;
        source.maxDistance = maxFXRolloffDistance;
        source.dopplerLevel = 0;
        if (tiresAudioMixer != null) {
            source.outputAudioMixerGroup = tiresAudioMixer;
        }
        return source;
    }

    private AudioSource SetUpFXAudioSource(AudioClip clip) {
        // create the new audio source component on the game object and set up its properties
        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = 0;
        source.spatialBlend = 1;
        source.loop = false;
        source.minDistance = 5;
        source.maxDistance = maxFXRolloffDistance;
        source.dopplerLevel = 0;
        if (audioMixer != null) {
            source.outputAudioMixerGroup = audioMixer;
        }
        return source;
    }

    public void gearUpAudio() {
        if(soundsOn){
            m_ChangeGearUp.Play();
        }
    }

    public void gearDownAudio() {
        if(soundsOn){
            m_ChangeGearDown.Play();
        }
    }

    public void backFireAudio() {
        if(soundsOn){
            int s;
            s = Random.Range(1, 3);
            if(s == 1){
                m_BackFire1.Play();
            } else if(s == 2){
                m_BackFire2.Play();
            }
        }
    }

    public void changeGearTurbo() {
        if(soundsOn){
            int s;
            s = Random.Range(1, 4);
            if(s == 1){
                m_changeGearClipTirbo1.Play();
            } else if(s == 2){
                m_changeGearClipTirbo2.Play();
            } else if(s == 3){
                m_changeGearClipTirbo3.Play();
            }
        }
    }

    private void applySoundsVolume() {
        m_ChangeGearUp.volume = changeGearVolume;
        m_ChangeGearDown.volume = changeGearVolume;
        m_BackFire1.volume = backFireVolume;
        m_BackFire2.volume = backFireVolume;
        // if(turboSound == true) {
        //     m_changeGearClipTirbo1.volume = changeGearTurboVolume;
        //     m_changeGearClipTirbo2.volume = changeGearTurboVolume;
        //     m_changeGearClipTirbo3.volume = changeGearTurboVolume;
        // }
        m_Burble.loop = true;
    }

    // private void setUpTurboSounds() {
    //     m_Turbo = SetUpEngineAudioSource(turbo);
    //     m_Turbo1 = SetUpEngineAudioSource(turbo1);

    //     m_changeGearClipTirbo1 = SetUpFXAudioSource(changeGearClipTirbo1);
    //     m_changeGearClipTirbo2 = SetUpFXAudioSource(changeGearClipTirbo2);
    //     m_changeGearClipTirbo3 = SetUpFXAudioSource(changeGearClipTirbo3);
    // }

    // unclamped versions of Lerp and Inverse Lerp, to allow value to exceed the from-to range
    private static float ULerp(float from, float to, float value) {
        return (1.0f - value)*from + value*to;
    }
}