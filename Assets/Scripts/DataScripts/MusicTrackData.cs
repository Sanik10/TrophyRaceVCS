using System;
using UnityEngine;

[CreateAssetMenu(fileName = "MusicTrackData", menuName = "Music Track Data/Music Track Data File", order = 2)]
public class MusicTrackData : ScriptableObject {
    [SerializeField]
    private string _guid;
    [SerializeField]
    private bool _includeTrackInGame = false;
    [SerializeField]
    private string _trackName = "";
    [SerializeField]
    private string _musicClipPath; // Путь в папке Resources
    [SerializeField]
    private float _musicClipMastering = 1f;
    [SerializeField]
    private float _preRaceStartTime = 0f;
    [SerializeField]
    private float _initialStartTime = 0f;
    [SerializeField]
    private MusicGenre _genre;

    private AudioClip _musicClip;

    public string guid => this._guid;
    public bool includeTrackInGame => this._includeTrackInGame;
    public string trackName => this._trackName;
    public string musicClipPath => this._musicClipPath;
    public float musicClipMastering => this._musicClipMastering;
    public float preRaceStartTime => this._preRaceStartTime;
    public float initialStartTime => this._initialStartTime;
    public MusicGenre genre => this._genre;

    public AudioClip musicClip {
        get {
            if (_musicClip == null) {
                _musicClip = Resources.Load<AudioClip>(_musicClipPath);
            }
            return _musicClip;
        }
    }

    private void OnValidate() {
        if (string.IsNullOrEmpty(this._guid)) {
            this._guid = Guid.NewGuid().ToString();
        }
    }

    public void UnloadClip() {
        _musicClip = null;
    }
}

[System.Serializable]
public enum MusicGenre {
    Mix, Phonk, HipHop, Electro, Rock, MainMenuOST
}