using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FramerateSettingsSection : MonoBehaviour {

    [SerializeField]
    private Toggle[] _togglesList;
    [SerializeField]
    private GameObject[] _toggleTexts;
    private bool[] _toggleCondition = new bool[4];
    private bool _toggleInWork = false;
    [SerializeField]
    private int synchCount = 0;

    [SerializeField]
    private List<FpsSettingButtonItem> _buttonsList = new List<FpsSettingButtonItem>();
    private int _currentFpsButton = 1;
    private int _targetFps = 30;

    private void Start() {
        this._toggleCondition = new bool[4];
        for(int i = 0; i < 4; i++) {
            _toggleTexts[i].SetActive(false);
        }

        if(PlayerPrefs.HasKey("FrameRatePrefs")) {
            this._targetFps = PlayerPrefs.GetInt("FrameRatePrefs");
        }

        for(int i = 0; i < this._buttonsList.Count-1; i++) {
            if(this._buttonsList[i].fps == this._targetFps) {
                this._currentFpsButton = i;
                ButtonColorManager.SetSelectedColor(this._buttonsList[i].button);
                this._buttonsList[i].button.interactable = false;
                break;
            }
        }

        if(PlayerPrefs.HasKey("SynchCountPrefs")) {
            synchCount = PlayerPrefs.GetInt("SynchCountPrefs");
            if(synchCount != 0) {
                this._togglesList[synchCount-1].isOn = true;
                this._toggleTexts[synchCount-1].SetActive(true);
            }
        }
    }

    public void EnableVSync(bool toggleOn) {
        if(_toggleInWork) {
            return;
        }
        _toggleInWork = true;
        if(synchCount != 0) {
            this._toggleTexts[synchCount-1].SetActive(false);
        }
        for(int i = 0; i < this._togglesList.Length; i++) {
            if(this._togglesList[i].isOn && this._togglesList[i] != this._toggleCondition[i]) {
                for(int q = i; q > -1; q--) {
                    if(!this._togglesList[q].isOn) {
                        this._togglesList[q].isOn = true;
                    }
                    this._toggleCondition[q] = this._togglesList[q].isOn;
                }
                break;
            }
        }

        for(int i = 0; i < this._togglesList.Length; i++) {
            if(!this._togglesList[i].isOn && i != 4) {
                for(int q = i; q < this._togglesList.Length; q++) {
                    if(this._togglesList[q].isOn) {
                        this._togglesList[q].isOn = false;
                    }
                    this._toggleCondition[q] = this._togglesList[q].isOn;
                }
                break;
            }
        }

        synchCount = 0;
        for(int i = 0; i < this._togglesList.Length; i++) {
            if(this._toggleCondition[i]) {
                synchCount = i+1;
            }
        }
        
        PlayerPrefs.SetInt("SynchCountPrefs", synchCount);
        if(synchCount != 0) {
            this._toggleTexts[synchCount-1].SetActive(true);
        }
        QualitySettings.vSyncCount = synchCount;
        Debug.Log("Screen synch per frame: " + synchCount);
        _toggleInWork = false;
    }

    public void SetFps(int value) {
        int buttonIndex = 0;
        ButtonColorManager.SetNormalColor(this._buttonsList[this._currentFpsButton].button);
        this._buttonsList[this._currentFpsButton].button.interactable = true;
        for(int i = 0; i < this._buttonsList.Count; i++) {
            if(this._buttonsList[i].fps == value) {
                this._currentFpsButton = i;
                ButtonColorManager.SetSelectedColor(this._buttonsList[i].button);
                this._buttonsList[i].button.interactable = false;
                break;
            }
        }
        PlayerPrefs.SetInt("FrameRatePrefs", value);
        Application.targetFrameRate = value;
        Debug.Log("Frame rate : " + value);
    }


}

[System.Serializable]
public class FpsSettingButtonItem {
    public string name;
    public Button button;
    public int fps;
}