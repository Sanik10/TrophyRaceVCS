using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TrophyRace.Architecture {
    public class TachometrElectro : MonoBehaviour {

        private VehicleManager _VehicleManager;
        private Engine _Engine;
        private Transmission _Transmission;
        private PhysicsCalculation _PhysicsCalculation;
        private VehicleDynamics _VehicleDynamics;
        private TextMeshProUGUI _RPMText;
        private TextMeshProUGUI _GearText;
        private TextMeshProUGUI _SpeedText;
        private TextMeshProUGUI _CurrentMileageText;
        private TextMeshProUGUI _AllMileageText;
        private int _maxRpm;


        private void Start() {
            foreach (Transform child in gameObject.transform) {
                if(child.transform.name == "RPMText") {
                    _RPMText = child.transform.GetComponent<TextMeshProUGUI>();
                }
                if(child.transform.name == "GearText") {
                    _GearText = child.transform.GetComponent<TextMeshProUGUI>();
                }
                if(child.transform.name == "SpeedText") {
                    _SpeedText = child.transform.GetComponent<TextMeshProUGUI>();
                }
                if(child.transform.name == "CurrentMileageText") {
                    _CurrentMileageText = child.transform.GetComponent<TextMeshProUGUI>();
                }
                if(child.transform.name == "AllMileageText") {
                    _AllMileageText = child.transform.GetComponent<TextMeshProUGUI>();
                }
            }
        }

        private void Update() {
            if(_VehicleManager == null) {
                _VehicleManager = GameObject.Find("scripts").GetComponent<VehicleSpawner>().playerVehicle.GetComponent<VehicleManager>();
                _Engine = _VehicleManager.Engine;
                _Transmission = _VehicleManager.Transmission;
                _PhysicsCalculation = _VehicleManager.PhysicsCalculation;
                _VehicleDynamics = _VehicleManager.VehicleDynamics;
            }

            if(_VehicleManager != null) {
                _RPMText.text = _Engine.rpm.ToString("f0") + " rpm";
                _GearText.text = (_Transmission.currentGear == 0) ? "N" : (_Transmission.currentGearRatio < 0) ? "R" : (_Transmission.currentGear).ToString("");
                _SpeedText.text = _PhysicsCalculation.speed.ToString("0");
                _SpeedText.color = (_VehicleDynamics.maxSpeedOnCurrentGear > _PhysicsCalculation.kph + 5) ? new Color(255, 255, 255) : new Color (255, 0, 0);
                _CurrentMileageText.text = (_VehicleDynamics.currentMileage).ToString("f2") + " м";
                _AllMileageText.text = ((_VehicleDynamics.allMileage + _VehicleDynamics.currentMileage) / 1000).ToString("f2") + " км";
            }
        }
    }
}