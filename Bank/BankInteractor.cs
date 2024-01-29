using UnityEngine;

namespace TrophyRace.Architecture {
    public class BankInteractor : Interactor {

        private BankRepository _repository;

        public int typtol => this._repository.typtol;
        public int qbit => this._repository.qbit;

        public override void OnCreate() {
            base.OnCreate();
            this._repository = Game.GetRepository<BankRepository>();
        }

        public override void Initialize() {
            Bank.Initialize(this);
        }

        public int GetCurrencyAmount(CurrencyType currencyType) {
            switch (currencyType) {
                case CurrencyType.Typtol:
                    return typtol;
                case CurrencyType.Qbit:
                    return qbit;
                default:
                    return 0;
            }
        }

        public bool IsEnoughCurrency(int value, CurrencyType currencyType) {
            int currentCurrency = GetCurrencyAmount(currencyType);
            return currentCurrency >= Mathf.Abs(value);
        }

        public void AddCurrency(object sender, int value, CurrencyType currencyType) {
            switch (currencyType) {
                case CurrencyType.Typtol:
                    this._repository.typtol += Mathf.Abs(value);
                    break;
                case CurrencyType.Qbit:
                    this._repository.qbit += Mathf.Abs(value);
                    break;
            }

            this._repository.Save();
        }

        public void SpendCurrency(object sender, int value, CurrencyType currencyType) {
            switch (currencyType) {
                case CurrencyType.Typtol:
                    this._repository.typtol -= Mathf.Abs(value);
                    break;
                case CurrencyType.Qbit:
                    this._repository.qbit -= Mathf.Abs(value);
                    break;
            }

            this._repository.Save();
        }
    }

    public enum CurrencyType {
        Typtol,
        Qbit
    }
}