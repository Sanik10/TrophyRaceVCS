using UnityEngine;
using TrophyRace.Architecture;

public class PhysicsCalculation : MonoBehaviour {

    private VehicleManager _VehicleManager;
    private VehicleDynamics _VehicleDynamics;
    private Rigidbody _rgdbody;

    [SerializeField]
    private Vector3 _centerOfMass;
    [SerializeField]
    private speedTypeEnum speedType;
    [SerializeField]
    private float _turningRadius;
    [SerializeField]
    private float _recommendedSpeed;
    [SerializeField]
    private float _brakeDistance;
    public float accelerationMagnitude;
    private float _speed;
    private float _kph;
    private float _kphByWheels;
    [SerializeField]
    private bool _speedByVelocity = true;
    [SerializeField]
    private float _downForceValue = 25;
    [SerializeField]
    private float _angularDragVar = 25;
    private float _mass;
    private float _dragAmount = 0.016f;
    private float _mps;
    
    public Rigidbody rgdbody => this._rgdbody;

    public float kph => this._kph;
    public float speed => this._speed;
    public float recommendedSpeed => this._recommendedSpeed;
    public float brakeDistance => this._brakeDistance;

    private void OnEnable() {
        GameManager.SetVehiclesInPreRaceModeEvent += PreRaceModeHandler;
        GameManager.StartRaceEvent += StartRaceHandler;
    }

    private void OnDestroy() {
        GameManager.SetVehiclesInPreRaceModeEvent -= PreRaceModeHandler;
    }

    private void Start() {
        this._rgdbody = GetComponent<Rigidbody>();
        this._rgdbody.drag = this._dragAmount;
        this._mass = this._rgdbody.mass;
        this._rgdbody.centerOfMass = this._centerOfMass;
        this._VehicleManager = GetComponent<VehicleManager>();
        this._VehicleDynamics = GetComponent<VehicleDynamics>();
    }

    private void FixedUpdate() {
        SpeedCalculation();
        CalculateVelocity();
    }

    private void SpeedCalculation() {
        this._mps = this._rgdbody.velocity.magnitude;
        this._kph = this._mps * 3.6f;
        this._kphByWheels = (this._VehicleDynamics.circumFerence * this._VehicleDynamics.driveWheelsRpm) * 0.06f;

        this._speed = (this._speedByVelocity) ? 
        ((speedType == speedTypeEnum.Kph) ? this._kph : (speedType == speedTypeEnum.Mph) ? this._mps * 2.237f : (speedType == speedTypeEnum.Fps) ?  this._mps * 3.281f : this._mps) 
        : 
        ((speedType == speedTypeEnum.Kph) ? this._kphByWheels : (speedType == speedTypeEnum.Mph) ? this._kphByWheels / 1.609f : (speedType == speedTypeEnum.Fps) ? this._kphByWheels / 1.097f : this._kphByWheels / 3.6f);
    }

    private void CalculateVelocity() {
        this._rgdbody.angularDrag = this._mps / this._angularDragVar;

        this._rgdbody.AddForce(-transform.up * this._downForceValue * this._kph);

        // Учитываем оригинальное значение drag
        float totalDrag = (this._VehicleManager.VehicleInputHandler.vertical <= 0 || this._VehicleManager.VehicleInputHandler.clutch == 0) ? this._dragAmount : 0;

        this._rgdbody.drag = totalDrag;

        float linearSpeed = this._rgdbody.velocity.magnitude; // Линейная скорость
        float angularSpeed = this._rgdbody.angularVelocity.magnitude; // Угловая скорость

        this._turningRadius = linearSpeed / angularSpeed;
        this._recommendedSpeed = Mathf.Sqrt((9.81f * this._turningRadius * this._VehicleManager.TiresFriction.totalGroundFriction)) * 3.6f;
        float slopeAngle = Mathf.Rad2Deg * Mathf.Atan2(transform.forward.y, Mathf.Sqrt(transform.forward.x * transform.forward.x + transform.forward.z * transform.forward.z));
        float brakePower = this._VehicleDynamics.totalBrakePower;
        //? (this._brakingPower * (1 - this._brakingDistribution))
        //: this._brakingPower * this._brakingDistribution
        this._brakeDistance = (this._mps * this._mps) / (9.81f * (brakePower / this._rgdbody.mass) * this._VehicleManager.TiresFriction.totalGroundFriction);
    }

    private void PreRaceModeHandler() {
        this._rgdbody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationY;
    }

    private void StartRaceHandler() {
        GameManager.StartRaceEvent -= StartRaceHandler;
        if(this._rgdbody == null) {
            this._rgdbody = GetComponent<Rigidbody>();
        }
        this._rgdbody.constraints = RigidbodyConstraints.None;
    }

    private void OnDrawGizmosSelected() {
        Gizmos.DrawWireSphere(this._centerOfMass, 0.3f);
    }
}

[System.Serializable] internal enum speedTypeEnum {
    Kph, Mph, Fps, Mps
}