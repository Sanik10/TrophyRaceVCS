using UnityEngine;
using System;
using System.Collections.Generic;
using TrophyRace.Architecture;

public class RaceTimer : MonoBehaviour {

    private bool raceStarted = false;
    private bool racePaused = false;
    private float uncountedTime = 0;
    private Dictionary<GameObject, CarData> _carTimes = new Dictionary<GameObject, CarData>();
    public Dictionary<GameObject, CarData> carTimes => _carTimes;

    private void OnEnable() {
        StartFinishLine.startFinishLinePassedEvent += FinishLinePassed;
        GameManager.StartRaceEvent += StartRace;
    }

    private void OnDisable() {
        StartFinishLine.startFinishLinePassedEvent -= FinishLinePassed;
        GameManager.StartRaceEvent -= StartRace;
    }

    private void Update() {
        if(raceStarted && !racePaused) {
            foreach (var kvp in _carTimes) {
                CarData carData = kvp.Value;
                carData.raceTime = Time.timeSinceLevelLoad - uncountedTime;
                carData.lapTime = Time.timeSinceLevelLoad - carData.lapStartTime;
            }
        }
    }

    public void RegisterCar(GameObject car) {
        if(!_carTimes.ContainsKey(car)) {
            _carTimes.Add(car, new CarData());
        }
    }

    public void FinishLinePassed(GameObject car) {
        if(_carTimes.ContainsKey(car)) {
            CarData carData = _carTimes[car];
            carData.lapStartTime = Time.timeSinceLevelLoad;
            carData.lapTime = 0f;
            carData.lapCount++;
            Debug.Log($"Круг: {carData.lapCount}, время гонки: {carData.raceTime}, время круга: {carData.lapTime}");
        }
    }

    public void StartRace() {
        uncountedTime = Time.timeSinceLevelLoad;
        raceStarted = true;
        racePaused = false;

        foreach(var carData in _carTimes.Values) {
            carData.raceTime = 0f;
            carData.lapTime = 0f;
            carData.lapCount = 0;
        }
    }

    public void PauseResumeRace() {
        racePaused = !racePaused;
    }
}

[System.Serializable]
public class CarData {
    public float raceTime;
    public float lapTime;
    public float lapStartTime;
    public int lapCount;
}