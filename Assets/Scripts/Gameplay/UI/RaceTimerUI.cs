using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using TrophyRace.Architecture;

public class RaceTimerUI : MonoBehaviour {

    private RaceTimer _raceTimer;
    private GameObject _playerCar;
    private TextMeshProUGUI lapTimeText;
    private TextMeshProUGUI totalRaceTimeText;

    // Start is called before the first frame update
    private void Start() {
        this._raceTimer = GameObject.Find("scripts").GetComponent<RaceTimer>();
        this._playerCar = GameObject.Find("scripts").GetComponent<VehicleSpawner>().playerVehicle;
        Transform lapTimeObject = transform.Find("LapTimeText");
        Transform totalRaceTimeObject = transform.Find("RaceTimeText");

        // Получаем компоненты TextMeshProUGUI
        lapTimeText = lapTimeObject.GetComponent<TextMeshProUGUI>();
        totalRaceTimeText = totalRaceTimeObject.GetComponent<TextMeshProUGUI>();

        // Проверяем, были ли найдены компоненты
        if (lapTimeText == null || totalRaceTimeText == null) {
            Debug.LogError("TextMeshProUGUI components not found!");
        }
    }

    // Update is called once per frame
    private void Update() {
        if (this._raceTimer != null) {
            lapTimeText.text = $"{FormatTime(this._raceTimer.carTimes[_playerCar].lapTime)} lap time";

            totalRaceTimeText.text = $"{FormatTime(this._raceTimer.carTimes[_playerCar].raceTime)} race time";
        }
    }

    private string FormatTime(float time) {
        int minutes = (int)(time / 60f);
        int seconds = (int)(time % 60f);
        int milliseconds = (int)((time * 1000f) % 1000);

        return $"{minutes:D2}:{seconds:D2}:{milliseconds:D3}";
    }
}