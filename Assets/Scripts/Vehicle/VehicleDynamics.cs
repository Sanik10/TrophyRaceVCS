using System;
using System.Collections.Generic;
using UnityEngine;

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



    [Header("Колёса")]
    [Range(0.1f, 1)]
    public float wheelsResistans = 1;
    public float wheelsRPM = 0;
    [SerializeField]
    private float _wheelRadius = 0;
    private float _circumFerence = 0;
    public float maxSpeedOnCurrentGear = 0;
    public bool isBraking = true;
    [HideInInspector]
    public WheelCollider[] wheelColliders;

    private float _allMileage;
    private float _maxSpeed = 0;
    private float _wheelBase = 0;
    private float _rearTrack = 0;
    private float steeringWheelSpeed = 0;
    [SerializeField]
    private float[] currentWheelSpeed;
    private driveType driveWheels;
    private int _torqueDevider;

    [Header("Управление")]
    public float horizontal = 0;
    public float totalSlip = 0;
    public float currentMileage;
    private float _brakingDistribution = 0;
    private float _brakingVariablePower = 0;
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
    private int _wheelsQuantity = 0;

    public float allMileage => this._allMileage;
    public float circumFerence => this._circumFerence;


    private void Start() {
        VehicleManager = GetComponent<VehicleManager>();
        GettingScriptableValues();
        Engine = VehicleManager.Engine;
        Transmission = VehicleManager.Transmission;
        PC = VehicleManager.PhysicsCalculation;
        this._VehicleInputHandler = VehicleManager.VehicleInputHandler;
        findValues();
    }

    private void findValues() {
        foreach(Transform i in gameObject.transform) {
            if(i.transform.name == "carColliders") {
                wheelColliders = new WheelCollider[i.transform.childCount];
                for(int q = 0; q < i.transform.childCount; q++) {
                    wheelColliders[q] = i.transform.GetChild(q).GetComponent<WheelCollider>();
                }    
            }
        }

        float radiusSum = 0;
        /* Старый принцип расчета радиуса ведущих колес
            if(driveWheels == driveType.rearWheelsDrive) {
                for(int i = 2; i < 4; i++) {
                    radiusSum += wheelColliders[i].radius;
                    this._wheelsQuantity = 2;
                }
            } else if(driveWheels == driveType.frontWeelsDrive) {
                for(int i = 0; i < 2; i++) {
                    radiusSum += wheelColliders[i].radius;
                    this._wheelsQuantity = 2;
                }
            } else {
                for(int i = 0; i < 4; i++) {
                    radiusSum += wheelColliders[i].radius;
                    this._wheelsQuantity = 4;
                }
            }
        */
        for(int i = 0; i < this._axles.Count; i++) {
            if(this._axles[i].powered) {
                for(int q = 0; q < this._axles[i].wheelsColliders.Length; q++) {
                    radiusSum += this._axles[i].wheelsColliders[q].radius;
                    this._wheelsQuantity += 1;
                }
            }
        }
        this._wheelRadius = (this._wheelsQuantity != 0) ? radiusSum / this._wheelsQuantity : 0;
        this._circumFerence = this._wheelRadius * 6.283185307179f;

        wheelPowerStats = new float[wheelColliders.Length];
        wheelRpmStats = new float[wheelColliders.Length];
        brakingPowerStats = new float[wheelColliders.Length];
        maxRpmOnWheel = new float[wheelColliders.Length];
        currentWheelSpeed = new float[wheelColliders.Length];
        VehicleDynamicsInitializedEvent?.Invoke(this);
    }

    private void FixedUpdate() {
        for(int i = 0; i < wheelColliders.Length; i++) {
            if(track) {
                brakingPowerStats[i] = wheelColliders[i].brakeTorque;
                wheelPowerStats[i] = wheelColliders[i].motorTorque;
                wheelRpmStats[i] = wheelColliders[i].rpm;
                if(wheelColliders[i].rpm > maxRpmOnWheel[i]) {
                    maxRpmOnWheel[i] = wheelColliders[i].rpm;
                }
            }
            currentWheelSpeed[i] = this._circumFerence * wheelColliders[i].rpm * 0.06f;
        }

        maxSpeedOnCurrentGear = Engine.maxRpm * this._circumFerence / 16.668f / (Transmission.currentGearRatio * Transmission.finalDrive);

        DriveWheelsRpm();
        VehicleSteering();
        BrakingVehicle();
        currentMileage += this._circumFerence * Mathf.Abs(wheelsRPM) / 10000;
    }

    private void DriveWheelsRpm() {
        float RpmSum = 0;
        if(driveWheels == driveType.rearWheelsDrive) {
            for(int i = 2; i < 4; i++) {
                RpmSum += wheelColliders[i].rpm;
            }
        } else if(driveWheels == driveType.frontWeelsDrive) {
            for(int i = 0; i < 2; i++) {
                RpmSum += wheelColliders[i].rpm;
            }
        } else {
            for(int i = 0; i < 4; i++) {
                RpmSum += wheelColliders[i].rpm;
            }
        }

        wheelsRPM = (this._wheelsQuantity != 0) ? RpmSum / this._wheelsQuantity : 0;
        TorqueDestribution();
    }

    private void TorqueDestribution() {
        for(int i = 0; i < wheelColliders.Length; i++) {
            if(
            (driveWheels == driveType.rearWheelsDrive && i >= 2) ||
            (driveWheels == driveType.frontWeelsDrive && i < wheelColliders.Length - 2) ||
            (driveWheels == driveType.allWheelsDrive)) {
                wheelColliders[i].motorTorque = (Mathf.Abs(currentWheelSpeed[i]) > Mathf.Abs(maxSpeedOnCurrentGear) || Mathf.Abs(PC.Kph) > Mathf.Abs(this._maxSpeed) || this._VehicleInputHandler.vertical < 0 || Transmission.neutralGear) ? 0 : Engine.torque * wheelsResistans / _torqueDevider;
            }
        }
    }

    private void BrakingVehicle() {
        for (int i = 0; i < wheelColliders.Length; i++) {
            this._brakingPower = (this._VehicleInputHandler.vertical < 0) ? this._brakingVariablePower : 0;

            wheelColliders[0].brakeTorque = wheelColliders[1].brakeTorque = this._brakingPower * (1 - this._brakingDistribution);
            wheelColliders[2].brakeTorque = wheelColliders[3].brakeTorque = this._brakingPower * this._brakingDistribution;

            if(this._VehicleInputHandler.handbrake && this._VehicleInputHandler.horizontal > 0.3f) {
                wheelColliders[3].brakeTorque = 10000;
            } else if(this._VehicleInputHandler.handbrake && this._VehicleInputHandler.horizontal < -0.3f) {
                wheelColliders[2].brakeTorque = 10000;
            } else if(this._VehicleInputHandler.handbrake) {
                wheelColliders[3].brakeTorque = wheelColliders[2].brakeTorque = 10000;
            }

            isBraking = (this._brakingPower > 0) ? true : false;
        }
    }

    private void VehicleSteering() {
        horizontal = Mathf.Lerp(this.horizontal, this._VehicleInputHandler.horizontal, steeringWheelSpeed/100);

        wheelColliders[0].steerAngle = (horizontal > 0) ? Mathf.Rad2Deg * Mathf.Atan(this._wheelBase / (this._radius + (this._rearTrack / 2))) * horizontal : (horizontal < 0) ? Mathf.Rad2Deg * Mathf.Atan(_wheelBase / (this._radius - (_rearTrack / 2))) * horizontal : 0;
        wheelColliders[1].steerAngle = (horizontal > 0) ? Mathf.Rad2Deg * Mathf.Atan(this._wheelBase / (this._radius - (this._rearTrack / 2))) * horizontal : (horizontal < 0) ? Mathf.Rad2Deg * Mathf.Atan(_wheelBase / (this._radius + (_rearTrack / 2))) * horizontal : 0;
    }

    private void GettingScriptableValues() {
        VehicleData vehicleData = this.VehicleManager.vehicleData;
        this._maxSpeed = vehicleData.maxSpeed;
        driveWheels = vehicleData.driveWheels;
        this._torqueDevider = vehicleData.torqueDevider;
        this._brakingVariablePower = vehicleData.brakingPowerVar;
        this._brakingDistribution = vehicleData.brakingDistribution;
        this._radius = vehicleData.radius;
        this.steeringWheelSpeed = vehicleData.steeringWheelSpeed;
        this._wheelBase = vehicleData.wheelBase + 2;
        this._rearTrack = vehicleData.rearTrack;
        this._allMileage = vehicleData.mileage;
    }

    private void OnDestroy() {
        if(gameObject.tag == "Player") {
            this.VehicleManager.vehicleData.mileage += currentMileage;
            this.VehicleManager.vehicleData.Save("WheelsSettings");
        }
    }
}

[System.Serializable]
public class AxleSettings {
    public bool front;
    public WheelCollider[] wheelsColliders;
    public bool powered;
    public bool steering;
}