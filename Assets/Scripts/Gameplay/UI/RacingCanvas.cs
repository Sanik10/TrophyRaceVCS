using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class RacingCanvas : MonoBehaviour {
    
    private VehicleManager VehicleManager;
    private GameObject TachometrAnalog;
    private GameObject TachometrElectro;
    private GameObject Needle;
    private Text SpeedText;
    private Text GearText;
    private Text RaceTimeText;
    private TextMeshProUGUI _AllMileage;
    private TextMeshProUGUI _CurrentMileage;

    public bool stopTimer = true;
    public bool reset = false;
    public bool electroTachometr;

    private float endAngle = -90;
    private float startAngle;
    private float desiredPosition;
    private float realTime;
    private float raceTime;
    private float whileStop;
    private int milliseconds;
    private int seconds;
    private int minutes;
    private int hours;

    private void Start() {
        SpeedText = GameObject.Find("SpeedText").GetComponent<Text>() as Text;
        RaceTimeText = GameObject.Find("RaceTimeText").GetComponent<Text>() as Text;
        GearText = GameObject.Find("GearText").GetComponent<Text>() as Text;
        Needle = GameObject.Find("Needle");
        _AllMileage = GameObject.Find("AllMileageText").GetComponent<TextMeshProUGUI>();
        _CurrentMileage = GameObject.Find("CurrentMileageText").GetComponent<TextMeshProUGUI>();
    }

    private void Update() {
        if(VehicleManager == null) {
            VehicleManager = GameObject.FindGameObjectWithTag("Player").GetComponent<VehicleManager>();
        }
        _CurrentMileage.text = (VehicleManager.VehicleDynamics.currentMileage).ToString("f2") + "м";
        _AllMileage.text = ((VehicleManager.VehicleDynamics.allMileage + VehicleManager.VehicleDynamics.currentMileage) / 1000).ToString("f2") + "км";
        SpeedText.text = VehicleManager.PhysicsCalculation.speed.ToString("0");
        SpeedText.color = (VehicleManager.VehicleDynamics.maxSpeedOnCurrentGear > VehicleManager.PhysicsCalculation.Kph + 5) ? new Color(255, 255, 255) : new Color (255, 0, 0);
        var Transmission = VehicleManager.Transmission;
        GearText.text = (Transmission.currentGear == 0) ? "N" : (Transmission.currentGearRatio < 0) ? "R" : (Transmission.currentGear).ToString("");
        updateNeedle();
        TimerLogic();
    }

    public void updateNeedle() {
        desiredPosition = startAngle - endAngle;
        float temp = VehicleManager.Engine.rpm / 8000;
        Needle.transform.eulerAngles = new Vector3 (0, 0, (startAngle - temp * desiredPosition));
    }

    private void TimerLogic() {
        realTime = Time.timeSinceLevelLoad;
        if(!stopTimer) {
            raceTime = realTime - whileStop;
            ITimer();
        } else {
            whileStop = realTime - raceTime;
        }
        RaceTimeText.text = "Race time  " + minutes.ToString("D2") + ":" + seconds.ToString("D2") + "." + milliseconds.ToString("D3");
    }

    private void ITimer() {
        hours = (int)(raceTime / 3600f);
        minutes = (int)(raceTime / 60f) % 60;
        seconds = (int)(raceTime % 60f);
        milliseconds = (int)(raceTime * 1000f) % 1000;
    }
}