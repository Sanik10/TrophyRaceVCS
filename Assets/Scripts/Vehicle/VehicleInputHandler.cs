using System;
using UnityEngine;
using TrophyRace.Architecture;

public class VehicleInputHandler : MonoBehaviour {

    public static Action<VehicleInputHandler, bool> ChangeGearUpEvent;
    public static Action<VehicleInputHandler, bool> ChangeGearDownEvent;
    public static Action<VehicleInputHandler> ReverseGearEvent;
    public static Action<VehicleInputHandler, bool> ActivateHighBeamsEvent;
    public static Action<VehicleInputHandler, bool> ActivateDippedBeamsEvent;

    [SerializeField]
    private float _vertical = 0;
    [SerializeField]
    private float _horizontal = 0;
    [SerializeField]
    private bool _handbrake = false;
    [SerializeField]
    private float _clutch = 1;
    [SerializeField]
    private bool _gearUp = false;
    [SerializeField]
    private bool _gearDown = false;
    [SerializeField]
    private bool _reverseGear = false;
    [SerializeField]
    private bool _activateDippedBeams = false;
    [SerializeField]
    private bool _activateHighBeams = false;
    [SerializeField]
    private InputType control;

    [SerializeField]
    private bool _preRaceMode = false;

    public float vertical {
        get {return Mathf.Clamp(this._vertical, -1f, 1f);}
        set {this._vertical = value;}
    }

    public float horizontal {
        get {return Mathf.Clamp(this._horizontal, -1f, 1f);}
        set {this._horizontal = value;}
    }

    public bool handbrake {
        get {return this._handbrake;}
        set {
            if(!this._preRaceMode) {
                this._handbrake = value;
            }
        }
    }

    public float clutch {
        get {return this._clutch;}
        set {this._clutch = value;}
    }

    public bool gearUp {
        get {return this._gearUp;}
        set {
            if(this._gearUp != value) {
                this._gearUp = value;
            }
            ChangeGearUpEvent?.Invoke(this, value);
        }
    }

    public bool gearDown {
        get {return this._gearDown;}
        set {
            if(this._gearDown != value) {
                this._gearDown = value;
            }
            ChangeGearDownEvent?.Invoke(this, value);
        }
    }

    public bool reverseGear {
        get {return this._reverseGear;}
        set {
            if(this._reverseGear != value) {
                this._reverseGear = value;
                if(value) {
                    ReverseGearEvent?.Invoke(this);
                }
            }
        }
    }

    public bool activateDippedBeams {
        get {return this._activateDippedBeams;}
        set {
            if(this._activateDippedBeams != value) {
                this.activateDippedBeams = value;
                ActivateDippedBeamsEvent?.Invoke(this, value);
            }
        }
    }

    public bool activateHighBeams {
        get {return this._activateHighBeams;}
        set {
            if(this._activateHighBeams != value) {
                this._activateHighBeams = value;
                ActivateHighBeamsEvent?.Invoke(this, value);
            }
        }
    }

    private void Start() {
        VehicleManager vehicleManager = GetComponent<VehicleManager>();
        if(vehicleManager.aiVehicle) {
            gameObject.AddComponent<AiInput>();
            control = InputType.Ai;
        } else if(GameObject.Find("CanvasSpawner") != null) {
            var parent = transform.root.gameObject;
            if(control == InputType.Keyboard) {
                Debug.Log("Keyboard input setted");
                GameObject.Find("CanvasSpawner").GetComponent<CanvasSpawner>().SpawnCanvas("Keyboard");
                parent.AddComponent<KeyboardInput>();
            } else if(control == InputType.Mobile) {
                Debug.Log("Mobile input setted");
                GameObject.Find("CanvasSpawner").GetComponent<CanvasSpawner>().SpawnCanvas("Mobile");
                parent.AddComponent<PhoneInput>();
            } else if(control == InputType.Joystick) {
                Debug.Log("Joystick input setted");
                GameObject.Find("CanvasSpawner").GetComponent<CanvasSpawner>().SpawnCanvas("Keyboard");
            } else if(control == InputType.Wheel) {
                Debug.Log("Wheel input setted");
                GameObject.Find("CanvasSpawner").GetComponent<CanvasSpawner>().SpawnCanvas("Keyboard");
            }
        }
    }

    private void OnEnable() {
        GameManager.SetVehiclesInPreRaceModeEvent += PreRaceModeHandler;
        GameManager.StartRaceEvent += StartRaceHandler;
    }

    private void OnDisable() {
        GameManager.SetVehiclesInPreRaceModeEvent -= PreRaceModeHandler;
    }

    private void PreRaceModeHandler() {
        this._preRaceMode = true;
        this._handbrake = true;
        this._clutch = 0;
    }

    private void StartRaceHandler() {
        GameManager.StartRaceEvent -= StartRaceHandler;
        this._preRaceMode = false;
        this._clutch = 1;
        this.handbrake = false;
    }
}

public enum InputType {
    Keyboard, Mobile, Joystick, Wheel, Ai
}