using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
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
        // Загрузка треков не из Resources, а использование уже настроенного списка musicList
        InitializeMusicSource();
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
        var currentTrackData = musicList[currentTrack];
        Addressables.LoadAssetAsync<AudioClip>(currentTrackData.musicClipAddress).Completed += handle => {
            if (handle.Status == AsyncOperationStatus.Succeeded) {
                source.clip = handle.Result;
                source.volume = musicVolume * currentTrackData.musicClipMastering;
                source.time = (this._preRaceMode) ? currentTrackData.preRaceStartTime : currentTrackData.initialStartTime;
                source.Play();
                StartCoroutine("WaitMusicEnd");
                musicChangeRate = Time.time + 0.2f;
            } else {
                Debug.LogError($"Failed to load music clip: {currentTrackData.musicClipAddress}");
            }
        };
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
        setUped = true;
    }

    private void UnloadCurrentClip() {
        if (musicList[currentTrack].musicClip != null) {
            musicList[currentTrack].UnloadClip();
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