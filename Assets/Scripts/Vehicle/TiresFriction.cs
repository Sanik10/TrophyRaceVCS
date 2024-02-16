using UnityEngine;
using Random = UnityEngine.Random;

public class TiresFriction : MonoBehaviour {

    private VehicleManager VehicleManager;
    private VehicleType _vehicleType;

    [SerializeField]
    private float _baseFriction = 1.0f;
    [SerializeField]
    private float _tireIntegrity;
    [SerializeField]
    private float _massMultiplier;
    [SerializeField]
    private float[] _typeMultiplier;
    [SerializeField]
    private float _totalGroundFriction;
    [SerializeField]
    private WheelCollider[] _wheelsColliders;
    [SerializeField]
    private SurfaceType[] _surfaceType;
    [SerializeField]
    private float[] _wheelSlip;

    [Header("Передняя ось авто")]
    [SerializeField]
    private FrictionSettings frontForwardFriction;
    [SerializeField]
    private FrictionSettings frontSidewaysFriction;

    [Header("Задняя ось авто")]
    [SerializeField]
    private FrictionSettings rearForwardFriction;
    [SerializeField]
    private FrictionSettings rearSidewaysFriction;

    private float[] _originExtremumValue;

    public float baseFriction => this._baseFriction;
    public float wearMultiplier => this._tireIntegrity;
    public float massMultiplier => this._massMultiplier;
    public float totalGroundFriction => this._totalGroundFriction;
    public float[] typeMultiplier => this._typeMultiplier;
    public WheelCollider[] wheelsColliders => this._wheelsColliders;
    public SurfaceType[] surfaceType => this._surfaceType;
    public float[] wheelSlip => this._wheelSlip;

    private void Start() {
        this.VehicleManager = GetComponent<VehicleManager>();
        InitializeFrictionFactors();
    }

    private void FixedUpdate() {
        DetermineSurfaceType();
        ApplyInclineAngle();
        ApplyFriction();
    }

    private void DetermineSurfaceType() {
        for (int i = 0; i < _wheelsColliders.Length; i++) {
            WheelCollider wheelCollider = _wheelsColliders[i];

            // Создаем луч, направленный вниз от колеса
            Ray ray = new Ray(wheelCollider.transform.position, -wheelCollider.transform.up);
            RaycastHit hit;

            // Проверяем столкновение луча с поверхностью
            if (Physics.Raycast(ray, out hit, 1.0f)) {
                Collider collider = hit.collider;

                // Получаем физический материал коллайдера
                PhysicMaterial currentMaterial = collider.sharedMaterial;

                // Присваиваем typeMultiplier в зависимости от типа поверхности
                if (currentMaterial != null) {
                    string materialName = currentMaterial.name.ToLower();

                    if (materialName.Contains("asphalt")) {
                        _typeMultiplier[i] = (materialName.Contains("wet")) ? 0.65f : 0.85f;
                        _surfaceType[i] = (materialName.Contains("wet")) ? SurfaceType.AsphaltWet : SurfaceType.Asphalt;
                    } else if (materialName.Contains("concrete")) {
                        _typeMultiplier[i] = (materialName.Contains("wet")) ? 0.7f : 0.8f;
                        _surfaceType[i] = (materialName.Contains("wet")) ? SurfaceType.ConcreteWet : SurfaceType.Concrete;
                    } else if (materialName.Contains("sand")) {
                        _typeMultiplier[i] = (materialName.Contains("wet")) ? 0.55f : 0.45f;
                        _surfaceType[i] = (materialName.Contains("wet")) ? SurfaceType.SandWet : SurfaceType.Sand;
                    } else if (materialName.Contains("dirt")) {
                        _typeMultiplier[i] = (materialName.Contains("wet")) ? 0.45f : 0.55f;
                        _surfaceType[i] = (materialName.Contains("wet")) ? SurfaceType.DirtWet : SurfaceType.Dirt;
                    } else if (materialName.Contains("snow")) {
                        _typeMultiplier[i] = (materialName.Contains("icy")) ? 0.2f : 0.3f;
                        _surfaceType[i] = (materialName.Contains("icy")) ? SurfaceType.SnowIcy : SurfaceType.Snow;
                    } else if (materialName.Contains("ice")) {
                        _typeMultiplier[i] = 0.18f;
                        _surfaceType[i] = SurfaceType.Ice;
                    }
                }
            }
        }
    }

    private void ApplyInclineAngle() {
        for (int i = 0; i < _wheelsColliders.Length; i++) {
            WheelCollider wheelCollider = _wheelsColliders[i];

            float wheelInclineAngle = Vector3.Angle(wheelCollider.transform.up, Vector3.up);
            float inclineEffect = Mathf.Clamp01(1 - (wheelInclineAngle / 90f));

            // Применяем эффект наклона к фактору сцепления
            _typeMultiplier[i] *= inclineEffect;
        }
    }

    private void ApplyFriction() {
        WheelHit hit;
        float frictionSum = 0f;
        for(int i = 0; i < _wheelsColliders.Length; i++) {
            WheelCollider wheelCollider = _wheelsColliders[i];

            WheelFrictionCurve forwardFriction = wheelCollider.forwardFriction;
            WheelFrictionCurve sidewaysFriction = wheelCollider.sidewaysFriction;

            forwardFriction.stiffness = this._typeMultiplier[i] * (0.25f * Mathf.Sqrt(this._tireIntegrity) + 0.75f);
            sidewaysFriction.stiffness = this._typeMultiplier[i] * (0.25f * Mathf.Sqrt(this._tireIntegrity) + 0.75f);
            sidewaysFriction.extremumValue = ((i > 1) && this.VehicleManager.VehicleInputHandler.handbrake && (this.VehicleManager.VehicleInputHandler.horizontal > 0.5f || this.VehicleManager.VehicleInputHandler.horizontal < -0.5f)) ? 0.2f : this._originExtremumValue[i];

            wheelCollider.forwardFriction = forwardFriction;
            wheelCollider.sidewaysFriction = sidewaysFriction;
            if(this._wheelsColliders[i].GetGroundHit(out hit)) {
                this._wheelSlip[i] = Mathf.Abs(hit.forwardSlip) + Mathf.Abs(hit.sidewaysSlip);
            }
            frictionSum += this._typeMultiplier[i];
        }
        this._totalGroundFriction = frictionSum / this._wheelsColliders.Length;
    }

    private void InitializeFrictionFactors() {
        VehicleData vehicleData = this.VehicleManager.vehicleData;

        this._vehicleType = vehicleData.type;
        this._baseFriction = (this._vehicleType == VehicleType.TrophyTruck) ? 0.775f : (this._vehicleType == VehicleType.Truck) ? 0.8f : (this._vehicleType == VehicleType.Prerunner) ? 0.85f : (this._vehicleType == VehicleType.Buggy) ? 0.9f : 0.85f;
        
        // Учет массы для настройки сцепления
        this._massMultiplier = 1 - vehicleData.normalizedMass;
        this._tireIntegrity = vehicleData.tireIntegrity;
        
        Transform carCollidersTransform = gameObject.transform.Find("carColliders");
        
        if(carCollidersTransform != null) {
            int wheelCount = carCollidersTransform.childCount;
            this._wheelsColliders = new WheelCollider[wheelCount];
            this._typeMultiplier = new float[wheelCount];
            this._surfaceType = new SurfaceType[wheelCount];
            this._originExtremumValue = new float[wheelCount];
            this._wheelSlip = new float[wheelCount];
            
            for(int q = 0; q < wheelCount; q++) {
                this._wheelsColliders[q] = carCollidersTransform.GetChild(q).GetComponent<WheelCollider>();

                this._wheelsColliders[q].forwardFriction = CustomFrictionCurve(q < 2 ? frontForwardFriction : rearForwardFriction);
                this._wheelsColliders[q].sidewaysFriction = CustomFrictionCurve(q < 2 ? frontSidewaysFriction : rearSidewaysFriction);

                this._originExtremumValue[q] = this._wheelsColliders[q].sidewaysFriction.extremumValue;
            }
        }
    }

    private WheelFrictionCurve CustomFrictionCurve(FrictionSettings frictionSettings) {
        WheelFrictionCurve frictionCurve = new WheelFrictionCurve();

        frictionCurve.extremumSlip = frictionSettings.extremumSlip[1] - this._massMultiplier * (frictionSettings.extremumSlip[1] - frictionSettings.extremumSlip[0]);
        frictionCurve.extremumValue = frictionSettings.extremumValue[1] - this._massMultiplier * (frictionSettings.extremumValue[1] - frictionSettings.extremumValue[0]);
        frictionCurve.asymptoteSlip = frictionSettings.asymptoteSlip[1] - this._massMultiplier * (frictionSettings.asymptoteSlip[1] - frictionSettings.asymptoteSlip[0]);
        frictionCurve.asymptoteValue = frictionSettings.asymptoteValue[1] - this._massMultiplier * (frictionSettings.asymptoteValue[1] - frictionSettings.asymptoteValue[0]);

        return frictionCurve;
    }
}

[System.Serializable]
public enum SurfaceType {
    Asphalt,
    AsphaltWet,
    Concrete,
    ConcreteWet,
    Sand,
    SandWet,
    Dirt,
    DirtWet,
    Snow,
    SnowIcy,
    Ice
}

[System.Serializable]
public class FrictionSettings {
    public float[] extremumSlip = new float[2];
    public float[] extremumValue = new float[2];
    public float[] asymptoteSlip = new float[2];
    public float[] asymptoteValue = new float[2];
}