using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TrophyRace.Architecture {
    public class VehicleBodyPainter : MonoBehaviour {
        
        private VehicleManager _VehicleManager;
        private VehicleSpawner _VehicleSpawner;
        [SerializeField]
        private GameObject SettingButtonsContainer;
        [SerializeField]
        private GameObject SettingButtonPrefab;
        [SerializeField]
        private GameObject ButtonsHolder;
        [SerializeField]
        private GameObject ScrollView;
        [SerializeField]
        public int currentCustomizationWindow = 0;
        private int buttonSize = 90;


        public List<CustomizationSubsectionItem> customizationSubsection = new List<CustomizationSubsectionItem>();
        
        private void Start() {
            ScrollView.SetActive(false);
            ButtonsHolder.SetActive(false);
            _VehicleSpawner = GameObject.Find("scripts").GetComponent<VehicleSpawner>();
        }

        private void GetVehicleManager() {
            _VehicleManager = _VehicleSpawner.playerVehicle.GetComponent<VehicleManager>();
        }

        public void SetActiveScrollView(int i) {
            ButtonsHolder.SetActive((i == 0) ? true : false);
            ScrollView.SetActive((i == 1) ? true : false);
        }

        public void CreatePaintButtons(int paintType) {
            GetVehicleManager();
            if(_VehicleManager.vehicleData.colorCustomization == true) {
                SetActiveScrollView(1);
                currentCustomizationWindow = paintType;
                for(int i = 0; i < _VehicleManager.VehicleVFX.colored[paintType].material.Length; i++) { //int i = 0; i < _VehicleManager.VehicleVFX.colorsCount[paintType]; i++
                    Instantiate(SettingButtonPrefab, SettingButtonsContainer.GetComponent<Transform>());
                    SettingButtonsContainer.GetComponent<Transform>().GetChild(i).GetComponent<RectTransform>().sizeDelta = new Vector2(buttonSize, buttonSize);
                    SettingButtonsContainer.GetComponent<Transform>().GetChild(i).GetComponent<Image>().color = _VehicleManager.VehicleVFX.colored[paintType].material[i].color;
                    // SettingButtonsContainer.GetComponent<Transform>().GetChild(i).GetComponent<Image>().material = _VehicleManager.VehicleVFX.colored[paintType].material[i];
                }
            }
        }

        public void DestroyAllPaintButtons() {
            GetVehicleManager();
            if(_VehicleManager.vehicleData.colorCustomization == true) {
                SetActiveScrollView(0);
                Transform[] children = SettingButtonsContainer.GetComponentsInChildren<Transform>();
                if(children != null) {
                    foreach(Transform child in children) {
                        if(child != SettingButtonsContainer.transform) {
                            Destroy(child.gameObject);
                        }
                    }
                }
            }
        }
    }
}

[System.Serializable]
public class CustomizationSubsectionItem {
    public string name;
    public int id;
    public int buttonRound;
    public Material[] paintColor;
}