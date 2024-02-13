using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TrophyRace.Architecture {

    public class CarInfoPanel : MonoBehaviour {

        public List<TextMeshProUGUI> _textsList;
        private VehicleList _vehicleList;
        public bool view = false;

        private void Start() {
            _textsList = new List<TextMeshProUGUI>();
                _textsList.Add(GameObject.Find("VehicleName").GetComponent<TextMeshProUGUI>()); //1
                _textsList.Add(GameObject.Find("TuningPoints").GetComponent<TextMeshProUGUI>()); //2
                _textsList.Add(GameObject.Find("MaxSpeed").GetComponent<TextMeshProUGUI>()); //3
                _textsList.Add(GameObject.Find("EnginePower").GetComponent<TextMeshProUGUI>()); //4
                _textsList.Add(GameObject.Find("GearsQuantity").GetComponent<TextMeshProUGUI>()); //5
                _textsList.Add(GameObject.Find("FirstGearRatio").GetComponent<TextMeshProUGUI>()); //6
                _textsList.Add(GameObject.Find("TurningRadius").GetComponent<TextMeshProUGUI>()); //7
                _textsList.Add(GameObject.Find("SteeringWheelSpeed").GetComponent<TextMeshProUGUI>()); //8
                _textsList.Add(GameObject.Find("Mileage").GetComponent<TextMeshProUGUI>()); //9

            _vehicleList = GameObject.Find("scripts").GetComponent<VehicleList>();
            VehicleList.vehicleListInitializedEvent += Subscribe;
        }

        private void OnEnable() {
            Subscribe();
        }

        private void OnDisable() {
            VehicleList.vehicleListInitializedEvent -= Subscribe;
            VehicleData.onDataLoadedEvent -= CarInfo;
        }

        public void Subscribe() {
            VehicleData.onDataLoadedEvent += CarInfo;
        }

        public void CarInfo() {
            var vehicleData = this._vehicleList.allVehiclesInGame.Find(vehicle => vehicle.id == PlayerPrefs.GetInt("selectedVehicleId"));
            _textsList[0].text = vehicleData.vehicleName.ToString();
            // _textsList[1].text = (vehicleData.maxSpeed * 1.8 + vehicleData.maxPower * 1.20 + 2000 / vehicleData.radius + 75 / vehicleData.shiftTime).ToString("f0") + " TP";
            _textsList[1].text = vehicleData.range.ToString("f0") + " TP";
            _textsList[2].text = "Макс. Скорость: " + vehicleData.maxSpeed.ToString() + "км/ч";
            _textsList[3].text = "Мощность: " + vehicleData.maxPower.ToString() + "ЛС / " + (vehicleData.maxPower/1.36f).ToString("f1") + "КВт";
            _textsList[4].text = "Количество передач: " + vehicleData.frontGearsQuantity.ToString() + " + 1R";
            _textsList[5].text = "Передаточное число первой: " + vehicleData.firstGear.ToString();
            _textsList[6].text = "Радиус поворота: " + vehicleData.radius.ToString() + "м";
            _textsList[7].text = "Скорость поворота руля: " + vehicleData.steeringWheelSpeed.ToString() + "c";
            _textsList[8].text = "Пробег: " + (vehicleData.mileage / 1000f).ToString("f2") + "км";

            if(view) {
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