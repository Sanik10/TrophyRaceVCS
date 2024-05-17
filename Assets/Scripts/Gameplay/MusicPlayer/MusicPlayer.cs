/*
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

public class MusicPlayer : MonoBehaviour {

    public bool musicOn;
    [SerializeField]
    private AudioMixerGroup audioMixer;
    [SerializeField] [Range(0, 1)]
    public float musicVolume;
    [SerializeField]
    private MusicGenre _selectedGenre;
    [SerializeField]
    private int currentTrack;
    public List<MusicTrackItem> musicList = new List<MusicTrackItem>();
    private AudioSource source;
    private bool setUped = false, pauseFlag = false;
    private float musicChangeRate;
    [SerializeField]
    private bool _setPreviusMusicTrack = false;
    [SerializeField]
    private bool _pauseMusicTrack = false;
    [SerializeField]
    private bool _setNextMusicTrack = false;
    [SerializeField]
    private InputType control;
    [SerializeField]
    private bool _preRaceMode = false;

    public bool setPreviusMusicTrack {
        get {return this._setPreviusMusicTrack;}
        set {
            if(this._setPreviusMusicTrack != value && !this._preRaceMode) {
                this._setPreviusMusicTrack = value;
            }
        }
    }

    public bool pauseMusicTrack {
        get {return this._pauseMusicTrack;}
        set {
            if(this._pauseMusicTrack != value && !this._preRaceMode) {
                this._pauseMusicTrack = value;
            }
        }
    }

    public bool setNextMusicTrack {
        get {return this._setNextMusicTrack;}
        set {
            if(this._setNextMusicTrack != value && !this._preRaceMode) {
                this._setNextMusicTrack = value;
            }
        }
    }

    void Start() {
        if(musicOn) {
            if(PlayerPrefs.HasKey("SelectedGenre")) {
                string prefsGenre = PlayerPrefs.GetString("SelectedGenre", "");
                MusicGenre selectedGenre;

                // Попытка парсинга строки в enum
                if(Enum.TryParse(prefsGenre, out selectedGenre)) {
                    this._selectedGenre = selectedGenre;
                    Debug.Log("Успешно распарсено в enum: " + selectedGenre);
                } else {
                    Debug.LogError("Жанра " + prefsGenre + " не существует");
                }
            }

            currentTrack = Random.Range(0, musicList.Count);
            while(musicList[currentTrack].genre != this._selectedGenre) {
                currentTrack = (currentTrack + 1) % musicList.Count;
            }
            InitializeMusicSource();
        }
    }

    private void Update() {
        if(musicOn) {
            if(!setUped) {
                InitializeMusicSource();
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
        if (this._selectedGenre != MusicGenre.Mix) {
            while (musicList[currentTrack].genre != this._selectedGenre) {
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
        while (musicList[currentTrack].genre != this._selectedGenre) {
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
    public MusicGenre genre;
}

using System;
using UnityEngine;
using TrophyRace.Architecture;

public class MusicPlayerInputHandler : MonoBehaviour {

    public static Action<MusicPlayerInputHandler, bool> SetPreviusMusicTrackEvent;
    public static Action<MusicPlayerInputHandler, bool> PauseMusicTrackEvent;
    public static Action<MusicPlayerInputHandler, bool> SetNextMusicTrackEvent;

    [SerializeField]
    private bool _setPreviusMusicTrack = false;
    [SerializeField]
    private bool _pauseMusicTrack = false;
    [SerializeField]
    private bool _setNextMusicTrack = false;
    [SerializeField]
    private InputType control;

    [SerializeField]
    private bool _preRaceMode = false;

    public bool setPreviusMusicTrack {
        get {return this._setPreviusMusicTrack;}
        set {
            if(this._setPreviusMusicTrack != value && !this._preRaceMode) {
                this._setPreviusMusicTrack = value;
                SetPreviusMusicTrackEvent?.Invoke(this, value);
            }
        }
    }

    public bool pauseMusicTrack {
        get {return this._pauseMusicTrack;}
        set {
            if(this._pauseMusicTrack != value && !this._preRaceMode) {
                this._pauseMusicTrack = value;
                PauseMusicTrackEvent?.Invoke(this, value);
            }
        }
    }

    public bool setNextMusicTrack {
        get {return this._setNextMusicTrack;}
        set {
            if(this._setNextMusicTrack != value && !this._preRaceMode) {
                this._setNextMusicTrack = value;
                SetNextMusicTrackEvent?.Invoke(this, value);
            }
        }
    }
}
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

public class MusicPlayer : MonoBehaviour {

    public bool musicOn;
    [SerializeField]
    private AudioMixerGroup audioMixer;
    [SerializeField] [Range(0, 1)]
    public float musicVolume;
    [SerializeField]
    private MusicGenre _selectedGenre;
    private int currentTrack;
    public List<MusicTrackData> musicList = new List<MusicTrackData>();
    private AudioSource source;
    private bool setUped = false, pauseFlag = false;
    private float musicChangeRate;
    [SerializeField]
    private bool _preRaceMode = false;

    private void Start() {
        if (musicOn) {
            LoadMusicTracks();

            if (PlayerPrefs.HasKey("SelectedGenre")) {
                string prefsGenre = PlayerPrefs.GetString("SelectedGenre", "");
                if (Enum.TryParse(prefsGenre, out MusicGenre selectedGenre)) {
                    this._selectedGenre = selectedGenre;
                    Debug.Log("Успешно распарсено в enum: " + selectedGenre);
                } else {
                    Debug.LogError("Жанра " + prefsGenre + " не существует");
                }
            }

            currentTrack = Random.Range(0, musicList.Count);
            while (musicList[currentTrack].genre != this._selectedGenre) {
                currentTrack = (currentTrack + 1) % musicList.Count;
            }
            InitializeMusicSource();
        }
    }

    private void LoadMusicTracks() {
        MusicTrackData[] loadedTracks = Resources.LoadAll<MusicTrackData>("MusicTracks");
        foreach (var track in loadedTracks) {
            if (track.includeTrackInGame) {
                musicList.Add(track);
            }
        }
    }

    IEnumerator WaitMusicEnd() {
        Debug.Log("Now playing: " + musicList[currentTrack].trackName);
        if (musicOn) {
            while (source.isPlaying) {
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
        if (_preRaceMode) return;

        source.Stop();
        UnloadCurrentClip();  // Добавляем выгрузку текущего трека
        currentTrack = (currentTrack - 1 + musicList.Count) % musicList.Count;
        while (this._selectedGenre != MusicGenre.Mix && musicList[currentTrack].genre != this._selectedGenre) {
            currentTrack = (currentTrack - 1 + musicList.Count) % musicList.Count;
        }
        SetMusicClipToSource();
    }

    private void PauseTrack() {
        if (_preRaceMode) return;

        if (!pauseFlag) {
            StopCoroutine("WaitMusicEnd");
            source.Pause();
            pauseFlag = true;
        } else {
            source.Play();
            StartCoroutine("WaitMusicEnd");
            pauseFlag = false;
        }
        musicChangeRate = Time.time + 0.2f;
    }

    private void NextTrack() {
        if (_preRaceMode) return;

        source.Stop();
        UnloadCurrentClip();  // Добавляем выгрузку текущего трека
        currentTrack = (currentTrack + 1) % musicList.Count;
        while (this._selectedGenre != MusicGenre.Mix && musicList[currentTrack].genre != this._selectedGenre) {
            currentTrack = (currentTrack + 1) % musicList.Count;
        }
        SetMusicClipToSource();
    }

    private void SetMusicClipToSource() {
        source.clip = musicList[currentTrack].musicClip;
        source.volume = musicVolume * musicList[currentTrack].musicClipMastering;
        source.time = musicList[currentTrack].initialStartTime;
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
        if (audioMixer != null) {
            source.outputAudioMixerGroup = audioMixer;
        }
        SetMusicClipToSource();
        source.time = musicList[currentTrack].preRaceStartTime;
        setUped = true;
    }

    private void UnloadCurrentClip() {
        if (musicList[currentTrack].musicClip != null) {
            musicList[currentTrack].UnloadClip();
            Resources.UnloadUnusedAssets();
        }
    }

    // Методы для обработки ввода
    public void OnSetPreviusMusicTrack(bool value) {
        if (value) PreviusTrack();
    }

    public void OnPauseMusicTrack(bool value) {
        if (value) PauseTrack();
    }

    public void OnSetNextMusicTrack(bool value) {
        if (value) NextTrack();
    }
}