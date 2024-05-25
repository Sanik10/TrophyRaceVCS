using UnityEngine;
using System;

namespace TrophyRace.Architecture {
    public class VehicleSelector : MonoBehaviour {
        
        [SerializeField]
        private string _selectedVehicleGuid = "d25eb36b-bd44-442e-9194-9f60a9e5dfb9";
        public string[] _allowedVehiclesGuid;
        private VehicleList _vehicleList;
        private VehicleSpawner _vehicleSpawner;
        private MenuManager _menuManager;

        public string selectedVehicleGuid => this._selectedVehicleGuid;

        private void Start() {
            _vehicleList = FindObjectOfType<VehicleList>();
            _menuManager = GetComponent<MenuManager>();
            OnVehicleListInitialized();
        }

/*
        private void OnVehicleListInitialized() {
            VehicleList.vehicleListInitializedEvent -= OnVehicleListInitialized;
            _vehicleSpawner = GetComponent<VehicleSpawner>();

            // Проверяем, есть ли сохраненный GUID
            if(PlayerPrefs.HasKey("selectedVehicleGuid")) {
                string savedGuid = PlayerPrefs.GetString("selectedVehicleGuid");
                Debug.Log()

                // Проверяем, существует ли сохраненный GUID в списке доступных автомобилей
                if(IsVehicleAllowed(savedGuid)) {
                    // Если существует, выбираем сохраненный автомобиль
                    this._selectedVehicleGuid = savedGuid;
                } else {
                    // Если не существует, выбираем первый доступный автомобиль в списке
                    if (_allowedVehiclesGuid.Length > 0) {
                        this._selectedVehicleGuid = _allowedVehiclesGuid[0];
                    } else {
                        // Если список пуст, устанавливаем стандартное значение
                        this._selectedVehicleGuid = "d25eb36b-bd44-442e-9194-9f60a9e5dfb9";
                    }
                }
            } else {
                Debug.Log("Нет ключа в префс");
                // Если нет сохраненного GUID, выбираем первый доступный автомобиль в списке
                if (_allowedVehiclesGuid.Length > 0) {
                    this._selectedVehicleGuid = _allowedVehiclesGuid[0];
                } else {
                    Debug.Log("Список пуст");
                    // Если список пуст, устанавливаем стандартное значение
                    this._selectedVehicleGuid = "d25eb36b-bd44-442e-9194-9f60a9e5dfb9";
                }
            }

            PlayerPrefs.SetString("selectedVehicleGuid", this._selectedVehicleGuid);
            _vehicleSpawner.SpawnVehicle(this._selectedVehicleGuid, SpawnMode.DisableCameras | SpawnMode.DisableReflectionProbes | SpawnMode.DisableMovement);
        }
*/

        private void OnVehicleListInitialized() {
            _vehicleSpawner = GetComponent<VehicleSpawner>();

            // Проверяем, есть ли сохраненный GUID
            if(PlayerPrefs.HasKey("selectedVehicleGuid")) {
                this._selectedVehicleGuid = PlayerPrefs.GetString("selectedVehicleGuid");
            } else {
                Debug.Log("Нет ключа selecetedVehicleGuid в PlayerPrefs");
                // Если нет сохраненного GUID, выбираем первый доступный автомобиль в списке
                if (_allowedVehiclesGuid.Length > 0) {
                    this._selectedVehicleGuid = _allowedVehiclesGuid[0];
                } else {
                    Debug.LogWarning("VehicleList пуст");
                    // Если список пуст, устанавливаем стандартное значение
                    this._selectedVehicleGuid = "d25eb36b-bd44-442e-9194-9f60a9e5dfb9";
                }
            }

            PlayerPrefs.SetString("selectedVehicleGuid", this._selectedVehicleGuid);
            _vehicleSpawner.SpawnVehicle(this._selectedVehicleGuid, SpawnMode.DisableCameras | SpawnMode.DisableReflectionProbes | SpawnMode.DisableMovement);
        }



        public void SetFilterByGuid(string[] allowedVehiclesGuid) {
            this._allowedVehiclesGuid = allowedVehiclesGuid;

            string m_selectedVehicleGuid = PlayerPrefs.GetString("selectedVehicleGuid");
            bool isAllowed = IsVehicleAllowed(m_selectedVehicleGuid);

            if(!isAllowed) {
                foreach(var vehicleGuid in _allowedVehiclesGuid) {
                    if(_vehicleList.allVehiclesInGame.Exists(vehicle => vehicle.guid == vehicleGuid)) {
                        m_selectedVehicleGuid = vehicleGuid;
                        break;
                    }
                }
                this._selectedVehicleGuid = m_selectedVehicleGuid;
                PlayerPrefs.SetString("selectedVehicleGuid", this._selectedVehicleGuid);
                Destroy(_vehicleSpawner.playerVehicle);
                _vehicleSpawner.SpawnVehicle(selectedVehicleGuid, SpawnMode.DisableCameras | SpawnMode.DisableReflectionProbes | SpawnMode.DisableMovement);
            }
        }

        private int FindNextVehicleIndex(int startIndex, int step) {
            int index = startIndex - 1; // Используем стартовый индекс - 1 для корректной работы

            int count = _vehicleList.allVehiclesInGame.Count;

            while(true) {
                index = (index + step + count) % count; // Проход по элементам в цикле

                string currentVehicleGuid = _vehicleList.allVehiclesInGame[index].guid;
                if(Array.Exists(_allowedVehiclesGuid, guid => guid == currentVehicleGuid)) {
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

            int currentIndex = Array.IndexOf(_allowedVehiclesGuid, _selectedVehicleGuid);
            int newIndex = (currentIndex + 1) % _allowedVehiclesGuid.Length;
            _selectedVehicleGuid = _allowedVehiclesGuid[newIndex];
            PlayerPrefs.SetString("selectedVehicleGuid", _selectedVehicleGuid);

            _vehicleSpawner.SpawnVehicle(_selectedVehicleGuid, SpawnMode.DisableCameras | SpawnMode.DisableReflectionProbes | SpawnMode.DisableMovement);
        }

        public void LeftButton() {
            Destroy(_vehicleSpawner.playerVehicle);

            int currentIndex = Array.IndexOf(_allowedVehiclesGuid, _selectedVehicleGuid);
            int newIndex = (currentIndex - 1 + _allowedVehiclesGuid.Length) % _allowedVehiclesGuid.Length;
            _selectedVehicleGuid = _allowedVehiclesGuid[newIndex];
            PlayerPrefs.SetString("selectedVehicleGuid", _selectedVehicleGuid);

            _vehicleSpawner.SpawnVehicle(_selectedVehicleGuid, SpawnMode.DisableCameras | SpawnMode.DisableReflectionProbes | SpawnMode.DisableMovement);
        }

        bool IsVehicleAllowed(string vehicleGuid) {
            foreach(string allowedGuid in _allowedVehiclesGuid) {
                if(allowedGuid == vehicleGuid) {
                    return true;
                }
            }
            return false;
        }
    }
}