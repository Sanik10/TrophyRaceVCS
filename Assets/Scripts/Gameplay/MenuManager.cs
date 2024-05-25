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

        public static event Action MainMenuOpenedEvent;
        public static event Action GarageOpenedEvent;
        public static event Action CareerSelectionOpenedEvent;
        public static event Action SettingsOpenedEvent;
        public static event Action VehicleCustomizationOpenedEvent;
        public static event Action VehiclesShopOpenedEvent;

        private GameObject _HomeButton;
        private GameObject _SettingsButton;
        private GameObject _StartRaceButton;

        public bool preRaceMode = false;
        public bool shopMode = false;


        private cameraSwitcher CS;
        [SerializeField]
        private TextMeshProUGUI balanceText;
        private bool backButtonCall = false;
        private VehicleSpawner _vehicleSpawner;
        [SerializeField]
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
            // balanceText = GameObject.Find("Balance").GetComponent<TextMeshProUGUI>();
            _vehicleList = FindObjectOfType<VehicleList>();
            if(this._vehicleList == null) {
                SceneManager.LoadScene("Starter");
            }
        }

        private void Start() {
            Game.Run();
            this._actionsByCanvasId.Add(0, MainMenuOpener); // MainMenu
            this._actionsByCanvasId.Add(1, GarageOpener); // Garage
            this._actionsByCanvasId.Add(2, CareerSelectionOpener); // Career Selection
            this._actionsByCanvasId.Add(3, SettingsOpener);
            this._actionsByCanvasId.Add(4, ClassPoolMenuOpener);
            this._actionsByCanvasId.Add(5, VehicleCustomizationOpener);
            this._actionsByCanvasId.Add(6, VehicleBodyPaintOpener);
            this._actionsByCanvasId.Add(7, VehicleDiskPaintOpener);
            this._actionsByCanvasId.Add(8, VehiclesShopOpener);

            this.preRaceMode = false;
            this.CS = GameObject.Find("cameras").GetComponent<cameraSwitcher>();

            DisableAllCanvases();

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

            DisableAllButtons();
            for(int i = 0; i < canvasSelection[canvasSelectionId].neededButtons.Length; i++) {
                buttonsList[canvasSelection[canvasSelectionId].neededButtons[i]].GO.SetActive(true);
            }

            if(this._actionsByCanvasId.ContainsKey(canvasSelectionId)) {
                this._actionsByCanvasId[canvasSelectionId].Invoke();
            }
        }

        private void MainMenuOpener() {
            this.backToCanvas.Clear();
            this.preRaceMode = false;
            this.shopMode = false;
            this._mapToStart = string.Empty;
            MainMenuOpenedEvent?.Invoke();
            // GetComponent<VehicleSelector>().SetFilterById(_vehicleList.GetPlayerOwnedVehicleIDs().ToArray());
        }

        private void GarageOpener() {
            RefreshBuyButton();
            this.shopMode = false;
            if(!this.preRaceMode) {
                GetComponent<VehicleSelector>().SetFilterByGuid(_vehicleList.GetPlayerOwnedVehiclesGuid().ToArray());
            } else {
                GetComponent<VehicleSelector>().SetFilterByGuid(GetComponent<MapButtonsData>().mapButtonsList[GetComponent<MapButtonsData>().selectedEvent].allowedVehiclesGuid);
            }
            RefreshPaintButton();
            GarageOpenedEvent?.Invoke();
        }

        private void CareerSelectionOpener() {
            this.preRaceMode = false;
            this.shopMode = false;
            this._mapToStart = string.Empty;
            CareerSelectionOpenedEvent?.Invoke();
        }

        private void SettingsOpener() {
            SettingsOpenedEvent?.Invoke();
        }

        private void VehicleCustomizationOpener() {
            VehicleCustomizationOpenedEvent?.Invoke();
        }

        private void VehicleBodyPaintOpener() {

        }

        private void VehicleDiskPaintOpener() {

        }

        private void ClassPoolMenuOpener() {

        }
        

        private void VehiclesShopOpener() {
            this.shopMode = true;
            RefreshBuyButton();
            GetComponent<VehicleSelector>().SetFilterByGuid(_vehicleList.GetAllVehiclesGuid().ToArray());
            VehiclesShopOpenedEvent?.Invoke();
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
            if(currentCanvas == 0) return;
            string savedVehicleGuid = PlayerPrefs.GetString("selectedVehicleGuid");

            int index = _vehicleList.allVehiclesInGame.FindIndex(vehicle => vehicle.guid == savedVehicleGuid);

            if(index != -1) {
                buttonsList[4].GO.SetActive(shopMode ? false : this._vehicleList.allVehiclesInGame[index].colorCustomization);
                buttonsList[5].GO.SetActive(shopMode ? false : !this._vehicleList.allVehiclesInGame[index].colorCustomization);
            }
        }

        private void RefreshBuyButton() {
            string savedVehicleGuid = PlayerPrefs.GetString("selectedVehicleGuid");

            int index = _vehicleList.allVehiclesInGame.FindIndex(vehicle => vehicle.guid == savedVehicleGuid);
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