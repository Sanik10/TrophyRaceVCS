using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using TMPro;

namespace TrophyRace.Architecture {

    public class CarInfoPanel : MonoBehaviour {

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
            _vehicleList = GameObject.Find("scripts").GetComponent<VehicleList>();

            VehicleList.vehicleListInitializedEvent += Subscribe;
        }

        private void OnEnable() {
            // localizedMaxSpeed.Arguments = new object[] { vehicleData.maxSpeed.ToString() };
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
            VehicleData vehicleData = this._vehicleList.allVehiclesInGame.Find(vehicle => vehicle.id == PlayerPrefs.GetInt("selectedVehicleId"));
            _textsList[0].text = vehicleData.vehicleName.ToString();
            // _textsList[1].text = (vehicleData.maxSpeed * 1.8 + vehicleData.maxPower * 1.20 + 2000 / vehicleData.radius + 75 / vehicleData.shiftTime).ToString("f0") + " TP";
            _textsList[1].text = vehicleData.range.ToString("f0") + " TP";
            // _textsList[2].text = "Макс. Скорость: " + vehicleData.maxSpeed.ToString() + "км/ч";
            _textsList[2].text = localizedMaxSpeed.GetLocalizedString(new object[1] { vehicleData.maxSpeed.ToString() });
            // _textsList[3].text = "Мощность: " + vehicleData.maxPower.ToString() + "ЛС / " + (vehicleData.maxPower/1.36f).ToString("f1") + "КВт";
            _textsList[3].text = localizedPower.GetLocalizedString(new object[2] { vehicleData.maxPower.ToString(), (vehicleData.maxPower/1.36).ToString("f1") });
            // _textsList[4].text = "Количество передач: " + vehicleData.frontGearsQuantity.ToString() + " + 1R";
            _textsList[4].text = localizedGearsQuantity.GetLocalizedString(new object[1] { vehicleData.frontGearsQuantity.ToString() });
            // _textsList[5].text = "Передаточное число первой: " + vehicleData.firstGear.ToString();
            _textsList[5].text = localizedFirstGearRatio.GetLocalizedString(new object[1] { vehicleData.firstGear.ToString() });
            // _textsList[6].text = "Радиус поворота: " + vehicleData.radius.ToString() + "м";
            _textsList[6].text = localizedTurningRadius.GetLocalizedString(new object[1] { vehicleData.radius.ToString() });
            // _textsList[7].text = "Скорость поворота руля: " + vehicleData.steeringWheelSpeed.ToString() + "c";
            _textsList[7].text = localizedSteeringWheelSpeed.GetLocalizedString(new object[1] { vehicleData.steeringWheelSpeed.ToString() });
            // _textsList[8].text = "Пробег: " + (vehicleData.mileage / 1000f).ToString("f2") + "км";
            _textsList[8].text = localizedMileage.GetLocalizedString(new object[1] { (vehicleData.mileage / 1000f) });
            // localizedMileage.Arguments = new object[1] { vehicleData.mileage / 1000f };
            // // Получаем локализованную строку
            // string formattedMileage = localizedMileage.GetLocalizedString().Result;
            // // Устанавливаем текст в TextMeshProUGUI
            // _textsList[8].text = formattedMileage;

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

/*
using UnityEngine;
using UnityEngine.Localization;
using TMPro;

public class CarInfoPanel : MonoBehaviour
{
    [SerializeField]
    private LocalizedString localizedMaxSpeed;
    [SerializeField]
    private List<TextMeshProUGUI> textFields = new List<TextMeshProUGUI>();
    private VehicleList vehicleList;

    private void Start()
    {
        // if(_textsList == null) {
            //     _textsList.Add(GameObject.Find("VehicleName").GetComponent<TextMeshProUGUI>()); //0
            //     _textsList.Add(GameObject.Find("TuningPoints").GetComponent<TextMeshProUGUI>()); //1
            //     _textsList.Add(GameObject.Find("MaxSpeed").GetComponent<TextMeshProUGUI>()); //2
            //     _textsList.Add(GameObject.Find("EnginePower").GetComponent<TextMeshProUGUI>()); //3
            //     _textsList.Add(GameObject.Find("GearsQuantity").GetComponent<TextMeshProUGUI>()); //4
            //     _textsList.Add(GameObject.Find("FirstGearRatio").GetComponent<TextMeshProUGUI>()); //5
            //     _textsList.Add(GameObject.Find("TurningRadius").GetComponent<TextMeshProUGUI>()); //6
            //     _textsList.Add(GameObject.Find("SteeringWheelSpeed").GetComponent<TextMeshProUGUI>()); //7
            //     _textsList.Add(GameObject.Find("Mileage").GetComponent<TextMeshProUGUI>()); //8
            // }

        vehicleList = GameObject.Find("scripts").GetComponent<VehicleList>();
        VehicleList.vehicleListInitializedEvent += Subscribe;
    }

    private void OnEnable()
    {
        Subscribe();
    }

    private void OnDisable()
    {
        VehicleList.vehicleListInitializedEvent -= Subscribe;
        VehicleData.onDataLoadedEvent -= CarInfo;
    }

    public void Subscribe()
    {
        VehicleData.onDataLoadedEvent += CarInfo;
    }

    public void CarInfo()
    {
        var vehicleData = vehicleList.allVehiclesInGame.Find(vehicle => vehicle.id == PlayerPrefs.GetInt("selectedVehicleId"));
        textFields[0].text = vehicleData.vehicleName.ToString();
        textFields[1].text = vehicleData.range.ToString("f0") + " TP";
        textFields[2].text = localizedMaxSpeed.GetLocalizedString(new object[] { vehicleData.maxSpeed }).Result;

        // Other text field updates...
    }
}
*/