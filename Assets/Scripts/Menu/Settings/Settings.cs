using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Settings : MonoBehaviour {

    [Header("Buttons for Settings Sections")]
    [SerializeField] private Button[] buttonsSections;

    [Header("Settings Sections Screens")]
    [SerializeField] private GameObject[] settingsSections;
    private int currentSectionIndex = 0;

    private void Start() {
        foreach (GameObject section in settingsSections) {
            section.SetActive(false);
        }
        settingsSections[currentSectionIndex].SetActive(true);
        ButtonColorManager.SetSelectedColor(buttonsSections[currentSectionIndex]);
        buttonsSections[currentSectionIndex].interactable = false;

    }

    public void ChangeSettingSection(int sectionIndex) {
        ButtonColorManager.SetNormalColor(buttonsSections[currentSectionIndex]);
        settingsSections[currentSectionIndex].SetActive(false);
        buttonsSections[currentSectionIndex].interactable = true;

        currentSectionIndex = sectionIndex;

        settingsSections[currentSectionIndex].SetActive(true);
        ButtonColorManager.SetSelectedColor(buttonsSections[currentSectionIndex]);
        buttonsSections[currentSectionIndex].interactable = false;
    }



    /*public bool applySettings = false;
    public int currentSettingsSubsection = 0;
    public ScrollRect _scrollRect;
    // private List<GameObject> _SubsectionButtonsContainer;
    // private Button btn;

    [Header("Subsections")]
    public List<SubsectionItem> subsections = new List<SubsectionItem>();

    [Header("Quality Level")]
    public List<QualitySettingsItem> qualities = new List<QualitySettingsItem>();

    [Header("Display")]
    public List<ResolutionItem> resolutions = new List<ResolutionItem>();
    public Toggle fullScreenToggle;

    [Header("Frame Rate")]
    public List<FrameRateItem> frameRates = new List<FrameRateItem>();
    public Toggle[] Toggle;
    public bool[] toggleCondition;
    public int synchCount;

    private void Start() {
        int startIndex = 0;
        if(PlayerPrefs.HasKey("QualityPrefs")) {
            startIndex = PlayerPrefs.GetInt("QualityPrefs");
            QualitySettings.SetQualityLevel(qualities[startIndex].Level, qualities[startIndex].ApplyExpensiveChanges);
        }
        if(PlayerPrefs.HasKey("FrameRatePrefs")) {
            startIndex = PlayerPrefs.GetInt("FrameRatePrefs");
            Application.targetFrameRate = frameRates[startIndex].FrameRate;
        }
        if(applySettings) {
            if(PlayerPrefs.HasKey("ResolutionPrefs")) {
                bool fullScreenFlag = true;
                startIndex = PlayerPrefs.GetInt("ResolutionPrefs");
                if(PlayerPrefs.HasKey("FullScreenPrefs")) {
                    fullScreenToggle.isOn = fullScreenFlag = (PlayerPrefs.GetInt("FullScreenPrefs") == 1) ? true : false;
                }
                Screen.SetResolution(resolutions[startIndex].Width, resolutions[startIndex].Height, fullScreenFlag);
            }
            if(PlayerPrefs.HasKey("SynchCountPrefs")) {
                synchCount = PlayerPrefs.GetInt("SynchCountPrefs");
                if(synchCount != 0) {
                    Toggle[synchCount-1].isOn = true;
                }
            }
        }
        if(_scrollRect == null) {
            _scrollRect = GameObject.Find("GeneralPart").GetComponent<ScrollRect>();
        }
        // _SubsectionButtonsContainer = new List<GameObject>();
        // _SubsectionButtonsContainer.Add(_ResolutionSettingContainer);
        CreateResolutionSubsection();
    }

    private void CreateResolutionSubsection() {
        subsections[currentSettingsSubsection]._SettingViewport.SetActive(true);
        _scrollRect.content = subsections[currentSettingsSubsection]._SettingButtonsContainer.GetComponent<RectTransform>();
        if(subsections[currentSettingsSubsection].created != true) {
            for(int i = 0; i < subsections[currentSettingsSubsection].buttonsCount; i++) {
                Instantiate(subsections[currentSettingsSubsection]._SettingButtonPrefab, subsections[currentSettingsSubsection]._SettingButtonsContainer.GetComponent<Transform>());
                subsections[currentSettingsSubsection]._SettingButtonsContainer.GetComponent<Transform>().GetChild(i).GetComponent<RectTransform>().sizeDelta = new Vector2(500, subsections[currentSettingsSubsection].buttonHeight);
                TextMeshProUGUI btnText = subsections[currentSettingsSubsection]._SettingButtonsContainer.GetComponent<Transform>().GetChild(i).GetComponent<Transform>().GetChild(0).GetComponent<TextMeshProUGUI>();
                switch(currentSettingsSubsection) {
                    case 0 :
                        btnText.alignment = TextAlignmentOptions.Center;
                        btnText.text = qualities[i].Level + " уровень детализации" + " (" + qualities[i].Description + ")";
                    break;

                    case 1 :
                        btnText.alignment = TextAlignmentOptions.Center;
                        btnText.text = resolutions[i].Width + " x " + resolutions[i].Height + " (" + resolutions[i].Description + ")";
                        if(resolutions[i].Standard == true) {
                            btnText.fontStyle = FontStyles.Bold | FontStyles.Underline;
                        }
                    break;

                    case 2 :
                        btnText.alignment = TextAlignmentOptions.Center;
                        btnText.text = frameRates[i].FrameRate + "Hz";
                        if(frameRates[i].Standard == true) {
                            btnText.fontStyle = FontStyles.Bold | FontStyles.Underline;
                        }
                    break;
                }
            }
            subsections[currentSettingsSubsection].created = true;
        }
    }

    #region WindowCreater
    // private void CreateSubsectionWindow(GameObject SettingButton, GameObject SettingButtonsContainer, int ChildCount, int ButtonHeight) {
    //     for(int i = 0; i < this.ChildCount; i++) {
    //         Instantiate(this.SettingButton, this.SettingButtonsContainer.GetComponent<Transform>());
    //         SettingButtonsContainer.GetComponent<Transform>().GetChild(i).GetComponent<RectTransform>().sizeDelta = new Vector2(500, ButtonHeight);
    //         TextMeshProUGUI ButtonText = SettingButtonsContainer.GetComponent<Transform>().GetChild(i).GetComponent<Transform>().GetChild(0).GetComponent<TextMeshProUGUI>();
    //     }
    // }

    // private void CreateResolutionSubsection() {
    //     for(int i = 0; i < this.resolutions.Count; i++) {
    //         Instantiate(this._SettingButton, this._ResolutionSettingContainer.GetComponent<Transform>());
    //         _ResolutionSettingContainer.GetComponent<Transform>().GetChild(i).GetComponent<RectTransform>().sizeDelta = new Vector2(500, buttonHeight);
    //         TextMeshProUGUI btnText = _ResolutionSettingContainer.GetComponent<Transform>().GetChild(i).GetComponent<Transform>().GetChild(0).GetComponent<TextMeshProUGUI>();
    //         btnText.alignment = TextAlignmentOptions.Center;
    //         btnText.text = resolutions[i].Width + " x " + resolutions[i].Height + " (" + resolutions[i].Description + ")";
    //         if(resolutions[i].Standard == true) {
    //             btnText.fontStyle = FontStyles.Bold | FontStyles.Underline;
    //         }
    //     }
    // }

    // private void CreateFrameRateSubsection() {
    //     for(int i = 0; i < this.frameRates.Count; i++) {
    //         Instantiate(this._SettingButton, this._ResolutionSettingContainer.GetComponent<Transform>());
    //         _ResolutionSettingContainer.GetComponent<Transform>().GetChild(i).GetComponent<RectTransform>().sizeDelta = new Vector2(500, buttonHeight);
    //         TextMeshProUGUI btnText = _ResolutionSettingContainer.GetComponent<Transform>().GetChild(i).GetComponent<Transform>().GetChild(0).GetComponent<TextMeshProUGUI>();
    //         btnText.alignment = TextAlignmentOptions.Center;
    //         btnText.text = resolutions[i].Width + " x " + resolutions[i].Height + " (" + resolutions[i].Description + ")";
    //         if(resolutions[i].Standard == true) {
    //             btnText.fontStyle = FontStyles.Bold | FontStyles.Underline;
    //         }
    //     }
    // }
    #endregion

    public void ChangeSubsectionWindow(int index) {
        subsections[currentSettingsSubsection]._SettingViewport.SetActive(false);
        currentSettingsSubsection = index;
        CreateResolutionSubsection();
    }

    public void GetButtonNumber(Button btn) {
        Transform parent = btn.transform.parent.transform;
        Transform pressedButton = btn.transform;
        int i = 0;
        foreach(Transform child in parent) {
            if(child == pressedButton) {
                switch(currentSettingsSubsection) {
                    case 0 :
                        ChangeQuality(i);
                    break;

                    case 1 :
                        ChangeResolution(i);
                    break;

                    case 2 :
                        ChangeFrameRate(i);
                    break;

                }
                // CallFunction(i);
                break;
            }
            i++;
        }
    }

    // public void CallFunction(int i) {
    //     if(currentSettingsSubsection == 1) {
    //         ChangeResolution1(i);
    //     }
    //     if(currentSettingsSubsection == 2) {
    //         ChangeResolution2(i);
    //     }
    //     if(currentSettingsSubsection == 3) {
    //         ChangeResolution3(i);
    //     }
    // }

    public void ChangeQuality(int index) {
        QualitySettings.SetQualityLevel(qualities[index].Level, qualities[index].ApplyExpensiveChanges);
        PlayerPrefs.SetInt("QualityPrefs", index);
    }

    public void ChangeResolution(int index) {
        Screen.SetResolution(resolutions[index].Width, resolutions[index].Height, ((PlayerPrefs.GetInt("FullScreenPrefs") == 1) ? true : false));
        PlayerPrefs.SetInt("ResolutionPrefs", index);
        Debug.Log("Screen resolution : " + resolutions[index].Width + " x " + resolutions[index].Height);
    }

    public void SetFullScreen(bool Fsc) {
        Screen.fullScreen = Fsc;
        PlayerPrefs.SetInt("FullScreenPrefs", (Fsc == true) ? 1 : 0);
    }

    public void enableVSync(bool toggleOn) {
        for(int i = 0; i < Toggle.Length; i++) {
            if(Toggle[i].isOn && Toggle[i] != toggleCondition[i]) {
                if(i != 0) {
                    for(int q = i; q > -1; q--) {
                        if(!Toggle[q].isOn) {
                            Toggle[q].isOn = true;
                        }
                        toggleCondition[q] = Toggle[q].isOn;
                    }
                }
            }
        }
        for(int i = 0; i < Toggle.Length; i++) {
            if(!Toggle[i].isOn) {
                if(i != 4) {
                    for(int q = i; q < Toggle.Length; q++) {
                        if(Toggle[q].isOn) {
                            Toggle[q].isOn = false;
                        }
                        toggleCondition[q] = Toggle[q].isOn;
                    }
                }
            }
        }
        synchCount = 0;
        for(int i = 0; i < Toggle.Length; i++) {
            if(toggleCondition[i]) {
                synchCount = i+1;
            }
        }
        
        PlayerPrefs.SetInt("SynchCountPrefs", synchCount);
        Debug.Log("Screen synch per frame: " + synchCount);
    }

    public void ChangeFrameRate(int index) {
        Application.targetFrameRate = frameRates[index].FrameRate;
        PlayerPrefs.SetInt("FrameRatePrefs", index);
        Debug.Log("Frame rate : " + frameRates[index].FrameRate);
    }


}

[System.Serializable]
public class SubsectionItem {
    public string name;
    public int buttonHeight = 100;
    public GameObject _SettingButtonPrefab;
    public int buttonsCount;
    public GameObject _SettingViewport;
    public GameObject _SettingButtonsContainer;
    public bool created = false;
}

[System.Serializable]
public class QualitySettingsItem {
    public int Level;
    public string Description;
    public bool ApplyExpensiveChanges = false;
}

[System.Serializable]
public class ResolutionItem {
    public int Width, Height;
    public string Description;
    public bool Standard;
}

[System.Serializable]
public class FrameRateItem {
    public int FrameRate;
    public bool Standard;
    */
}