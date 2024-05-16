using UnityEngine;
using System;

namespace TrophyRace.Architecture {
    public class VehiclesShop : MonoBehaviour {

        public static event Action vehicleBuyedEvent;

        private VehicleList _vehicleList;
        private VehicleInteractor _vehicleInteractor;

        public void Start() {
            _vehicleInteractor = new VehicleInteractor();
            _vehicleList = GetComponent<VehicleList>();
            LoadAvailableVehicles();
        }

        /* public void BuyVehicle() {
            int vehicleId = PlayerPrefs.GetInt("selectedVehicleId");
            VehicleData vehicleToBuy = _vehicleList.availableForPurchase.Find(vehicle => vehicle.id == vehicleId);

            if (vehicleToBuy != null && !vehicleToBuy.isOwned) {
                int missingAmount = _vehicleInteractor.MissingCurrency(vehicleToBuy.price, CurrencyType.Qbit);

                if (missingAmount == 0) {
                    if (_vehicleInteractor.BuyVehicle(vehicleToBuy)) {
                        Debug.Log("Машина куплена: " + vehicleId);
                        // Установка флага владения машины
                        vehicleToBuy.isOwned = true;
                        // Оповещение о покупке машины
                        vehicleBuyedEvent?.Invoke();
                        vehicleToBuy.Save("VehiclesShop");
                    }
                } else {
                    Debug.Log($"Недостаточно средств для покупки машины. Не хватает: {missingAmount} Qbit.");
                }
            } else {
                Debug.Log("Машина уже куплена или недоступна для покупки: " + vehicleId);
            }
        } */

        public void BuyVehicle() {
            string vehicleGuid = PlayerPrefs.GetString("selectedVehicleGuid");
            VehicleData vehicleToBuy = _vehicleList.availableForPurchase.Find(vehicle => vehicle.guid == vehicleGuid);

            if (vehicleToBuy != null && !vehicleToBuy.isOwned) {
                int missingAmount = _vehicleInteractor.MissingCurrency(vehicleToBuy.price, CurrencyType.Qbit);

                if (missingAmount == 0) {
                    if (_vehicleInteractor.BuyVehicle(vehicleToBuy)) {
                        Debug.Log("Машина куплена: " + vehicleGuid);
                        // Установка флага владения машины
                        vehicleToBuy.isOwned = true;
                        // Оповещение о покупке машины
                        vehicleBuyedEvent?.Invoke();
                        vehicleToBuy.Save("VehiclesShop");
                    }
                } else {
                    Debug.Log($"Недостаточно средств для покупки машины. Не хватает: {missingAmount} Qbit.");
                }
            } else {
                Debug.Log("Машина уже куплена или недоступна для покупки: " + vehicleGuid);
            }
        }

/*
public void BuySelectedVehicle() {
    int vehicleId = PlayerPrefs.GetInt("selectedVehicleId");
    VehicleData vehicleToBuy = _vehicleList.availableForPurchase.Find(vehicle => vehicle.id == vehicleId);

    if(vehicleToBuy != null) {
        bool purchaseSuccessful = BuyVehicle(vehicleToBuy, _bank);

        if (purchaseSuccessful) {
            Debug.Log("Машина куплена: " + vehicleId);
        } else {
            Debug.Log("Машина уже куплена или недоступна для покупки: " + vehicleId);
        }
    }
}
*/

        private void LoadAvailableVehicles() {

        }
    }
}