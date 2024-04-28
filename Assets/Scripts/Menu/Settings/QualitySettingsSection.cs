using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QualitySettingsSection : MonoBehaviour {

    [SerializeField]
    private Button[] _QualityButtons;
    private int _selectedIndex = 2;
    private bool _gfxBoost = false;

    private void Start() {
        this._selectedIndex = QualitySettings.GetQualityLevel();
        ButtonColorManager.SetSelectedColor(this._QualityButtons[this._selectedIndex]);
        this._QualityButtons[this._selectedIndex].interactable = false;
    }

    public void ChangeQualityLevel(int qualityLevel) {
        ButtonColorManager.SetNormalColor(this._QualityButtons[this._selectedIndex]);
        this._QualityButtons[this._selectedIndex].interactable = true;
        this._selectedIndex = qualityLevel;
        PlayerPrefs.SetInt("QualityPrefs", qualityLevel);
        ApplyQualityLevel();
    }

    private void ApplyQualityLevel() {
        QualitySettings.SetQualityLevel(this._selectedIndex, this._gfxBoost);
        ButtonColorManager.SetSelectedColor(this._QualityButtons[this._selectedIndex]);
        this._QualityButtons[this._selectedIndex].interactable = false;
    }
}