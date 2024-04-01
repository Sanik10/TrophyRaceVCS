using System;
using System.Collections.Generic;
using UnityEngine;
using NWH.WheelController3D;
using NWH.Common.Vehicles;

public class VehicleDynamics : MonoBehaviour {

    public static Action<VehicleDynamics> VehicleDynamicsInitializedEvent;

    private VehicleManager VehicleManager;
    private Engine Engine;
    private Transmission Transmission;
    private PhysicsCalculation PC;
    private VehicleInputHandler _VehicleInputHandler;

    [Header("Конфигурация осей авто")]
    [SerializeField]
    private List<AxleSettings> _axles = new List<AxleSettings>();
    private int _frontSteeringAxles = 0;
    private int _rearSteeringAxles = 0;

    // private WheelController[] _wheelControllers;
    private WheelUAPI[] _wheelControllers;
    [Range(0.1f, 1)] [SerializeField]
    private float _wheelsResistance = 1;
    private float _allMileage;
    private float _driveWheelsRpm = 0;
    private float _wheelRadius = 0;
    [SerializeField]
    private float _circumFerence = 0;
    [SerializeField]
    private float _maxSpeedOnCurrentGear = 0;
    private bool _braking = true;
    private float _maxSpeed = 0;
    private float _wheelBase = 0;
    private float _rearTrack = 0;
    private float steeringWheelSpeed = 0;
    [SerializeField]
    private float[] currentWheelSpeed;
    private driveType driveWheels;
    private int _torqueDevider;
    private float _brakingDistribution = 0;
    private float _totalBrakePower = 0;

    [Header("Управление")]
    private float _horizontal = 0;
    private float _currentMileage = 0;
    private float _radius = 0;
    private float _brakingPower = 0;

    [Header("Отладка")]
    public bool track = false;
    public float[] maxRpmOnWheel;
    [SerializeField]
    private float[] wheelPowerStats;
    [SerializeField]
    private float[] wheelRpmStats;
    [SerializeField]
    private float[] brakingPowerStats;
    private int _driveWheelsQuantity = 0;

    public List<AxleSettings> axles => this._axles;
    public float allMileage => this._allMileage;
    public float maxSpeedOnCurrentGear => this._maxSpeedOnCurrentGear;
    public float brakingDistribution => this._brakingDistribution;
    public float circumFerence => this._circumFerence;
    public float currentMileage => this._currentMileage;
    public float driveWheelsRpm => this._driveWheelsRpm;
    public float horizontal => this._horizontal;
    public float totalBrakePower => this._totalBrakePower;
    public float wheelRadius => this._wheelRadius;
    public bool isBraking => this._braking;


    private void Start() {
        VehicleManager = GetComponent<VehicleManager>();
        GettingScriptableValues();
        Engine = VehicleManager.Engine;
        Transmission = GetComponent<Transmission>();
        PC = VehicleManager.PhysicsCalculation;
        this._VehicleInputHandler = VehicleManager.VehicleInputHandler;
        findValues();
    }

    private void findValues() {
        foreach(Transform i in gameObject.transform) {
            if(i.transform.name == "carColliders") {
                this._wheelControllers = new WheelController[i.transform.childCount];
                // this._wheelControllersAPI = new WheelUAPI[i.transform.childCount];
                for(int q = 0; q < i.transform.childCount; q++) {
                    this._wheelControllers[q] = i.transform.GetChild(q).GetComponent<WheelController>();
                    // this._wheelControllersAPI[q] = this._wheelControllers[q].wheelUAPI;
                }    
            }
        }

        float radiusSum = 0;

        for(int i = 0; i < this._axles.Count; i++) {
            if(this._axles[i].powered) {
                for(int q = 0; q < this._axles[i].wheelsControllers.Length; q++) {
                    radiusSum += this._axles[i].wheelsControllers[q].Radius;
                    this._driveWheelsQuantity += 1;
                }
                this._wheelRadius = (this._driveWheelsQuantity != 0) ? radiusSum / this._driveWheelsQuantity : 0;
            }

            if(this._axles[i].steering) {
                if(this.axles[i].front) {
                    this._frontSteeringAxles++;
                } else {
                    this._rearSteeringAxles++;
                }
            }
        }
        int lastAxlePositionInList = this._axles.Count-1;
        int lastWheelColliderInLastAxle = this._axles[lastAxlePositionInList].wheelsControllers.Length - 1;

        this._wheelBase = Mathf.Abs(this._axles[0].wheelsControllers[0].transform.localPosition.z) + Mathf.Abs(this._axles[lastAxlePositionInList].wheelsControllers[lastWheelColliderInLastAxle].transform.localPosition.z);
        // this._rearTrack = (Mathf.Abs(this._axles[lastAxlePositionInList].wheelsColliders[0].transform.localPosition.x) + Mathf.Abs(this._axles[lastAxlePositionInList].wheelsColliders[lastWheelColliderInLastAxle].transform.localPosition.x)) / 2;
        this._rearTrack = (Mathf.Abs(this._axles[lastAxlePositionInList].wheelsControllers[0].transform.localPosition.x) + Mathf.Abs(this._axles[lastAxlePositionInList].wheelsControllers[lastWheelColliderInLastAxle].transform.localPosition.x)) * 0.32520325203252f;

        this._circumFerence = this._wheelRadius * 6.283185307179f;

        wheelPowerStats = new float[this._wheelControllers.Length];
        wheelRpmStats = new float[this._wheelControllers.Length];
        brakingPowerStats = new float[this._wheelControllers.Length];
        maxRpmOnWheel = new float[this._wheelControllers.Length];
        currentWheelSpeed = new float[this._wheelControllers.Length];
        VehicleDynamicsInitializedEvent?.Invoke(this);
    }

    private void FixedUpdate() {
        for(int i = 0; i < this._wheelControllers.Length; i++) {
            if(track) {
                brakingPowerStats[i] = this._wheelControllers[i].BrakeTorque;
                wheelPowerStats[i] = this._wheelControllers[i].MotorTorque;
                wheelRpmStats[i] = this._wheelControllers[i].RPM;
                if(this._wheelControllers[i].RPM > maxRpmOnWheel[i]) {
                    maxRpmOnWheel[i] = this._wheelControllers[i].RPM;
                }
            }
            currentWheelSpeed[i] = this._circumFerence * this._wheelControllers[i].RPM * 0.06f;
        }

        // maxSpeedOnCurrentGear = Engine.maxRpm * this._circumFerence / 16.668f / (Transmission.currentGearRatio * Transmission.finalDrive);
        this._maxSpeedOnCurrentGear = 6.283185307179f * Engine.maxRpm / this._circumFerence / Transmission.currentGearRatio * Transmission.finalDrive * this._wheelRadius * 0.06f;
        // maxSpeedOnCurrentGear = (Engine.maxRpm * this._circumFerence / 3.1415f) / (Transmission.currentGearRatio * Transmission.finalDrive * 16.668f);
        DriveWheelsRpm();
        VehicleSteering();
        BrakingVehicle();

        this._currentMileage += this._circumFerence * Mathf.Abs(this._driveWheelsRpm) / 10000;
    }

    private void DriveWheelsRpm() {
        float rpmSum = 0;

        for(int i = 0; i < this._axles.Count; i++) {
            for(int q = 0; q < this._axles[i].wheelsControllers.Length; q++) {
                rpmSum += this._axles[i].powered ? this._axles[i].wheelsControllers[q].RPM : 0;
            }
        }
        this._driveWheelsRpm = (this._driveWheelsQuantity != 0) ? rpmSum / this._driveWheelsQuantity : 0;

        TorqueDestribution();
    }

    private void TorqueDestribution() {
        for (int i = 0; i < this._axles.Count; i++) {
            for (int q = 0; q < this._axles[i].wheelsControllers.Length; q++) {
                this._axles[i].wheelsControllers[q].MotorTorque = 
                    (this._axles[i].powered)
                        ? (Mathf.Abs(currentWheelSpeed[i]) > Mathf.Abs(this._maxSpeedOnCurrentGear) || Mathf.Abs(PC.kph) > Mathf.Abs(this._maxSpeed) || this._VehicleInputHandler.vertical < 0 || Transmission.neutralGear)
                            ? 0
                            : Engine.torque * this._wheelsResistance //this._driveWheelsQuantity
                        : 0;
            }
        }
    }

    private void BrakingVehicle() {
        this._brakingPower = (this._VehicleInputHandler.vertical < 0) ? this._totalBrakePower : 0;

        for(int i = 0; i < this._axles.Count; i++) {
            for(int q = 0; q < this._axles[i].wheelsControllers.Length; q++) {
                this._axles[i].wheelsControllers[q].BrakeTorque =
                    (this._VehicleInputHandler.vertical < 0)
                        ? ((this._axles[i].front)
                            ? (this._brakingPower * (1 - this._brakingDistribution))
                            : this._brakingPower * this._brakingDistribution)
                        : (this._VehicleInputHandler.handbrake && !this._axles[i].front)
                            ? (Mathf.Abs(this._VehicleInputHandler.horizontal) < 0.3f)
                                ? 10000
                                : ((this._VehicleInputHandler.horizontal > 0.3f && q % 2 != 0) || (this._VehicleInputHandler.horizontal < -0.3f && q % 2 == 0))
                                    ? 10000
                                    : 0
                            : 0;
            }
        }

        this._braking = (this._brakingPower > 0) ? true : false;
    }

    private void VehicleSteering() {
        int currentFrontSteeringAxle = 1;
        int currentRearSteeringAxle = 1;
        this._horizontal = Mathf.Lerp(this._horizontal, this._VehicleInputHandler.horizontal, steeringWheelSpeed/100);

        for(int i = 0; i < this._axles.Count; i++) {
            if(this._axles[i].steering) {
                for (int q = 0; q < this._axles[i].wheelsControllers.Length; q++) {
                    this._axles[i].wheelsControllers[q].SteerAngle = 
                        (this._horizontal > 0) 
                            ? Mathf.Rad2Deg * Mathf.Atan(this._wheelBase / (this._radius + (q % 2 == 0 
                                ? (this._rearTrack) 
                                : -this._rearTrack))) * (this._axles[i].front 
                                    ? this._horizontal 
                                    : -this._horizontal) * (this._axles[i].front 
                                        ? ((float)currentFrontSteeringAxle / this._frontSteeringAxles) 
                                        : ((float)currentRearSteeringAxle / this._rearSteeringAxles))
                            : (this._horizontal < 0)
                                ? Mathf.Rad2Deg * Mathf.Atan(this._wheelBase / (this._radius - (q % 2 == 0 
                                    ? (this._rearTrack) 
                                    : -this._rearTrack))) * (this._axles[i].front 
                                        ? this._horizontal 
                                        : -this._horizontal) * (this._axles[i].front 
                                            ? ((float)currentFrontSteeringAxle / this._frontSteeringAxles) 
                                            : ((float)currentRearSteeringAxle / this._rearSteeringAxles))
                                : 0;
                }

                if(this._axles[i].front) {
                    currentFrontSteeringAxle++;
                } else {
                    currentRearSteeringAxle++;
                }
            }
        }
    }

    private void GettingScriptableValues() {
        VehicleData vehicleData = this.VehicleManager.vehicleData;
        this._maxSpeed = vehicleData.maxSpeed;
        driveWheels = vehicleData.driveWheels;
        this._torqueDevider = vehicleData.torqueDevider;
        this._totalBrakePower = vehicleData.brakingPowerVar;
        this._brakingDistribution = vehicleData.brakingDistribution;
        this._radius = vehicleData.radius;
        this.steeringWheelSpeed = vehicleData.steeringWheelSpeed;
        this._wheelBase = vehicleData.wheelBase + 2;
        this._rearTrack = vehicleData.rearTrack;
        this._allMileage = vehicleData.mileage;
    }

    private void OnDestroy() {
        if(gameObject.tag == "Player") {
            this.VehicleManager.vehicleData.mileage += this._currentMileage;
            this.VehicleManager.vehicleData.Save("WheelsSettings");
        }
    }
}

[System.Serializable]
public class AxleSettings {
    public bool front;
    public WheelUAPI[] wheelsControllers;
    public bool powered;
    public bool steering;
}