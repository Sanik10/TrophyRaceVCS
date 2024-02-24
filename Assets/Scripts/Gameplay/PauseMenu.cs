using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour {

    public GameObject settingsMenu;
    public GameObject pauseMenu;
    public Slider GlobalVolumeSlider;
    public Slider VehicleVolumeSlier;
    public Slider MusicVolumeSlider;
    private bool _pauseGame = false;
    private bool _settingsMenu = false;

    private void Start() {
        // GlobalVolumeSlider.value = Mathf.Pow(10f, PlayerPrefs.GetFloat("GlobalVolume"));
        // VehicleVolumeSlier.value = Mathf.Pow(10f, PlayerPrefs.GetFloat("VehicleVolume"));
        // MusicVolumeSlider.value = Mathf.Pow(10f, PlayerPrefs.GetFloat("MusicVolume"));
        settingsMenu = GameObject.Find("SettingsMenu");
        settingsMenu.SetActive(false);
        pauseMenu = GameObject.Find("PauseMenu");
        pauseMenu.SetActive(false);
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.Escape)) {
            if(!_pauseGame) {
                Pause();
            } else {
                Resume();
            }
        }
    }

    public void Pause() {
        pauseMenu.SetActive(true);
        _pauseGame = true;
        Time.timeScale = 0;
    }

    public void Resume() {
        pauseMenu.SetActive(false);
        settingsMenu.SetActive(false);
        _pauseGame = false;
        _settingsMenu = false;
        Time.timeScale = 1;
    }

    public void ExitRace() {
        Time.timeScale = 1;
        SceneManager.LoadSceneAsync("MainMenu");
    }

    public void OpenSettingsMenu() {
        if(!_settingsMenu) {
            settingsMenu.SetActive(true);
            _settingsMenu = true;
        } else {
            settingsMenu.SetActive(false);
            _settingsMenu = false;
        }
    }
}