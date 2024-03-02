using UnityEngine;
using TrophyRace.Architecture;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Engine))]
[RequireComponent(typeof(Transmission))]
[RequireComponent(typeof(VehicleDynamics))]
[RequireComponent(typeof(TiresFriction))]
[RequireComponent(typeof(PhysicsCalculation))]
[RequireComponent(typeof(VehicleInputHandler))]
public class VehicleManager : MonoBehaviour {

    public string vehicleName = "";
    public int id => this._id;
    public bool aiVehicle = false;
    public bool turnOffCameras = false;
    public bool chaseVehicle = false;

    private int _id = 0;
    [Header("Components")]
    private VehicleData _vehicleData;
    public VehicleData vehicleData {
        get {return this._vehicleData;}
        set {this._vehicleData = value;}
    }
    public Engine Engine;
    public Transmission Transmission;
    public VehicleDynamics VehicleDynamics;
    public TiresFriction TiresFriction;
    public PhysicsCalculation PhysicsCalculation;
    public RealisticEngineSound[] RES;
    public VehicleSFX VehicleSFX;
    public VehicleVFX VehicleVFX;
    public VehicleInfo VehicleInfo;
    public VehicleInputHandler VehicleInputHandler;

    private void Start() {
        this._id = this._vehicleData.id;
        if(aiVehicle) {
            gameObject.tag = "Ai";
        }
        this.VehicleInfo = GetComponent<VehicleInfo>();
        this.Engine = GetComponent<Engine>();
        this.Transmission = GetComponent<Transmission>();
        this.VehicleDynamics = GetComponent<VehicleDynamics>();
        this.TiresFriction = GetComponent<TiresFriction>();
        this.RES = GetComponentsInChildren<RealisticEngineSound>();
        this.VehicleSFX = GetComponent<VehicleSFX>();
        this.VehicleVFX = GetComponent<VehicleVFX>();
        this.VehicleInputHandler = GetComponent<VehicleInputHandler>();
        if(this.vehicleName == null) {
            this.vehicleName = this.VehicleInfo.Name;
        }
    }

    private void OnEnable() {
        GameManager.SetVehiclesInPreRaceModeEvent += PreRaceModeHandler;
    }

    private void OnDisable() {
        GameManager.SetVehiclesInPreRaceModeEvent -= PreRaceModeHandler;
    }

    private void FixedUpdate() {
        for (int i = 0; i < RES.Length; i++) {
            RES[i].maxRPMLimit = Engine.maxRpm;
            RES[i].carMaxSpeed = 350;
            RES[i].carCurrentSpeed = PhysicsCalculation.kph;
            RES[i].engineCurrentRPM = Engine.rpm;
            RES[i].gasPedalValue = (VehicleInputHandler.vertical > 0) ? Engine.throttle : 0;
            RES[i].gasPedalPressing = (VehicleInputHandler.vertical > 0);
            RES[i].isReversing = (Transmission.currentGearRatio < 0);
        }
    }

    private void PreRaceModeHandler() {
        VehicleInputHandler.handbrake = true;
    }

    // void OnGUI(){
    //     float pos = 50;

    //     GUI.Label(new Rect(20, pos, 200, 20),"currentGear: " + Transmission.currentGear.ToString("0"));
    //     pos+=25f;
    //     GUI.Label(new Rect(20, pos, 200, 20),"Torque: " + Engine.torque.ToString("0"));
    //     pos+=25f;
    //     GUI.Label(new Rect(20, pos, 200, 20),"KPH : " + PhysicsCalculation.speed.ToString("0"));
    //     pos+=25f;
    //     GUI.Label(new Rect(20, pos, 200, 20),"Нагрузка на двигатель :");
    //     pos+=20f;
    //     GUI.HorizontalSlider(new Rect(20, pos, 200, 20), Engine.engineLoad, -1.0F, 1.0F);
    //     pos+=25f;
    //     GUI.Label(new Rect(20, pos, 200, 20),"Акселератор :");
    //     pos+=20f;
    //     GUI.HorizontalSlider(new Rect(20, pos, 200, 20), Mathf.Abs(Engine.throttle), 0F, 1.0F);
    //     pos+=25f;
    //     GUI.Label(new Rect(20, pos, 200, 20),"Тормоз :");
    //     pos+=20f;
    //     GUI.HorizontalSlider(new Rect(20, pos, 200, 20), Mathf.Abs(VehicleDynamics.braking), 0F, 1.0F);
    //     pos+=25f;
    //     GUI.Label(new Rect(20, pos, 200, 20),"brakes: " + VehicleDynamics.brakePower.ToString("0"));
    //     pos+=25f;
    // }
}