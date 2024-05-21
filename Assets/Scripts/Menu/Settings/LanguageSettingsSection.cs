using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;

public class LanguageSettingsSection : MonoBehaviour {

    [SerializeField]
    private Button[] _languageButtons;
    [SerializeField]
    private int _selectedIndex = 0;
    private bool _inWork = false;

    private void Start() {
        if(PlayerPrefs.HasKey("LanguagePrefs")) {
            this._selectedIndex = PlayerPrefs.GetInt("LanguagePrefs");
            // StartCoroutine(SetLocale(this._selectedIndex));
        }
        ButtonColorManager.SetSelectedColor(this._languageButtons[this._selectedIndex]);
        this._languageButtons[this._selectedIndex].interactable = false;
    }

    public void ChangeLocale(int localeId) {
        if(this._inWork) return;
        PlayerPrefs.SetInt("LanguagePrefs", localeId);
        ButtonColorManager.SetNormalColor(this._languageButtons[this._selectedIndex]);
        this._languageButtons[this._selectedIndex].interactable = true;
        this._selectedIndex = localeId;
        StartCoroutine(SetLocale(localeId));
    }

    IEnumerator SetLocale(int _localeId) {
        this._inWork = true;
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[_localeId];
        ButtonColorManager.SetSelectedColor(this._languageButtons[this._selectedIndex]);
        this._languageButtons[this._selectedIndex].interactable = false;
        this._inWork = false;
    }
}