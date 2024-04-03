using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

namespace TrophyRace.Architecture {

    public class MenuManager : MonoBehaviour {

        private GameObject _HomeButton;
        private GameObject _SettingsButton;
        private GameObject _StartRaceButton;


        public bool preRaceMode = false;
        public bool shopMode = false;


        private cameraSwitcher CS;
        private TextMeshProUGUI balanceText;
        private bool backButtonCall = false;
        private VehicleSpawner _vehicleSpawner;
        private VehicleList _vehicleList;

        public int currentCanvas = 0;
        private string _mapToStart;
        public string mapToStart => this._mapToStart;
        public List<CanvasSelectionItem> canvasSelection = new List<CanvasSelectionItem>();
        public List<ButtonItem> buttonsList = new List<ButtonItem>();
        public List<int> backToCanvas = new List<int>();
        private Dictionary<int, Action> _actionsByCanvasId = new Dictionary<int, Action>();

        private Player player;

        private void Awake() {
            balanceText = GameObject.Find("Balance").GetComponent<TextMeshProUGUI>();
            _vehicleList = GetComponent<VehicleList>();
        }

        private void Start() {
            Game.Run();
            Game.OnGameInitializedEvent += OnGameInitialized;

            this._actionsByCanvasId.Add(0, HandleCanvas0); // MainMenu
            this._actionsByCanvasId.Add(1, HandleCanvas1); // Garage
            this._actionsByCanvasId.Add(2, HandleCanvas2); // Career Selection
            this._actionsByCanvasId.Add(3, HandleCanvas3);
            this._actionsByCanvasId.Add(4, HandleCanvas4);
            this._actionsByCanvasId.Add(5, HandleCanvas5);
            this._actionsByCanvasId.Add(6, HandleCanvas6);
            this._actionsByCanvasId.Add(7, HandleCanvas7);
            this._actionsByCanvasId.Add(8, HandleCanvas8);

            this.preRaceMode = false;
            this.CS = GameObject.Find("cameras").GetComponent<cameraSwitcher>();

            DisableAllCanvases();
            DisableAllButtons();

            ChangeCanvasSection(currentCanvas);
            RefreshGlobalInfo();
        }

        private void OnEnable() {
            VehicleData.onDataLoadedEvent += OnVehicleDataLoaded;
            VehiclesShop.vehicleBuyedEvent += VehicleBuyed;
            Bank.OnBankInitializedEvent += OnBankInitialized;
        }

        private void OnDisable() {
            // VehicleList.vehicleListInitializedEvent -= Subscribe;
            VehicleData.onDataLoadedEvent -= OnVehicleDataLoaded;
            VehiclesShop.vehicleBuyedEvent -= VehicleBuyed;
            Bank.OnBankInitializedEvent -= OnBankInitialized;
        }

        private void OnGameInitialized() {
            Game.OnGameInitializedEvent -= OnGameInitialized;
            var playerInteractor = Game.GetInteractor<PlayerInteractor>();
            var player = playerInteractor.player;
        }

        public void ChangeCanvasSection(int canvasSelectionId) {
            DisableAllButtons();
            canvasSelection[currentCanvas].GO.SetActive(false);

            if(!backButtonCall) {
                backToCanvas.Add(currentCanvas);
            } else {
                backButtonCall = false;
            } 

            currentCanvas = canvasSelectionId;
            canvasSelection[canvasSelectionId].GO.SetActive(true);
            this.CS.CameraTransition(canvasSelection[canvasSelectionId].cameraPosition);

            for(int i = 0; i < canvasSelection[canvasSelectionId].neededButtons.Length; i++) {
                buttonsList[canvasSelection[canvasSelectionId].neededButtons[i]].GO.SetActive(true);
            }

            if(this._actionsByCanvasId.ContainsKey(canvasSelectionId)) {
                this._actionsByCanvasId[canvasSelectionId].Invoke();
            }
        }

        private void HandleCanvas0() {
            this.backToCanvas.Clear();
            this.preRaceMode = false;
            this.shopMode = false;
            this._mapToStart = string.Empty;
            // GetComponent<VehicleSelector>().SetFilterById(_vehicleList.GetPlayerOwnedVehicleIDs().ToArray());
        }

        private void HandleCanvas1() {
            RefreshBuyButton();
            this.shopMode = false;
            if(!this.preRaceMode) {
                GetComponent<VehicleSelector>().SetFilterById(_vehicleList.GetPlayerOwnedVehicleIDs().ToArray());
            } else {
                GetComponent<VehicleSelector>().SetFilterById(GetComponent<MapButtonsData>().mapButtonsList[GetComponent<MapButtonsData>().selectedEvent].allowedVehiclesId);
            }
            RefreshPaintButton();
        }

        private void HandleCanvas2() {
            this.preRaceMode = false;
            this.shopMode = false;
            this._mapToStart = string.Empty;
        }

        private void HandleCanvas3() {

        }

        private void HandleCanvas4() {

        }

        private void HandleCanvas5() {

        }

        private void HandleCanvas6() {

        }

        private void HandleCanvas7() {

        }
        

        private void HandleCanvas8() {
            this.shopMode = true;
            RefreshBuyButton();
            GetComponent<VehicleSelector>().SetFilterById(_vehicleList.GetAllVehicleIDs().ToArray());
        }

        public void ActivateButton(int buttonOn) {
            // ActivateAllButtons();
            // _buttonsList[buttonOn].SetActive(true);
            buttonsList[buttonOn].GO.SetActive(true);

        }

        private void OnVehicleDataLoaded() {
            // RefreshGlobalInfo();
            RefreshPaintButton();
            RefreshBuyButton();
        }

        private void VehicleBuyed() {
            // RefreshGlobalInfo();
            RefreshBuyButton();
            RefreshCurrencyText();
        }

        private void OnBankInitialized() {
            RefreshCurrencyText();
        }

        public void DisableAllCanvases() {
            for(int i = 0; i < canvasSelection.Count; i++) {
                canvasSelection[i].GO.SetActive(false);
            }
        }

        public void BackButton() {
            backButtonCall = true;
            ChangeCanvasSection(backToCanvas[backToCanvas.Count-1]);
            if(backToCanvas.Count > 0) {
                backToCanvas.RemoveAt(backToCanvas.Count-1);
            }
        }

        public void DisableAllButtons() {
            foreach(var button in buttonsList) {
                button.GO.SetActive(false);
            }
        }

        public void StartRaceButton() {
            if(!this.preRaceMode) {
                ChangeCanvasSection(2);
                this._mapToStart = string.Empty;
            } else {
                if(!string.IsNullOrEmpty(this._mapToStart)) {
                    if(Enum.TryParse(this._mapToStart, out mapNameToStart enumValue)) {
                        // Если значение входит в перечисление, можно загружать сцену
                        SceneManager.LoadSceneAsync(this._mapToStart);
                    } else {
                        // Если значение не является именем сцены, выполните соответствующие действия
                        Debug.LogWarning($"'{this._mapToStart}' не является именем сцены.");
                        // Например, можно вывести сообщение об ошибке или выполнить другие действия
                    }
                }
            }
        }

        public void SetMapToStart(string mapToStart) {
            this.preRaceMode = true;
            ChangeCanvasSection(1);
            this._mapToStart = mapToStart;
        }

        private void RefreshGlobalInfo() {
            RefreshCurrencyText();
        }

        private void RefreshPaintButton() {
            int savedVehicleId = PlayerPrefs.GetInt("selectedVehicleId");

            int index = _vehicleList.allVehiclesInGame.FindIndex(vehicle => vehicle.id == savedVehicleId);

            if(index != -1) {
                buttonsList[4].GO.SetActive(shopMode ? false : this._vehicleList.allVehiclesInGame[index].colorCustomization);
                buttonsList[5].GO.SetActive(shopMode ? false : !this._vehicleList.allVehiclesInGame[index].colorCustomization);
            }
        }

        private void RefreshBuyButton() {
            int savedVehicleId = PlayerPrefs.GetInt("selectedVehicleId");

            int index = _vehicleList.allVehiclesInGame.FindIndex(vehicle => vehicle.id == savedVehicleId);

            if(index != -1) {
                bool isOwned = _vehicleList.allVehiclesInGame[index].isOwned;
                
                Transform priceTextTransform = buttonsList[9].GO.transform.Find("PriceText (TMP)");

                if (priceTextTransform != null) {
                    TextMeshProUGUI priceText = priceTextTransform.GetComponent<TextMeshProUGUI>();
                    if (priceText != null) {
                        priceText.text = FormatPrice(_vehicleList.allVehiclesInGame[index].price) + " Qbit";
                    }
                }

                buttonsList[9].GO.SetActive(!isOwned);
            }
        }

        private string FormatPrice(int price) {
            return price.ToString("N0").Replace(",", " ");
        }

        private void RefreshCurrencyText() {
            if(!Bank.isInitialized) {
                return;
            }
            balanceText.text = Bank.GetCurrencyAmount(CurrencyType.Qbit).ToString() + " Q";
            Debug.Log("Bank refreshed");
        }
    }
}


[System.Serializable]
public class CanvasSelectionItem {
    public string name;
    public GameObject GO;
    public int cameraPosition;
    public int[] neededButtons;
}

[System.Serializable]
public class ButtonItem {
    public string name;
    public GameObject GO;
    public string location;
    public string function;
}