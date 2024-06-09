using System;
using UnityEngine;

namespace TrophyRace.Architecture {
    public static class Bank {

        public static event Action OnBankInitializedEvent;

        private static BankInteractor _bankInteractor;

        public static bool isInitialized {get; private set;}

        public static int GetCurrencyAmount(CurrencyType currencyType) {
            ClassCheck();
            return _bankInteractor.GetCurrencyAmount(currencyType);
        }

        public static bool IsEnoughCurrency(int value, CurrencyType currencyType) {
            ClassCheck();
            return _bankInteractor.IsEnoughCurrency(value, currencyType);
        }

        public static void AddCurrency(object sender, int value, CurrencyType currencyType) {
            ClassCheck();
            _bankInteractor.AddCurrency(sender, Mathf.Abs(value), currencyType);
        }

        public static void SpendCurrency(object sender, int value, CurrencyType currencyType) {
            ClassCheck();
            _bankInteractor.SpendCurrency(sender, Mathf.Abs(value), currencyType);
        }
        
        public static int MissingCurrency(int value, CurrencyType currencyType) {
            ClassCheck();
            int currentCurrency = _bankInteractor.GetCurrencyAmount(currencyType);
            return Mathf.Max(0, Mathf.Abs(value) - currentCurrency);
        }

        public static void Initialize(BankInteractor interactor) {
            _bankInteractor = interactor;
            isInitialized = true;
            OnBankInitializedEvent?.Invoke();
        }

        private static void ClassCheck() {
            if(!isInitialized) {
                throw new Exception("Bank isn't Initialised yet");
            }
        }
    }
}