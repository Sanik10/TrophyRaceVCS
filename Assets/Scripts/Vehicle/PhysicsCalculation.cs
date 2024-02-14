using UnityEngine;
using TrophyRace.Architecture;

public class PhysicsCalculation : MonoBehaviour {

    private VehicleManager VehicleManager;
    private WheelsSettings WS;
    private Rigidbody _rgdbody;
    public Rigidbody rgdbody => this._rgdbody;
    [SerializeField]
    private speedTypeEnum speedType;
    public float turningRadius;
    public float recommendedSpeed;
    public float speed;
    public float Kph;
    public float KphByWheels;
    public Vector3 centerOfMass;
    public Vector3 centerOfMassAuto;
    public bool speedByVelocity = true;
    public float DownForceValue = 25;
    public float angularDragVar = 25;
    public float mass;
    public float dragAmount = 0.032f;
    private float Mps;

    private float maxRpmForSpringEffect = 6000f;
    private float springEffectStrength = 0.08f;
    public float originalDrag = 0.032f;

    private void OnEnable() {
        GameManager.SetVehiclesInPreRaceModeEvent += PreRaceModeHandler;
        GameManager.StartRaceEvent += StartRaceHandler;
    }

    private void OnDestroy() {
        GameManager.SetVehiclesInPreRaceModeEvent -= PreRaceModeHandler;
    }

    private void Start() {
        this._rgdbody = GetComponent<Rigidbody>();
        this._rgdbody.drag = dragAmount;
        this.mass = this._rgdbody.mass;
        this._rgdbody.centerOfMass = centerOfMass;
        this.VehicleManager = GetComponent<VehicleManager>();
        this.maxRpmForSpringEffect = this.VehicleManager.Engine.maxRpm * 0.7f;
    }

    private void FixedUpdate() {
        SpeedCalculation();
        CalculateVelocity();
    }

    private void SpeedCalculation() {
        Mps = this._rgdbody.velocity.magnitude;
        Kph = Mps * 3.6f;
        KphByWheels = (VehicleManager.WheelsSettings.circumFerence * VehicleManager.WheelsSettings.wheelsRPM) * 0.06f;

        speed = (speedByVelocity) ? 
        ((speedType == speedTypeEnum.Kph) ? Kph : (speedType == speedTypeEnum.Mph) ? Mps * 2.237f : (speedType == speedTypeEnum.Fps) ?  Mps * 3.281f : Mps) 
        : 
        ((speedType == speedTypeEnum.Kph) ? KphByWheels : (speedType == speedTypeEnum.Mph) ? KphByWheels / 1.609f : (speedType == speedTypeEnum.Fps) ? KphByWheels / 1.097f : KphByWheels / 3.6f);
    }

    private void CalculateVelocity() {
        this._rgdbody.angularDrag = Mps / angularDragVar;

        this._rgdbody.AddForce(-transform.up * DownForceValue * Kph);

        // Учитываем оригинальное значение drag
        float totalDrag = (VehicleManager.VehicleInputHandler.vertical <= 0 || VehicleManager.VehicleInputHandler.clutch == 0) ? 0.016f : 0;

        this._rgdbody.drag = totalDrag;

        float linearSpeed = this._rgdbody.velocity.magnitude; // Линейная скорость
        float angularSpeed = this._rgdbody.angularVelocity.magnitude; // Угловая скорость

        this.turningRadius = linearSpeed / angularSpeed;
        this.recommendedSpeed = Mathf.Sqrt((9.81f * turningRadius * VehicleManager.WheelsSettings.groundFriction)) * 3.6f;
        this.centerOfMassAuto = this._rgdbody.centerOfMass;
    }

    private void PreRaceModeHandler() {
        _rgdbody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationY;
    }

    private void StartRaceHandler() {
        GameManager.StartRaceEvent -= StartRaceHandler;
        this._rgdbody = GetComponent<Rigidbody>();
        this._rgdbody.constraints = RigidbodyConstraints.None;
    }

    private void OnDrawGizmosSelected() {
        Gizmos.DrawWireSphere(centerOfMass, 0.25f);
    }
}

[System.Serializable] internal enum speedTypeEnum {
    Kph, Mph, Fps, Mps
}