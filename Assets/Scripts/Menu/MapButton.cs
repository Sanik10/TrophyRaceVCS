using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TrophyRace.Architecture {

    public class MapButton : MonoBehaviour {

        [SerializeField]
        private MapButtonsData _MBD;

        [SerializeField]
        private int _id;

        [SerializeField]
        private Image _image;

        [SerializeField]
        private TextMeshProUGUI _mapName;

        [SerializeField]
        private GameObject _greenBorder;

        [SerializeField]
        private string _mapToStart;

        [SerializeField]
        private Sprite[] _awardSprite;

        [SerializeField]
        private Image[] _awardImage;

        private int _posInList;

        private void OnEnable() {
            // MapButtonsData.MapButtonsDataLoadedEvent += RefreshButton;
            // MapButtonsData.MapButtonsDataLoadedEvent -= RefreshButton;
            MenuManager.CareerSelectionOpenedEvent += RefreshButton;
        }

        private void Awake() {
            if(this._image == null) {
                foreach(Transform i in gameObject.transform) {
                    if(i.transform.name == "MapButtonImage") {
                        this._image = i.GetComponent<Image>(); 
                    }
                }
            }
            this._MBD = GameObject.Find("scripts").GetComponent<MapButtonsData>();
        }

        private void Start() {
            RefreshButton();
        }

        private void FindPosInMapButtonsList() {
            bool loaded = false;
            for(int i = 0; i < _MBD.mapButtonsList.Count; i++) {
                if(_MBD.mapButtonsList[i].id == this._id) {
                    this._posInList = i;
                    loaded = true;
                    break;
                }
            }

            if(!loaded) {
                Debug.LogError($"Не найдена позиция для кнопки с айди {_id}");
            }
        }

        public void RefreshButton() {
            if(this._MBD == null) {
                this._MBD = GameObject.Find("scripts").GetComponent<MapButtonsData>();
            }
            FindPosInMapButtonsList();
            this._greenBorder.SetActive(!this._MBD.mapButtonsList[this._posInList].openedMap);
            this._image.sprite = this._MBD.mapButtonsList[this._posInList].sprite;
            this._mapName.text = this._MBD.mapButtonsList[this._posInList].mapName;
            for(int i = 0; i < 5; i++) {
                if(i < this._MBD.mapButtonsList[this._posInList].openedAwards) {
                    _awardImage[i].sprite = this._awardSprite[1];
                } else {
                    _awardImage[i].sprite = this._awardSprite[0];
                }
            }
            this._mapToStart = this._MBD.mapButtonsList[this._posInList].mapToStart.ToString();
        }

        public void StartRace() {
            this._MBD.mapButtonsList[this._posInList].openedMap = true;
            this._MBD.selectedEvent = this._posInList;
            this._MBD.SaveCurrintButton(this._posInList);
            RefreshButton();
            GameObject.Find("scripts").GetComponent<VehicleSelector>().SetFilterById(this._MBD.mapButtonsList[this._posInList].allowedVehiclesId);
            GameObject.Find("scripts").GetComponent<MenuManager>().SetMapToStart(this._mapToStart);
            Debug.Log(_mapToStart);
        }

        private void OnDestroy() {
            MenuManager.CareerSelectionOpenedEvent -= RefreshButton;
        }
    }
}