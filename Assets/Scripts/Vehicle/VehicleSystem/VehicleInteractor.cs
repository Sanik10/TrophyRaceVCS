using System.Collections.Generic;

namespace TrophyRace.Architecture {
    public class VehicleInteractor : Interactor {
        
        private VehicleRepository _vehicleRepository;

        public override void OnCreate() {
            base.OnCreate();
            _vehicleRepository = Game.GetRepository<VehicleRepository>();
        }

        public override void Initialize() {
            base.Initialize();
            // Здесь можно провести инициализацию, если это необходимо
        }

        public List<VehicleData> GetAvailableVehicles() {
            return _vehicleRepository.GetAvailableVehicles();
        }

        public bool BuyVehicle(VehicleData vehicle) {
            int vehiclePrice = vehicle.price;
            if(Bank.IsEnoughCurrency(vehiclePrice, CurrencyType.Qbit)) {
                Bank.SpendCurrency(this, vehiclePrice, CurrencyType.Qbit);
                // _vehicleRepository.AddVehicleToPlayerCollection(vehicle);
                return true;
            }
            return false; // Если недостаточно средств для покупки
        }

        public int MissingCurrency(int value, CurrencyType currencyType) {
            return Bank.MissingCurrency(value, currencyType);
        }
    }
}