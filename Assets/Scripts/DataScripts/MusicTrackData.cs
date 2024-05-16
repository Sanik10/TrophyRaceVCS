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
    private AudioClip _musicClip;
    [SerializeField]
    private float _musicClipMastering = 1f;
    [SerializeField]
    private float _preRaceStartTime = 0f;
    [SerializeField]
    private float _initialStartTime = 0f;
    [SerializeField]
    private MusicGenre _genre;

    public string guid => this._guid;
    public bool includeTrackInGame => this._includeTrackInGame;
    public string trackName => this._trackName;
    public AudioClip musicClip => this._musicClip;
    public float musicClipMastering => this._musicClipMastering;
    public float preRaceStartTime => this._preRaceStartTime;
    public float initialStartTime => this._initialStartTime;
    public MusicGenre genre => this._genre;

    private void OnValidate() {
        if(string.IsNullOrEmpty(this._guid)) {
            this._guid = Guid.NewGuid().ToString();
        }
    }
}

[System.Serializable]
public enum MusicGenre {
    Mix, Phonk, HipHop, Electro, Rock
}