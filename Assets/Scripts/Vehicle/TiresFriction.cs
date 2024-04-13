using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using NWH.Common.Vehicles;

public class TiresFriction : MonoBehaviour {

    private VehicleManager VehicleManager;
    private VehicleType _vehicleType;

    [SerializeField]
    private LayerMask _raycastLayerMask;

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
    private WheelUAPI[] _wheelsControllers;
    [SerializeField] 
    private SurfaceType[] _surfaceType;
    [SerializeField]
    private float[] _springLength;
    [SerializeField]
    private float[] _wheelSlip;
    [SerializeField]
    private float[] _rayDistance;

    private float[] _originExtremumValue;
    Dictionary<string, SurfaceType> materialSurfaceMap = new Dictionary<string, SurfaceType>
    {
        {"asphalt", SurfaceType.Asphalt},
        {"concrete", SurfaceType.Concrete},
        {"sand", SurfaceType.Sand},
        {"dirt", SurfaceType.Dirt},
        {"snow", SurfaceType.Snow},
        {"ice", SurfaceType.Ice},
        {"ifriction", SurfaceType.IFriction}
    };
    Dictionary<string, SurfaceType> wetMaterialSurfaceMap = new Dictionary<string, SurfaceType>
    {
        {"asphaltwet", SurfaceType.AsphaltWet},
        {"concretewet", SurfaceType.ConcreteWet},
        {"sandwet", SurfaceType.SandWet},
        {"dirtwet", SurfaceType.DirtWet},
        {"snowwet", SurfaceType.SnowIcy},
        // "ice" не нуждается в отдельном варианте, так как он всегда "лед"
    };

    public float baseFriction => this._baseFriction;
    public float wearMultiplier => this._tireIntegrity;
    public float massMultiplier => this._massMultiplier;
    public float totalGroundFriction => this._totalGroundFriction;
    public float[] typeMultiplier => this._typeMultiplier;
    public WheelUAPI[] wheelsControllers => this._wheelsControllers;
    public SurfaceType[] surfaceType => this._surfaceType;
    public float[] wheelSlip => this._wheelSlip;

    private void Start() {
        this.VehicleManager = GetComponent<VehicleManager>();
        InitializeValues();
    }

    private void FixedUpdate() {
        DetermineSurfaceType();
        SetFrictionPreset();
    }

    private void DetermineSurfaceType() {
        for (int wheelIndex = 0; wheelIndex < this._wheelsControllers.Length; wheelIndex++) {

            RaycastHit hit;
            Vector3 raycastOrigin = this._wheelsControllers[wheelIndex].transform.position;

            this._springLength[wheelIndex] = this._wheelsControllers[wheelIndex].SpringLength;
            this._rayDistance[wheelIndex] = this._wheelsControllers[wheelIndex].SpringLength + this._wheelsControllers[wheelIndex].Radius + 0.1f;

            Debug.DrawRay(raycastOrigin, -this._wheelsControllers[wheelIndex].transform.up * this._rayDistance[wheelIndex], Color.red, 0.1f);
            if(Physics.Raycast(raycastOrigin, -this._wheelsControllers[wheelIndex].transform.up, out hit, this._rayDistance[wheelIndex])) {
                PhysicMaterial currentMaterial = hit.collider.sharedMaterial;

                this._surfaceType[wheelIndex] = (currentMaterial != null) ? DetermineSurfaceType(currentMaterial) : SurfaceType.Undefined;
            }
        }
    }

    private SurfaceType DetermineSurfaceType(PhysicMaterial material) {
        string materialName = material?.name.ToLower();
        if(materialName == null) {
            return SurfaceType.Undefined;
        }

        SurfaceType surfaceType = (materialSurfaceMap.TryGetValue(materialName, out SurfaceType baseSurfaceType)) ? baseSurfaceType : (materialName.Contains("wet") && wetMaterialSurfaceMap.TryGetValue(materialName, out SurfaceType wetSurfaceType)) ? wetSurfaceType : SurfaceType.Undefined;

        return surfaceType;
    }

    private void SetFrictionPreset() {
        for(int wheelIndex = 0; wheelIndex < this._wheelsControllers.Length; wheelIndex++) {
            if(this._surfaceType[wheelIndex] != SurfaceType.Undefined) {
                FrictionPreset frictionPreset = FrictionPresetsCache.Instance.GetFrictionPreset(this._surfaceType[wheelIndex].ToString());
                if(frictionPreset != null) {
                    this._wheelsControllers[wheelIndex].FrictionPreset = frictionPreset;
                }
            }
        }
    }

    private void InitializeValues() {
        Transform carCollidersTransform = gameObject.transform.Find("carColliders");
        if(carCollidersTransform != null) {
            int wheelCount = carCollidersTransform.childCount;
            this._wheelsControllers = new WheelUAPI[wheelCount];
            this._surfaceType = new SurfaceType[wheelCount];
            this._springLength = new float[wheelCount];
            this._rayDistance = new float[wheelCount];
            
            for(int q = 0; q < wheelCount; q++) {
                this._wheelsControllers[q] = carCollidersTransform.GetChild(q).GetComponent<WheelUAPI>();
            }
        }
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
    Ice,
    Undefined,
    IFriction
}