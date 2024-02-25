using System;
using UnityEngine;

public class Transmission : MonoBehaviour {

    private VehicleManager _VehicleManager;
    private Engine Engine;
    private VehicleInputHandler _VehicleInputHandler;
    private VehicleSFX VSFX;
    
    [SerializeField]
    private transmissionType _transmission;
    [SerializeField]
    private float[] _gears;
    private float _firstGear = 0;
    private float _finalDrive = 0;
    private float _shiftTime = 0;
    private float _currentGearRatio = 0;
    private float _gearsMultiplier = 0;
    private float _reverseGear = 0;
    private float _gearChangeRate = 0;
    private float _lastGearRatio = 0;
    private int _currentGear = 1;
    private int _lastGearNumber = 0;
    private int _frontGearsQuantity = 0;
    private int _maxSpeed = 0;
    private int _rpmUpShift = 0;
    private int _rpmDownShift = 0;
    private bool _neutralGear = false;
    private bool _gearUp = false;
    private bool _gearDown = false;

    public bool clutchButtonNeed = false;
    public float[] gears => this._gears;
    public float finalDrive => this._finalDrive;
    public float currentGearRatio => this._currentGearRatio;
    public int currentGear => this._currentGear;
    public bool neutralGear => this._neutralGear;
    public int rpmUpShift => this._rpmUpShift;
    public int rpmDownShift => this._rpmDownShift;

    private void Awake() {
        VehicleDynamics.VehicleDynamicsInitializedEvent += SetUpGears;
    }

    private void Start() {
        this._VehicleManager = GetComponent<VehicleManager>();
        this.Engine = this._VehicleManager.Engine;
        this._VehicleInputHandler = this._VehicleManager.VehicleInputHandler;
        this.VSFX = this._VehicleManager.VehicleSFX;

        if(this._VehicleManager.aiVehicle) {
            this._transmission = transmissionType.Automatic;
        }

        GetVehicleData();
        VehicleInputHandler.ChangeGearUpEvent += HandleChangeGearUpEvent;
        VehicleInputHandler.ChangeGearDownEvent += HandleChangeGearDownEvent;
        VehicleInputHandler.ReverseGearEvent += HandleReverseGearEvent;
    }

    private void SetUpGears(VehicleDynamics vd) {
        VehicleDynamics currentVD = GetComponent<VehicleDynamics>();
        if(vd == currentVD) {
            VehicleDynamics.VehicleDynamicsInitializedEvent -= SetUpGears;

            this._lastGearRatio = (Engine.maxRpm-1500) * currentVD.circumFerence / (20.2f * finalDrive * this._maxSpeed); //4167   4050/200
            this._gears = new float[this._frontGearsQuantity + 1];
            this._gears[0] = 0;
            this._gears[1] = this._firstGear;
            this._reverseGear = this._gears[1] * -1.5f;
            this._gearsMultiplier = Mathf.Pow((this._lastGearRatio / this._firstGear), (1.0f / this._frontGearsQuantity));
            for (int i = 2; i < this._frontGearsQuantity + 1; i++) {
                this._gears[i] = this._gears[i - 1] * this._gearsMultiplier;
            }
            this._currentGearRatio = this._gears[this._currentGear];
            
            if(this._VehicleManager.aiVehicle) {
                this._transmission = transmissionType.Automatic;
            }
        }
    }

    private void FixedUpdate() {
    }

    private void GearBox() {
        if(this._VehicleManager.aiVehicle) {
            this._transmission = transmissionType.Automatic;
        }

        if(this._transmission == transmissionType.Automatic) {

            if(this._VehicleInputHandler.handbrake == true){return;}
            if(this._currentGear < this._gears.Length-1 && Time.time >= this._gearChangeRate && ((Engine.rpm > this._rpmUpShift && !this._VehicleInputHandler.reverseGear) || (this._VehicleInputHandler.gearUp))) {ShiftUp();} //Engine.velocity < 20 Engine.loadProcent < 15
            if(Engine.rpm < this._rpmDownShift && this._currentGear > 1 && Time.time >= this._gearChangeRate) {ShiftDown();}
            this._VehicleInputHandler.clutch = (this._neutralGear == true) ? 0 : 1;

        } else if(this._transmission == transmissionType.Sequental) {

            if(this._currentGear < this._gears.Length-1 && this._gearUp && Time.time >= this._gearChangeRate) {ShiftUp();}
            if(this._currentGear > 0 && this._gearDown && Time.time >= this._gearChangeRate) {ShiftDown();}
            this._VehicleInputHandler.clutch = (this._neutralGear == true) ? 0 : 1;

        } else if(this._transmission == transmissionType.Manual) {

            this.clutchButtonNeed = true;
            if(this._VehicleInputHandler.clutch == 1) {return;}
            if(this._currentGear < this._gears.Length-1 && this._gearUp && Time.time >= this._gearChangeRate) {ShiftUp();}
            if(this._currentGear > 0 && this._gearDown && Time.time >= this._gearChangeRate) {ShiftDown();}

        }

        #region Change gear script
        void ShiftUp() {
            if(this._VehicleInputHandler.reverseGear) {
                this._VehicleInputHandler.reverseGear = false;
            } else {
                this._currentGear++;
            }
            this._lastGearNumber = this._currentGear;
            VSFX.gearUpAudio();
            if (Engine.rpm > 5500){
                VSFX.backFireAudio();
            }
            // if (VSFX.turboSound && Engine.rpm > 4500 && this._VehicleInputHandler.vertical > 0) {
            //     VSFX.changeGearTurbo();
            // }
            this._currentGear = 0;
            this._currentGearRatio = 0;
            Invoke("SetNeutralGear", this._shiftTime);
            this._gearChangeRate = Time.time + 1f;
        }

        void ShiftDown() {
            if(this._currentGearRatio < 0) {
                return;
            }
            this._gearChangeRate = Time.time + 1f;
            VSFX.gearDownAudio();
            if(!this._VehicleInputHandler.reverseGear){
                this._currentGear--;
            }
            this._lastGearNumber = this._currentGear;
            this._currentGear = 0;
            this._currentGearRatio = 0;
            Invoke("SetNeutralGear", this._shiftTime);
        }
        #endregion
    }

    private void HandleChangeGearUpEvent(VehicleInputHandler IH, bool value) {
        if(IH == this._VehicleManager.VehicleInputHandler) {
            this._gearUp = value;
            GearBox();
        }
    }

    private void HandleChangeGearDownEvent(VehicleInputHandler IH, bool value) {
        if(IH == this._VehicleManager.VehicleInputHandler) {
            this._gearDown = value;
            GearBox();
        }
    }

    private void HandleReverseGearEvent(VehicleInputHandler IH) {
        if(IH == this._VehicleManager.VehicleInputHandler) {
            this._currentGear = 1;
            this._neutralGear = false;
            this._currentGearRatio = this._reverseGear;
        }
    }

    private void SetNeutralGear() {
        this._currentGear = this._lastGearNumber;
        this._neutralGear = (this._currentGear == 0) ? true : false;
        this._currentGearRatio = this._gears[this._currentGear];
    }

    private void OnDestroy() {
        VehicleInputHandler.ChangeGearUpEvent -= HandleChangeGearUpEvent;
        VehicleInputHandler.ChangeGearDownEvent -= HandleChangeGearDownEvent;
        VehicleInputHandler.ReverseGearEvent -= HandleReverseGearEvent;
    }

    private void GetVehicleData() {
        // var vehicleData = Resources.Load<VehicleData>("VehiclesConfig" + "/" + VehicleManager.id);
        VehicleData vehicleData = this._VehicleManager.vehicleData;
        this._transmission = vehicleData.transmission;
        this._frontGearsQuantity = vehicleData.frontGearsQuantity;
        this._maxSpeed = vehicleData.maxSpeed;
        this._firstGear = vehicleData.firstGear;
        this._finalDrive = vehicleData.finalDrive;
        this._shiftTime = vehicleData.shiftTime;
        this._rpmUpShift = vehicleData.rpmUpShift;
        this._rpmDownShift = vehicleData.rpmDownShift;
    }
}