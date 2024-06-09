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