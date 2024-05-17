using UnityEngine;

public class KeyboardInput : MonoBehaviour {

    private VehicleInputHandler _VehicleInputHandler;
    private float _buttonDelay;
    private MusicPlayer _musicPlayer;

    private void Start() {
        this._VehicleInputHandler = GetComponent<VehicleInputHandler>();
        this._musicPlayer = FindObjectOfType<MusicPlayer>();
    }

    private void Update() {
        this._VehicleInputHandler.vertical = Input.GetAxis("Vertical");
        this._VehicleInputHandler.horizontal = Input.GetAxis("Horizontal");
        this._VehicleInputHandler.handbrake = (Input.GetAxis("Jump") != 0) ? true : false;
        this._VehicleInputHandler.clutch = (Input.GetKey(KeyCode.E) == true) ? 0 : 1;
        this._VehicleInputHandler.gearUp = (Input.GetAxis("Fire1") != 0) ? true : false;
        this._VehicleInputHandler.gearDown = (Input.GetAxis("Fire2") != 0) ? true : false;

        if (Input.GetKey(KeyCode.R)) {
            this._VehicleInputHandler.reverseGear = true;
        }

        if (Input.GetKey(KeyCode.N) && Time.time >= this._buttonDelay) {
            this._buttonDelay = Time.time + 0.04f;
            this._VehicleInputHandler.activateHighBeams = !this._VehicleInputHandler.activateHighBeams;
        }

        if (Input.GetKey(KeyCode.B) && Time.time >= this._buttonDelay) {
            this._buttonDelay = Time.time + 0.04f;
            this._VehicleInputHandler.activateDippedBeams = !this._VehicleInputHandler.activateDippedBeams;
        }

        // Обработка музыкальных команд
        if (Input.GetKey(KeyCode.J) && Time.time >= this._buttonDelay) {
            this._buttonDelay = Time.time + 0.04f;
            _musicPlayer.OnSetPreviusMusicTrack(true);
        }

        if (Input.GetKey(KeyCode.K) && Time.time >= this._buttonDelay) {
            this._buttonDelay = Time.time + 0.04f;
            _musicPlayer.OnPauseMusicTrack(true);
        }

        if (Input.GetKey(KeyCode.L) && Time.time >= this._buttonDelay) {
            this._buttonDelay = Time.time + 0.04f;
            _musicPlayer.OnSetNextMusicTrack(true);
        }
    }
}