using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using TMPro;

namespace TrophyRace.Architecture {

    public class CarInfoPanel : MonoBehaviour {

        [SerializeField]
        private int _fontSize = 35;
        [SerializeField]
        private LocalizedString localizedMaxSpeed;
        [SerializeField]
        private LocalizedString localizedPower;
        [SerializeField]
        private LocalizedString localizedGearsQuantity;
        [SerializeField]
        private LocalizedString localizedFirstGearRatio;
        [SerializeField]
        private LocalizedString localizedTurningRadius;
        [SerializeField]
        private LocalizedString localizedSteeringWheelSpeed;
        [SerializeField]
        private LocalizedString localizedMileage;
        [SerializeField]
        private List<TextMeshProUGUI> _textsList = new List<TextMeshProUGUI>();
        private VehicleList _vehicleList;
        public bool view = false;

        private void Start() {
            _vehicleList = GameObject.FindObjectOfType<VehicleList>();
            foreach(TextMeshProUGUI label in this._textsList) {
                label.fontSize = this._fontSize;
            }

            VehicleList.vehicleListInitializedEvent += Subscribe;
        }

        private void OnEnable() {
            Subscribe();
            MenuManager.GarageOpenedEvent += CarInfo;
        }

        private void OnDisable() {
            VehicleList.vehicleListInitializedEvent -= Subscribe;
            VehicleData.onDataLoadedEvent -= CarInfo;
            MenuManager.GarageOpenedEvent -= CarInfo;
        }

        public void Subscribe() {
            VehicleData.onDataLoadedEvent += CarInfo;
        }

        public void CarInfo() {
            if (_vehicleList == null) {
                _vehicleList = GameObject.FindObjectOfType<VehicleList>();
                // Debug.LogError("VehicleList is null");
            }

            string selectedVehicleGuid = PlayerPrefs.GetString("selectedVehicleGuid", string.Empty);
            VehicleData vehicleData = this._vehicleList.allVehiclesInGame.Find(vehicle => vehicle.guid == selectedVehicleGuid);

            if (vehicleData == null) {
                Debug.LogError("VehicleData is null for guid: " + selectedVehicleGuid);
                return;
            }

            if (_textsList.Count < 9) {
                Debug.LogError("Texts list does not contain enough elements");
                return;
            }

            if (_textsList[0] != null) _textsList[0].text = vehicleData.vehicleName.ToString();
            if (_textsList[1] != null) _textsList[1].text = vehicleData.range.ToString("f0") + " TP";
            if (_textsList[2] != null) _textsList[2].text = localizedMaxSpeed.GetLocalizedString(new object[1] { vehicleData.maxSpeed.ToString() });
            if (_textsList[3] != null) _textsList[3].text = localizedPower.GetLocalizedString(new object[2] { vehicleData.maxPower.ToString(), (vehicleData.maxPower/1.36).ToString("f1") });
            if (_textsList[4] != null) _textsList[4].text = localizedGearsQuantity.GetLocalizedString(new object[1] { vehicleData.frontGearsQuantity.ToString() });
            if (_textsList[5] != null) _textsList[5].text = localizedFirstGearRatio.GetLocalizedString(new object[1] { vehicleData.firstGear.ToString() });
            if (_textsList[6] != null) _textsList[6].text = localizedTurningRadius.GetLocalizedString(new object[1] { vehicleData.radius.ToString() });
            if (_textsList[7] != null) _textsList[7].text = localizedSteeringWheelSpeed.GetLocalizedString(new object[1] { vehicleData.steeringWheelSpeed.ToString() });
            if (_textsList[8] != null) _textsList[8].text = localizedMileage.GetLocalizedString(new object[1] { (vehicleData.mileage / 1000f).ToString() });

            if (view) {
                Debug.Log(vehicleData.normalizedPower + " normalizedPower");
                Debug.Log(vehicleData.normalizedBrakeTorque + " normalizedBrakeTorque");
                Debug.Log(vehicleData.normalizedInertia + " normalizedInertia");
                Debug.Log(vehicleData.normalizedMaxSpeed + " normalizedMaxSpeed");
                Debug.Log(vehicleData.normalizedTireIntegrity + " normalizedTireIntegrity");
                Debug.Log(vehicleData.normalizedPowerProcentIdle + " normalizedPowerProcentIdle");
                Debug.Log(vehicleData.normalizedPowerProcentMax + " normalizedPowerProcentMax");
                Debug.Log(vehicleData.normalizedShiftTime + " normalizedShiftTime");
                Debug.Log(vehicleData.normalizedSteeringSpeed + " normalizedSteeringSpeed");
                Debug.Log(vehicleData.normalizedMass + " normalizedMass");
            }
        }
    }
}