using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace TrophyRace.Architecture {
    public class VehicleList : MonoBehaviour {

        public static event Action vehicleListInitializedEvent;

        private VehicleRepository _vehicleRepository;

        [SerializeField]
        private List<VehicleData> _allVehicles = new List<VehicleData>();
        [SerializeField]
        private List<VehicleData> _playerVehicles = new List<VehicleData>();
        [SerializeField]
        private List<VehicleData> _availableForPurchase = new List<VehicleData>();
        
        public List<VehicleData> allVehiclesInGame => this._allVehicles;
        public List<VehicleData> playerVehicles => this._playerVehicles;
        public List<VehicleData> availableForPurchase => this._availableForPurchase;

        private void Awake() {
            VehiclesShop.vehicleBuyedEvent += ReimportVehicles;
        }

        private void Start() {
            this._vehicleRepository = new VehicleRepository(); // Создаем экземпляр VehicleRepository
            this._vehicleRepository.Initialize();

            ReimportVehicles();

            vehicleListInitializedEvent?.Invoke();
        }

        private void ReimportVehicles() {
            this._allVehicles = this._vehicleRepository.GetAvailableVehicles();
            this._availableForPurchase = GetAvialableForPurchaseVehicles();
            this._playerVehicles = GetPlayerOwnedVehicles();
        }

        private void OnDisable() {
            VehiclesShop.vehicleBuyedEvent -= ReimportVehicles;
        }

        private List<VehicleData> GetPlayerOwnedVehicles() {
            if (this._allVehicles != null) {
                return this._allVehicles.Where(vehicle => vehicle.isOwned).ToList();
            }
            return new List<VehicleData>();
        }

        public List<string> GetPlayerOwnedVehiclesGuid() {
            return _playerVehicles.Select(vehicle => vehicle.guid).ToList();
        }

        public List<string> GetAllVehiclesGuid() {
            return _allVehicles.Select(vehicle => vehicle.guid).ToList();
        }

        private List<VehicleData> GetAvialableForPurchaseVehicles() {
            if (this._allVehicles != null) {
                return this._allVehicles.Where(vehicle => !vehicle.isOwned).ToList();
            }
            return new List<VehicleData>();
        }
    }
}