using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace TrophyRace.Architecture {
    public class VehicleRepository : Repository {
        
        private List<VehicleData> _allVehicles;

        public override void OnCreate() {
        }

        public override void Initialize() {
            LoadAllVehicles();
        }

        private void LoadAllVehicles() {
            // Фильтрация по IncludeVehicle
            _allVehicles = new List<VehicleData>(Resources.LoadAll<VehicleData>("VehiclesConfig").Where(vehicle => vehicle.includeVehicleInGame));
        }

        public List<VehicleData> GetAvailableVehicles() {
            return _allVehicles;
        }

        public override void Save() {
            // Сохранение состояния автомобилей в сохраненные данные
            // Например, сохранение списка _playerOwnedVehicles в файл или базу данных
        }
    }
}