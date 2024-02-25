using System;
using UnityEngine;

public class Engine : MonoBehaviour {

    private VehicleManager VehicleManager;
    private Transmission _Transmission;
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
        this.VehicleManager = GetComponent<VehicleManager>();
        GetVehicleData();
        this._Transmission = this.VehicleManager.Transmission;
        this._VehicleInputHandler = this.VehicleManager.VehicleInputHandler;
        this._powerCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(this._idleRpm, this._maxpower - this._maxpower * this._maxPowerProcentAtIdleRpm / 100), new Keyframe(this._medRpm, this._maxpower), new Keyframe(this._maxRpm, this._maxpower - this._maxpower * this._maxPowerProcentAtMaxRpm / 100), new Keyframe(this._maxRpm+1, 0));
        this._powerCurve.preWrapMode = WrapMode.Clamp;
        this._rpm = this._idleRpm;
        this._rpmVariableLimiter = this._maxRpm;
    }

    private void FixedUpdate() {
        if(this._VehicleInputHandler == null) {
            this._VehicleInputHandler = this.VehicleManager.VehicleInputHandler;
        }
        RunEngine();
    }

    private void RunEngine() {
        // this._throttle = (this._VehicleInputHandler.vertical > 0 && this._Transmission.currentGearRatio >= 0) ? this._VehicleInputHandler.vertical : (this._Transmission.currentGearRatio < 0) ? this._VehicleInputHandler.vertical : 0;
        // if(this._rpm < this._idleRpm) {
        //     this._throttle += 0.05f;
        // } else if(this._rpm > this._idleRpm) {
        // }
        this._throttle = (this._rpm < this._idleRpm) ? this._throttle + 0.05f : (this._VehicleInputHandler.vertical > 0) ? this._VehicleInputHandler.vertical : 0;

        if(this._throttle > 1f) {
            this._throttle = 1f;
        }
        this._rpmVariableLimiter = (this._VehicleInputHandler.handbrake && this._Transmission.currentGear == 1 && this._throttle > 0 && VehicleManager.PhysicsCalculation.Kph < 5) ? this._medRpm+200 : this._maxRpm;

        RpmCalculating();

        this._load = (Mathf.Lerp(this._load, this._VehicleInputHandler.vertical - ((this._rpm - 1000) / this._maxRpm ), (this._engineSmoothTime * 10) * Time.fixedDeltaTime));
        this.loadProcent = _load * 100;
    }

    /* private void RpmCalculating() {
        this._additionRpm = (!this._Transmission.neutralGear && !this._VehicleInputHandler.handbrake) ? 0 : ((this._rpm > this._rpmVariableLimiter) ? this._additionOnNeutral * -2 : this._additionOnNeutral);

        this._rpm = (!this._Transmission.neutralGear && !this._VehicleInputHandler.handbrake) ?
            (Mathf.SmoothDamp(this._rpm, (Mathf.Lerp(this._rpm, this._idleRpm + Mathf.Abs(VehicleManager.WheelsSettings.wheelsRPM) * this._Transmission.finalDrive * Mathf.Abs(this._Transmission.currentGearRatio), (this._engineSmoothTime * 100) * Time.fixedDeltaTime)), ref this._velocity, this._engineInertia))
            :
            (Mathf.SmoothDamp(this._rpm, (Mathf.Lerp(this._rpm, this._idleRpm + this._additionRpm * this._throttle * this._Transmission.finalDrive * Mathf.Abs(this._Transmission.gears[1]), (this._engineSmoothTime * 18) * Time.fixedDeltaTime)), ref this._velocity, this._engineInertia));

        // Расчет крутящего момента
        this._power = _powerCurve.Evaluate(this._rpm);
        this._torque = (!this._Transmission.neutralGear && !this._VehicleInputHandler.handbrake) ? (this._power * (this._Transmission.currentGearRatio * this._Transmission.finalDrive) * this._throttle * this._VehicleInputHandler.clutch) : 0;

        // Добавляем обратный момент при сбросе газа
        float backTorqueVar = this.backTorque; // Устанавливайте подходящее значение обратного момента
        float throttleReleaseFactor = Mathf.Pow(1.0f - _throttle, 2);
        float reverseTorque = backTorqueVar * throttleReleaseFactor;

        // Интегрируем обратный момент
        this._rpm += _torque / _engineInertia - reverseTorque;

        // Ограничиваем обороты
        this._rpm = Mathf.Clamp(_rpm, 0, this._maxRpm);

        this._kiloWatts = this._power / 1.3596f;
        this._newtonMeters = (this._kiloWatts * 9549) / this._rpm;
    } */
    
    private void RpmCalculating() {
        float targetRPM;
        this._additionRpm = (!this._Transmission.neutralGear && !this._VehicleInputHandler.handbrake) ? 0 : ((this._rpm > this._rpmVariableLimiter) ? this._additionOnNeutral * -5 : this._additionOnNeutral);


        if(this._Transmission.neutralGear || this._VehicleInputHandler.handbrake || this._VehicleInputHandler.clutch == 0 || (this._VehicleInputHandler.vertical < 0 && this._rpm < 1000)) {
            // В режиме нейтрали или, при активном ручнике, или при выжатом сцеплении 
            targetRPM = Mathf.Lerp(this._rpm, this._idleRpm + this._additionRpm * this._throttle * this._Transmission.finalDrive * Mathf.Abs(this._Transmission.gears[1]), (this._engineSmoothTime * 20) * Time.fixedDeltaTime);
        } else {
            // Расчет оборотов с учетом влияния колес на двигатель
            float wheelRPMContribution = Mathf.Abs(VehicleManager.VehicleDynamics.driveWheelsRpm) * this._Transmission.finalDrive * Mathf.Abs(this._Transmission.currentGearRatio);

            // Подстройте коэффициент, чтобы усилить влияние колес на обороты двигателя
            float wheelInfluenceFactor = 1.6f;

            targetRPM = Mathf.Lerp(this._rpm, wheelRPMContribution * wheelInfluenceFactor, (this._engineSmoothTime * 100) * Time.fixedDeltaTime);
        }

        // Сопротивление двигателя в случае, когда машина стоит или двигается накатом
        float engineResistance = 250.0f; // Подстройте коэффициент сопротивления по вашим требованиям
        targetRPM -= engineResistance * Time.fixedDeltaTime;

        // Рассчитываем обороты двигателя
        this._rpm = Mathf.SmoothDamp(this._rpm, targetRPM, ref this._velocity, this._engineInertia);

        // Ограничиваем обороты
        this._rpm = Mathf.Clamp(this._rpm, 0, this._maxRpm);

        this._power = _powerCurve.Evaluate(this._rpm);

        this._torque = (!this._Transmission.neutralGear && !this._VehicleInputHandler.handbrake) ? (this._power * (this._Transmission.currentGearRatio * this._Transmission.finalDrive) * this._throttle * this._VehicleInputHandler.clutch) : 0;

        this._kiloWatts = this._power / 1.3596f;
        this._newtonMeters = (this._kiloWatts * 9549) / this._rpm;
    }

    private void GetVehicleData() {
        // var vehicleData = Resources.Load<VehicleData>("VehiclesConfig" + "/" + VehicleManager.id);
        VehicleData vehicleData = this.VehicleManager.vehicleData;
        this._idleRpm = vehicleData.idleRpm;
        this._medRpm = vehicleData.medRpm;
        this._maxRpm = vehicleData.maxRpm;
        this._additionOnNeutral = vehicleData.additionRpmOnNeutral;

        this._maxpower = vehicleData.maxPower;
        this._maxPowerProcentAtIdleRpm = vehicleData.maxPowerProcentAtIdleRpm;
        this._maxPowerProcentAtMaxRpm = vehicleData.maxPowerProcentAtMaxRpm;
        this._engineInertia = vehicleData.engineInertia;
        this._engineSmoothTime = vehicleData.engineSmoothTime;
    }
}