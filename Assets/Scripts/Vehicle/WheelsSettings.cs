using System;
using UnityEngine;

public class WheelsSettings : MonoBehaviour {

    public static Action<WheelsSettings> WheelsSettingsInitializedEvent;

    private VehicleManager VehicleManager;
    private Engine Engine;
    private Transmission Transmission;
    private PhysicsCalculation PC;
    private VehicleInputHandler _VehicleInputHandler;

    [Header("Колёса")]
    [Range(0.1f, 1)]
    public float wheelsResistans = 1;
    public float wheelsRPM = 0;
    public float wheelRadius = 0;
    public float circumFerence = 0;
    public float maxSpeedOnCurrentGear = 0;
    public float originExtremumValue = 0;
    public float allMileage => this._allMileage;
    public float groundFriction => this._groundFriction;
    public bool isBraking = true;
    // private terrainType[] currentSurface;
    [HideInInspector]
    public WheelCollider[] wheelColliders;

    private float _allMileage;
    private float _maxSpeed = 0;
    private float _wheelBase = 0;
    private float _rearTrack = 0;
    private float steeringWheelSpeed = 0;
    [SerializeField]
    private float _groundFriction = 0;
    [SerializeField]
    private float[] currentWheelSpeed;
    private float[] originalSidewaysStiffness;
    private float[] originalForwardStiffness;
    private driveType driveWheels;
    private int _torqueDevider;
    private WheelFrictionCurve[] sFriction, fFriction;
    private Vector3 wheelPosition;
    private Quaternion wheelRotation;

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
    public float[] wheelSlip;
    [SerializeField]
    private float[] wheelPowerStats;
    [SerializeField]
    private float[] wheelRpmStats;
    [SerializeField]
    private float[] brakingPowerStats;
    private int wheelsQuantity = 0;

    // private float frontAxleBraking;
    // private float rearAxleBraking;
    // public bool frontAxleCamber = false;
    // public Transform FrontAxle;
    // public bool rearAxleCamber = false;
    // public Transform RearAxle;
    // public float frontCenterOfWheel, rearCenterOfWheel;
    // private float frontCamber, rearCamber, camber;

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

        float RadiusSum = 0;
        if(driveWheels == driveType.rearWheelsDrive) {
            for(int i = 2; i < 4; i++) {
                RadiusSum += wheelColliders[i].radius;
                wheelsQuantity = 2;
            }
        } else if(driveWheels == driveType.frontWeelsDrive) {
            for(int i = 0; i < 2; i++) {
                RadiusSum += wheelColliders[i].radius;
                wheelsQuantity = 2;
            }
        } else {
            for(int i = 0; i < 4; i++) {
                RadiusSum += wheelColliders[i].radius;
                wheelsQuantity = 4;
            }
        }
        wheelRadius = (wheelsQuantity != 0) ? RadiusSum / wheelsQuantity : 0;
        circumFerence = wheelRadius * 6.283185307179f;

        originalSidewaysStiffness = originalForwardStiffness = new float[4];
        sFriction = fFriction = new WheelFrictionCurve[4];

        
        for(int i = 0; i < 4; i++) {
            originalSidewaysStiffness[i] = wheelColliders[i].sidewaysFriction.stiffness;
            originalForwardStiffness[i] = wheelColliders[i].forwardFriction.stiffness;
        }

        wheelSlip = new float[wheelColliders.Length];
        wheelPowerStats = new float[wheelColliders.Length];
        wheelRpmStats = new float[wheelColliders.Length];
        brakingPowerStats = new float[wheelColliders.Length];
        maxRpmOnWheel = new float[wheelColliders.Length];
        currentWheelSpeed = new float[wheelColliders.Length];
        // currentSurface = new terrainType[wheelColliders.Length];
        WheelsSettingsInitializedEvent?.Invoke(this);
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
            currentWheelSpeed[i] = circumFerence * wheelColliders[i].rpm * 0.06f;
        }

        maxSpeedOnCurrentGear = Engine.maxRpm * circumFerence / 16.668f / (Transmission.currentGearRatio * Transmission.finalDrive);

        // GroundCheck();
        DriveWheelsRpm();
        VehicleSteering();
        BrakingVehicle();
        Friction();
        currentMileage += circumFerence * Mathf.Abs(wheelsRPM) / 10000;
    }

    /* private void GroundCheck() {
        WheelHit hit;
        for(int i = 0; i < wheelColliders.Length; i++) {
            if(wheelColliders[i].GetGroundHit(out hit)) {
                currentSurface[i] = 
                (hit.collider.material.dynamicFriction == 1) ? terrainType.Asphalt
                :
                (hit.collider.material.dynamicFriction == 0.85f) ? terrainType.Grass 
                :
                (hit.collider.material.dynamicFriction == 0.8f) ? terrainType.Sand
                :
                (hit.collider.material.dynamicFriction == 0.7f) ? terrainType.Water
                :
                terrainType.NotStated;
            }
        }
    } */

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

        wheelsRPM = (wheelsQuantity != 0) ? RpmSum / wheelsQuantity : 0;
        TorqueDestribution();
    }

    private void TorqueDestribution() {
        if(driveWheels == driveType.rearWheelsDrive) {
            for(int i = 2; i < wheelColliders.Length; i++) {
                wheelColliders[i].motorTorque = (Mathf.Abs(currentWheelSpeed[i]) > Mathf.Abs(maxSpeedOnCurrentGear) || Mathf.Abs(PC.Kph) > Mathf.Abs(this._maxSpeed) || this._VehicleInputHandler.vertical <= 0  || Transmission.neutralGear) ? 0 : Engine.torque * wheelsResistans / _torqueDevider;
            }
        } else if(driveWheels == driveType.frontWeelsDrive) {
            for(int i = 0; i < wheelColliders.Length - 2; i++) {
                wheelColliders[i].motorTorque = (Mathf.Abs(currentWheelSpeed[i]) > Mathf.Abs(maxSpeedOnCurrentGear) || Mathf.Abs(PC.Kph) > Mathf.Abs(this._maxSpeed) || this._VehicleInputHandler.vertical <= 0 || Transmission.neutralGear) ? 0 : Engine.torque * wheelsResistans / _torqueDevider;
            }
        } else {
            for(int i = 0; i < wheelColliders.Length; i++) {
                wheelColliders[i].motorTorque = (Mathf.Abs(currentWheelSpeed[i]) > Mathf.Abs(maxSpeedOnCurrentGear) || Mathf.Abs(PC.Kph) > Mathf.Abs(this._maxSpeed) || this._VehicleInputHandler.vertical <= 0 || Transmission.neutralGear) ? 0 : Engine.torque * wheelsResistans / _torqueDevider;
            }
        }
    }

    private void BrakingVehicle() {
        for (int i = 0; i < wheelColliders.Length; i++) {
            this._brakingPower = (this._VehicleInputHandler.vertical < 0) ? this._brakingVariablePower : (this._VehicleInputHandler.vertical == 0 && PC.Kph <= 10 && PC.Kph >= -10) ? 300 : 0;

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

    private void Friction() {
        float[] groundStaticFriction = new float[4];
        WheelHit hit;
        for(int i = 0; i < wheelColliders.Length; i++) {
            fFriction[i] = wheelColliders[i].forwardFriction;
            sFriction[i] = wheelColliders[i].sidewaysFriction;
            if(wheelColliders[i].GetGroundHit(out hit)) {
                groundStaticFriction[i] = hit.collider.material.staticFriction;
                fFriction[i].stiffness = hit.collider.material.staticFriction * originalForwardStiffness[i];
                sFriction[0].extremumValue = sFriction[1].extremumValue = originExtremumValue;
                sFriction[2].extremumValue = sFriction[3].extremumValue = (this._VehicleInputHandler.handbrake && (this._VehicleInputHandler.horizontal > 0.5f || this._VehicleInputHandler.horizontal < -0.5f)) ? 0.2f : originExtremumValue;
            }
            wheelColliders[i].forwardFriction = fFriction[i];
            wheelColliders[i].sidewaysFriction = sFriction[i];
            wheelSlip[i] = Mathf.Abs(hit.forwardSlip) + Mathf.Abs(hit.sidewaysSlip);
        }
        totalSlip = (wheelSlip[0] + wheelSlip[1] + wheelSlip[2] + wheelSlip[3]) / 4;
        _groundFriction = (groundStaticFriction[0] + groundStaticFriction[1] + groundStaticFriction[2] + groundStaticFriction[3]) / 4;
    }

    private void GettingScriptableValues() {
        // var vehicleData = Resources.Load<VehicleData>("VehiclesConfig" + "/" + VehicleManager.id);
        VehicleData vehicleData = this.VehicleManager.vehicleData;
        this._maxSpeed = vehicleData.maxSpeed;
        driveWheels = vehicleData.driveWheels;
        this._torqueDevider = vehicleData.torqueDevider;
        originExtremumValue = vehicleData.originExtremumValue;
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