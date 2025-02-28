using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class VehicleVFX : MonoBehaviour {
    
    private VehicleManager VehicleManager;
    private PhysicsCalculation PC;
    private VehicleDynamics VehicleDynamics;
    private TiresFriction TF;

    public bool hasEffects;
    public bool smokeFlag;

    [Header("Кузов")]
    #region Basket color presets
    public int[] colorsCount;
    [SerializeField]
    private List<ColoredItem> _colored = new List<ColoredItem>();

    public List<ColoredItem> colored => this._colored;

    private bool _colorCustomization = false;
    #endregion

    [Header("Шины")]
    public bool hasTrails, hasSmokes;
    [SerializeField]
    private surfaceType[] currentSurface;
    public TrailRenderer[] skidmarksDrifting;
    public ParticleSystem[] smoke;
    public TrailRenderer[] skidmarksOffroad;
    public ParticleSystem[] sandDust;

    [Header("Кулеры")]
    public bool hasFan = false;
    public Transform[] Fan;
    const float FanSpeed = 7;

    [Header("Фары")]
    public bool hasHeadLights = false;
    public GameObject dippedBeamsON;
    public GameObject highBeamsON;
    public bool hasTailLighs = false;
    public GameObject tailLightsON;
    public bool hasReverseLighs = false;
    public GameObject reverseLightsON;

    void Start() {
        findValues();
        GetVehicleData();
        if(VehicleManager.aiVehicle && _colorCustomization) {
            ChangeColor(0, Random.Range(0, this._colored[0].material.Length-1));
            ChangeColor(1, Random.Range(0, this._colored[1].material.Length-1));
        }
    }

    private void findValues() {
        VehicleManager = GetComponent<VehicleManager>();
        PC = VehicleManager.PhysicsCalculation;
        VehicleDynamics = VehicleManager.VehicleDynamics;
        TF = VehicleManager.TiresFriction;
        currentSurface = new surfaceType[4];
    }

    void FixedUpdate() {
        if(hasFan) {
            RotationFan();
        }

        lights();
        

        // if(hasEffects) {
        //     // GroundCheck();
        //     // driftVFX();
        //     /// offroadVFX();
        // }
    }
/*
    private void driftVFX() {
        for(int i = 0; i < VehicleDynamics.wheelColliders.Length; i++) {
            var smokeMain = smoke[i].main;
            sandDust[i].Stop();
            skidmarksOffroad[i].emitting = false;
            if(currentSurface[i] != surfaceType.Grass || currentSurface[i] != surfaceType.Sand) {
                if(TF.wheelSlip[i] > 0.7f && TF.wheelsColliders[i].isGrounded && hasSmokes) {
                    smokeMain.simulationSpeed = (VehicleDynamics.wheelColliders[i].rpm > 1000) ? 8 : (VehicleDynamics.wheelColliders[i].rpm > 600) ? 4 : 2;
                    smokeMain.startSpeed = (PC.kph > 0) ? 2 : -2;
                    smoke[i].Play();
                } else {
                    smoke[i].Stop();
                }

                if(i < 2 && TF.wheelsColliders[i].isGrounded) {
                    skidmarksDrifting[i].emitting = (TF.wheelSlip[i] > 0.7f) ? (hasTrails ? true : false) : false;
                } else if((TF.wheelSlip[2] > 0.7f || TF.wheelSlip[3] > 0.7f) && hasSmokes) {
                    skidmarksDrifting[2].emitting = hasTrails ? true : false;
                    smoke[2].Play();
                    skidmarksDrifting[3].emitting = hasTrails ? true : false;
                    smoke[3].Play();
                } else {
                    skidmarksDrifting[i].emitting = false;
                }
            }
        }
    }

    private void offroadVFX() {
        for(int i = 0; i < VehicleDynamics.wheelColliders.Length; i++) {
            var sandDustMain = sandDust[i].main;
            var sandDustemission = sandDust[i].emission;
            if(currentSurface[i] == surfaceType.Grass) {
                smoke[i].Stop();
                skidmarksOffroad[i].emitting = (hasTrails && VehicleDynamics.wheelColliders[i].isGrounded) ? true : false;
                if(PC.kph > 10 && VehicleDynamics.wheelColliders[i].isGrounded && hasSmokes) {
                    if(smokeFlag != true) {
                        sandDust[i].Play();
                        smokeFlag = true;
                    }
                } else {
                    if(smokeFlag != false) {
                        sandDust[i].Stop();
                        smokeFlag = false;
                    }
                }
            } else if(currentSurface[i] == surfaceType.Sand) {
                smoke[i].Stop();
                skidmarksOffroad[i].emitting = (hasTrails && VehicleDynamics.wheelColliders[i].isGrounded) ? true : false;
                skidmarksDrifting[i].emitting = false;

                if((VehicleDynamics.wheelColliders[i].isGrounded && PC.kph > 20 || PC.kph < -20)  && hasSmokes) {
                    // if(PC.Kph > 0) {
                    //     sandDustMain.simulationSpeed = (PC.Kph > 200) ? -14 : (PC.Kph > 100) ? -7 : (PC.Kph > 30) ? -3 : -1;
                    // } else {
                    //     sandDustMain.simulationSpeed = (PC.Kph < -200) ? 14 : (PC.Kph < -100) ? 7 : (PC.Kph < -30) ? 3 : 1;
                    // }
                    // sandDustMain.simulationSpeed = (PC.Kph > 200) ? 10 : (PC.Kph > 100) ? 5 : (PC.Kph > 30) ? 3 : 2;
                    sandDustMain.simulationSpeed = (PC.kph > 200) ? 4 : (PC.kph > 100) ? 3 : (PC.kph > 50) ? 3 : 2;
                    sandDustemission.rateOverTime = (PC.kph > 200) ? 50 : (PC.kph > 100) ? 30 : (PC.kph > 30) ? 20 : 10;
                    sandDust[i].Play();
                } else {
                    // if(smokeFlag != false) {
                        sandDust[i].Stop();
                    // }
                }
            }
        }
    }

    private void GroundCheck() {
        WheelHit hit;
        for(int i = 0; i < VehicleDynamics.wheelColliders.Length; i++) {
            if(VehicleDynamics.wheelColliders[i].GetGroundHit(out hit)) {
                currentSurface[i] = 
                (hit.collider.material.dynamicFriction == 1) ? surfaceType.Asphalt
                :
                (hit.collider.material.dynamicFriction == 0.85f) ? surfaceType.Grass 
                :
                (hit.collider.material.dynamicFriction == 0.8f) ? surfaceType.Sand
                :
                (hit.collider.material.dynamicFriction == 0.7f) ? surfaceType.Water
                :
                surfaceType.NotStated;
            }
        }
    }
*/
    private void lights() {
        if(hasHeadLights) {
            dippedBeamsON.SetActive(VehicleManager.VehicleInputHandler.activateDippedBeams ? true : false);
            highBeamsON.SetActive(VehicleManager.VehicleInputHandler.activateHighBeams ? true : false);
        }
        if(hasTailLighs) {
            tailLightsON.SetActive((VehicleDynamics.isBraking) ? true : false);
        }
        if(hasReverseLighs) {
            var Ts = VehicleManager.Transmission;
            reverseLightsON.SetActive((Ts.currentGearRatio < 0) ? true : false);
        }
    }

    private void RotationFan() {
        for(int i = 0; i < Fan.Length; i++) {
            Fan[i].transform.Rotate(0, FanSpeed, 0);
        }
    }

    public void ChangeColor(int coloredId, int materialId) {
        if(_colorCustomization) {
            for(int i = 0; i < colored[coloredId].basketPart.Length; i++) {
                colored[coloredId].basketPart[i].GetComponent<MeshRenderer>().material = colored[coloredId].material[materialId];
            }
        }
    }

    private void GetVehicleData() {
        // var vehicleData = Resources.Load<VehicleData>("VehiclesConfig" + "/" + VehicleManager.id);
        VehicleData vehicleData = this.VehicleManager.vehicleData;
        _colorCustomization = vehicleData.colorCustomization;
        ChangeColor(0, vehicleData.bodyMaterailId);
        ChangeColor(1, vehicleData.diskMaterailId);
    }
}

[System.Serializable]
internal enum surfaceType{
    Asphalt, Grass, Sand, Water, Snow, Ice, NotStated
}

[System.Serializable]
public class ColoredItem {
    public int id;
    public GameObject[] basketPart;
    public Material[] material;
}