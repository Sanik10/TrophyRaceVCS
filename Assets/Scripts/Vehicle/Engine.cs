using System;
using UnityEngine;

public class Engine : MonoBehaviour {

    private VehicleManager _VehicleManager;
    private Transmission _Transmission;
    private VehicleDynamics _VehicleDynamics;
    private VehicleInputHandler _VehicleInputHandler;

    [Header("Двигатель")]
    [SerializeField]
    private AnimationCurve _powerCurve;
    private float _rpmVariableLimiter = 0;
    private float _load = 0;
    private float _additionRpm = 0;
    [SerializeField]
    private float _rpm = 0;
    [SerializeField]
    private float _power = 0;
    private float _kiloWatts = 0;
    private float _newtonMeters = 0;
    private float _idleRpm = 500;
    private float _medRpm = 0;
    private float _maxRpm = 0;
    private float _torque = 0;
    private float _additionOnNeutral = 0;
    private int _maxpower = 0;
    private int _maxPowerProcentAtIdleRpm = 0;
    private int _maxPowerProcentAtMaxRpm = 0;
    [SerializeField]
    private float _engineInertia = 0;
    private float _engineSmoothTime = 0;
    [SerializeField]
    private float _throttle = 0;
    private float _velocity = 0;

    public float rpm => this._rpm;
    public float power => this._power;
    public float kiloWatts => this._kiloWatts;
    public float newtonMeters => this._newtonMeters;
    public float idleRpm => this._idleRpm;
    public float medRpm => this._medRpm;
    public float maxRpm => this._maxRpm;
    public float torque => this._torque;
    public float additionOnNeutral => this._additionOnNeutral;
    public int maxpower => this._maxpower;
    public int maxPowerProcentAtIdleRpm => this._maxPowerProcentAtIdleRpm;
    public int maxPowerProcentAtMaxRpm => this._maxPowerProcentAtMaxRpm;
    public float engineInertia => this._engineInertia;
    public float engineSmoothTime => this._engineSmoothTime;
    public float throttle => this._throttle;
    public float velocity => this._velocity;
    public float loadProcent = 0;
    public float backTorque = 50f;



    private void Start() {
        this._VehicleManager = GetComponent<VehicleManager>();
        GetVehicleData();
        this._Transmission = this._VehicleManager.Transmission;
        this._VehicleInputHandler = this._VehicleManager.VehicleInputHandler;
        this._powerCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(this._idleRpm, this._maxpower - this._maxpower * this._maxPowerProcentAtIdleRpm / 100), new Keyframe(this._medRpm, this._maxpower), new Keyframe(this._maxRpm, this._maxpower - this._maxpower * this._maxPowerProcentAtMaxRpm / 100), new Keyframe(this._maxRpm+1, 0));
        this._powerCurve.preWrapMode = WrapMode.Clamp;
        this._rpm = this._idleRpm;
        this._rpmVariableLimiter = this._maxRpm;
        this._additionOnNeutral = this.maxRpm / 4;
        this._VehicleDynamics = this._VehicleManager.VehicleDynamics;
    }

    private void FixedUpdate() {
        if(this._VehicleInputHandler == null) {
            this._VehicleInputHandler = this._VehicleManager.VehicleInputHandler;
        }
        RunEngine();
    }

    private void RunEngine() {
        this._throttle = (this._rpm < this._idleRpm) ? this._throttle + 0.05f : Mathf.SmoothStep(this._throttle, (this._VehicleInputHandler.vertical > 0) ? 1 : 0, (this._engineSmoothTime * 150) * Time.fixedDeltaTime);

        if(this._throttle > 1f) {
            this._throttle = 1f;
        }

        this._rpmVariableLimiter = (this._VehicleInputHandler.handbrake && this._Transmission.currentGear == 1 && this._throttle > 0 && this._VehicleManager.PhysicsCalculation.kph < 5) ? this.maxRpm * 0.775f : this._maxRpm;

        RpmCalculating();

        this._load = (Mathf.Lerp(this._load, this._VehicleInputHandler.vertical - ((this._rpm - 1000) / this._maxRpm ), (this._engineSmoothTime * 10) * Time.fixedDeltaTime));
        this.loadProcent = _load * 100;
    }

    private void RpmCalculating() {
        float targetRPM;
        this._additionRpm = (!this._Transmission.neutralGear && !this._VehicleInputHandler.handbrake) ? 0 : ((this._rpm > this._rpmVariableLimiter) ? this._additionOnNeutral * -1f : this._additionOnNeutral);

        if(this._Transmission.neutralGear || this._VehicleInputHandler.handbrake || this._VehicleInputHandler.clutch == 0 || (this._VehicleInputHandler.vertical < 0 && this._rpm < 1000)) {
            // В режиме нейтрали или, при активном ручнике, или при выжатом сцеплении 
            targetRPM = Mathf.Lerp(this._rpm, this._idleRpm + this._additionRpm * this._throttle * this._Transmission.finalDrive * Mathf.Abs(this._Transmission.gears[1]), (this._engineSmoothTime * ((throttle < 0.2) ? 200 : 80)) * Time.fixedDeltaTime);
        } else {
            // Расчет оборотов с учетом влияния колес на двигатель
            float wheelRPMContribution = Mathf.Abs(this._VehicleDynamics.driveWheelsRpm) * this._VehicleDynamics.circumFerence * this._Transmission.finalDrive * Mathf.Abs(this._Transmission.currentGearRatio);

            targetRPM = Mathf.Lerp(this._rpm, wheelRPMContribution, (this._engineSmoothTime * 100) * Time.fixedDeltaTime);
        }

        // Сопротивление двигателя в случае, когда машина стоит или двигается накатом
        float engineResistance = this._throttle > 0.1 ? 300.0f : 0f; // Подстройте коэффициент сопротивления по вашим требованиям
        targetRPM -= engineResistance * Time.fixedDeltaTime;

        // Рассчитываем обороты двигателя
        this._rpm = Mathf.SmoothDamp(this._rpm, targetRPM, ref this._velocity, this._engineInertia);

        // Ограничиваем обороты
        this._rpm = Mathf.Clamp(this._rpm, 0, this._maxRpm);

        this._power = _powerCurve.Evaluate(this._rpm);

        this._torque = (!this._Transmission.neutralGear && !this._VehicleInputHandler.handbrake) ? (this._power * (this._Transmission.currentGearRatio * this._Transmission.finalDrive) * this._throttle * this._VehicleInputHandler.clutch) : 0;

        this._kiloWatts = (!this._Transmission.neutralGear && !this._VehicleInputHandler.handbrake) ? this._power / 1.3596f * this._throttle : 0;
        this._newtonMeters = (this._kiloWatts * 9549) / this._rpm;
    }

    private void GetVehicleData() {
        // var vehicleData = Resources.Load<VehicleData>("VehiclesConfig" + "/" + VehicleManager.id);
        VehicleData vehicleData = this._VehicleManager.vehicleData;
        this._idleRpm = vehicleData.idleRpm;
        this._medRpm = vehicleData.medRpm;
        this._maxRpm = vehicleData.maxRpm;
        // this._additionOnNeutral = vehicleData.additionRpmOnNeutral;

        this._maxpower = vehicleData.maxPower;
        this._maxPowerProcentAtIdleRpm = vehicleData.maxPowerProcentAtIdleRpm;
        this._maxPowerProcentAtMaxRpm = vehicleData.maxPowerProcentAtMaxRpm;
        this._engineInertia = vehicleData.engineInertia;
        this._engineSmoothTime = vehicleData.engineSmoothTime;
    }
}