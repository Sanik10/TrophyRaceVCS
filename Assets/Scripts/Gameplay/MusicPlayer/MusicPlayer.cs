using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

public class MusicPlayer : MonoBehaviour {

    public bool musicOn;
    public AudioMixerGroup audioMixer;
    // private InputHandler InputHandler;
    [Range(0, 1)]
    public float musicVolume;
    public musicGenre selectedGenre;
    public int currentTrack;
    public List<MusicTrackItem> musicList = new List<MusicTrackItem>();
    private AudioSource source;
    private bool setUped = false, pauseFlag = false;
    private float musicChangeRate;

    void Start() {
        if(musicOn) {
            currentTrack = Random.Range(0, musicList.Count-1);
            while(musicList[currentTrack].genre != selectedGenre) {
                currentTrack++;
            }
            InitializeMusicSource();
        }
    }

    private void Update() {
        if(musicOn) {
            if(!setUped) {
                InitializeMusicSource();
            }

            if(Input.GetKey(KeyCode.J) && Time.time > musicChangeRate) {
                PreviusTrack();
            }
            if(Input.GetKey(KeyCode.K) && Time.time > musicChangeRate) {
                PauseTrack();
            }
            if(Input.GetKey(KeyCode.L) && Time.time > musicChangeRate) {
                NextTrack();
            }
        }
    }

    IEnumerator WaitMusicEnd() {
        Debug.Log("Now playing: " + musicList[currentTrack].name);
        if(musicOn) {
            while(source.isPlaying) {
                yield return null;
            }
            NextTrack();
        }
    }

    private void DeleteMusic() {
        Destroy(source);
        StopCoroutine("WaitMusicEnd");
        setUped = false;
    }

    private void PreviusTrack() {
        source.Stop();
        currentTrack = (currentTrack - 1 + musicList.Count) % musicList.Count;
        // Если currentTrack равен 0, то устанавливаем его в конец списка, иначе уменьшаем на 1
        if (selectedGenre != musicGenre.Mix) {
            while (musicList[currentTrack].genre != selectedGenre) {
                currentTrack = (currentTrack - 1 + musicList.Count) % musicList.Count;
            }
        }
        SetMusicClipToSource();
    }

    private void PauseTrack() {
        if(!pauseFlag) {
            StopCoroutine("WaitMusicEnd");
            source.Pause();
            pauseFlag = true;
        } else if(pauseFlag) {
            source.Play();
            StartCoroutine("WaitMusicEnd");
            pauseFlag = false;
        }
        musicChangeRate = Time.time + 0.2f;
    }

    private void NextTrack() {
        source.Stop();
        currentTrack = (currentTrack + 1) % musicList.Count;
        // Увеличиваем currentTrack на 1, если он превышает количество треков, он зацикливается
        while (musicList[currentTrack].genre != selectedGenre) {
            currentTrack = (currentTrack + 1) % musicList.Count;
        }
        SetMusicClipToSource();
    }

    private void SetMusicClipToSource() {
        source.clip = musicList[currentTrack].musicClip;
        source.volume = musicVolume * musicList[currentTrack].musicClipMastering;
        source.time = musicList[currentTrack].startTime;
        source.Play();
        StartCoroutine("WaitMusicEnd");
        musicChangeRate = Time.time + 0.2f;
    }
    
    private void InitializeMusicSource() {
        source = gameObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.reverbZoneMix = 0;
        source.spatialBlend = 0;
        source.loop = false;
        source.pitch = 1;
        if(audioMixer != null) {
            source.outputAudioMixerGroup = audioMixer;
        }
        SetMusicClipToSource();
        source.time = musicList[currentTrack].preRaceStartTime;
        setUped = true;
    }
}

[System.Serializable]
public class MusicTrackItem {
    public string name;
    public AudioClip musicClip;
    [Range(0f, 1f)]
    public float musicClipMastering = 1f;
    public float preRaceStartTime = 0.0f;
    public float startTime = 0.0f;
    public musicGenre genre;
}

[System.Serializable]
public enum musicGenre{
    Phonk, HipHop, Electro, Rock, Mix
}