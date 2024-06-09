using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QualitySettingsSection : MonoBehaviour {

    [SerializeField]
    private Button[] _qualityButtons;
    [SerializeField]
    private int _selectedIndex = 2;

    private void Start() {
        this._selectedIndex = QualitySettings.GetQualityLevel();
        ButtonColorManager.SetSelectedColor(this._qualityButtons[this._selectedIndex]);
        this._qualityButtons[this._selectedIndex].interactable = false;
    }

    public void ChangeQualityLevel(int qualityLevel) {
        PlayerPrefs.SetInt("QualityPrefs", qualityLevel);
        ButtonColorManager.SetNormalColor(this._qualityButtons[this._selectedIndex]);
        this._qualityButtons[this._selectedIndex].interactable = true;    
        this._selectedIndex = qualityLevel;
        ApplyQualityLevel();
    }

    private void ApplyQualityLevel() {
        QualitySettings.SetQualityLevel(this._selectedIndex, false);

        ButtonColorManager.SetSelectedColor(this._qualityButtons[this._selectedIndex]);
        this._qualityButtons[this._selectedIndex].interactable = false;
    }
}