using UnityEngine;
using System;

namespace TrophyRace.Architecture {
    public class VehicleSelector : MonoBehaviour {
        
        [SerializeField]
        private int _vehiclePointer = 0;
        [SerializeField]
        private int _selectedVehicleId = 1;
        public int[] _allowedVehiclesId;
        private VehicleList _vehicleList;
        private VehicleSpawner _vehicleSpawner;
        private MenuManager _menuManager;

        public int vehiclePointer => this._vehiclePointer;
        public int selectedVehicleId => this._selectedVehicleId;

        // Здесь добавьте ссылки на _vehicleList, _vehicleSpawner и другие компоненты

        private void Awake() {
            VehicleList.vehicleListInitializedEvent += OnVehicleListInitialized;
        }

        private void Start() {
            _vehicleList = GetComponent<VehicleList>();
            _menuManager = GetComponent<MenuManager>();
            if(PlayerPrefs.HasKey("pointer")) {
                this._vehiclePointer = PlayerPrefs.GetInt("pointer");
            }
        }

        private void OnVehicleListInitialized() {
            VehicleList.vehicleListInitializedEvent -= OnVehicleListInitialized;
            _vehicleSpawner = GetComponent<VehicleSpawner>();
            if(PlayerPrefs.HasKey("selectedVehicleId")) {
                this._selectedVehicleId = PlayerPrefs.GetInt("selectedVehicleId");
            }
            _vehicleSpawner.SpawnVehicle(this._selectedVehicleId, SpawnMode.DisableCameras | SpawnMode.DisableReflectionProbes | SpawnMode.DisableMovement);
        }

        public void SetFilterById(int[] allowedVehiclesId) {
            this._allowedVehiclesId = allowedVehiclesId;

            int m_selectedVehicleId = PlayerPrefs.GetInt("selectedVehicleId");
            bool isAllowed = IsVehicleAllowed(m_selectedVehicleId);

            if(!isAllowed) {
                foreach(var vehicleId in _allowedVehiclesId) {
                    if(_vehicleList.allVehiclesInGame.Exists(vehicle => vehicle.id == vehicleId)) {
                        m_selectedVehicleId = vehicleId;
                        break;
                    }
                }
                this._vehiclePointer = m_selectedVehicleId;
                this._selectedVehicleId = m_selectedVehicleId;
                PlayerPrefs.SetInt("selectedVehicleId", this._selectedVehicleId);
                Destroy(_vehicleSpawner.playerVehicle);
                _vehicleSpawner.SpawnVehicle(selectedVehicleId, SpawnMode.DisableCameras | SpawnMode.DisableReflectionProbes | SpawnMode.DisableMovement);
            }
        }

        private int FindNextVehicleIndex(int startIndex, int step) {
            int index = startIndex - 1; // Используем стартовый индекс - 1 для корректной работы

            int count = _vehicleList.allVehiclesInGame.Count;

            while(true) {
                index = (index + step + count) % count; // Проход по элементам в цикле

                int currentVehicleId = _vehicleList.allVehiclesInGame[index].id;
                if(Array.Exists(_allowedVehiclesId, id => id == currentVehicleId)) {
                    break; // Если нашли подходящий, выходим из цикла
                }

                if(index == startIndex - 1) {
                    break; // Если прошли полный круг и не нашли, выходим из цикла
                }
            }

            return index + 1; // Возвращаем индекс + 1 для согласованности с отображением пользователю (1 до Count)
        }

        public void RightButton() {
            Destroy(_vehicleSpawner.playerVehicle);

            int newIndex = FindNextVehicleIndex(this._vehiclePointer, 1);
            this._vehiclePointer = newIndex;

            PlayerPrefs.SetInt("pointer", this._vehiclePointer);
            this._selectedVehicleId =  _vehicleList.allVehiclesInGame[this._vehiclePointer - 1].id;
            PlayerPrefs.SetInt("selectedVehicleId", _vehicleList.allVehiclesInGame[this._vehiclePointer - 1].id);
            _vehicleSpawner.SpawnVehicle(_vehicleList.allVehiclesInGame[this._vehiclePointer - 1].id, SpawnMode.DisableCameras | SpawnMode.DisableReflectionProbes | SpawnMode.DisableMovement);
        }

        public void LeftButton() {
            Destroy(_vehicleSpawner.playerVehicle);

            int newIndex = FindNextVehicleIndex(this._vehiclePointer, -1);
            this._vehiclePointer = newIndex;

            PlayerPrefs.SetInt("pointer", this._vehiclePointer);
            this._selectedVehicleId =  _vehicleList.allVehiclesInGame[this._vehiclePointer - 1].id;
            PlayerPrefs.SetInt("selectedVehicleId", _vehicleList.allVehiclesInGame[this._vehiclePointer - 1].id);
            _vehicleSpawner.SpawnVehicle(_vehicleList.allVehiclesInGame[this._vehiclePointer - 1].id, SpawnMode.DisableCameras | SpawnMode.DisableReflectionProbes | SpawnMode.DisableMovement);
        }

        bool IsVehicleAllowed(int vehicleID) {
            foreach(int allowedID in _allowedVehiclesId) {
                if(allowedID == vehicleID) {
                    return true;
                }
            }
            return false;
        }
    }
}