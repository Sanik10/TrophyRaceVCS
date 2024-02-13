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
        // this.originalDrag = this._rgdbody.drag;
    }

    private void FixedUpdate() {
        SpeedCalculation();
        CalculateVelocity();
        // ApplySpringEffect();
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

        // // Применяем плавный эффект "пружины" к сопротивлению движению
        // float maxRpmEffect = Mathf.Lerp(maxRpmForSpringEffect, VehicleManager.Engine.maxRpm, Mathf.InverseLerp(0f, VehicleManager.Engine.maxRpm, VehicleManager.Engine.rpm));
        // float normalizedEffect = Mathf.InverseLerp(maxRpmForSpringEffect, maxRpmEffect, VehicleManager.Engine.rpm);
        // float springDrag = Mathf.Lerp(0f, springEffectStrength, normalizedEffect);

        // Учитываем оригинальное значение drag
        // float totalDrag = (VehicleManager.VehicleInputHandler.vertical <= 0 || VehicleManager.VehicleInputHandler.clutch == 0) ? originalDrag : springDrag;

        // this._rgdbody.drag = totalDrag;

        float linearSpeed = this._rgdbody.velocity.magnitude; // Линейная скорость
        float angularSpeed = this._rgdbody.angularVelocity.magnitude; // Угловая скорость

        this.turningRadius = linearSpeed / angularSpeed;
        this.recommendedSpeed = Mathf.Sqrt((9.81f * turningRadius * VehicleManager.WheelsSettings.groundFriction)) * 3.6f;
    }

    private void PreRaceModeHandler() {
        rgdbody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationY;
    }

    private void StartRaceHandler() {
        GameManager.StartRaceEvent -= StartRaceHandler;
        this._rgdbody = GetComponent<Rigidbody>();
        this.rgdbody.constraints = RigidbodyConstraints.None;
    }

    private void OnDrawGizmosSelected() {
        Gizmos.DrawWireSphere(centerOfMass, 0.25f);
    }
}

[System.Serializable] internal enum speedTypeEnum {
    Kph, Mph, Fps, Mps
}