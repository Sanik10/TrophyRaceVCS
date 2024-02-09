using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class PhoneInput : MonoBehaviour {

    private VehicleInputHandler _VehicleInputHandler;
    public PhoneSteeringType phoneSteering;
    private ButtonValueReciever MoveButton;
    private ButtonValueReciever BrakeButton;
    private ButtonValueReciever ClutchButton;
    private ButtonValueReciever GearUpButton;
    private ButtonValueReciever GearDownButton;
    private ButtonValueReciever ReverseGearButton;
    private Vector3 acceleration;
    private float accelerationMultiplier = 0.7f;

    private void Start() {
        this._VehicleInputHandler = GetComponent<VehicleInputHandler>();
        if(GameObject.Find("MoveButton") != null) {
            MoveButton = GameObject.Find("MoveButton").GetComponent<ButtonValueReciever>();
        }
        if(GameObject.Find("BrakeButton") != null) {
            BrakeButton = GameObject.Find("BrakeButton").GetComponent<ButtonValueReciever>();
        }
        if(GameObject.Find("ClutchButton") != null) {
            ClutchButton = GameObject.Find("ClutchButton").GetComponent<ButtonValueReciever>();
        }
        if(GameObject.Find("GearUpButton") != null) {
            GearUpButton = GameObject.Find("GearUpButton").GetComponent<ButtonValueReciever>();
        }
        if(GameObject.Find("GearDownButton") != null) {
            GearDownButton = GameObject.Find("GearDownButton").GetComponent<ButtonValueReciever>();
        }
        if(GameObject.Find("ReverseGearButton") != null) {
            ReverseGearButton = GameObject.Find("ReverseGearButton").GetComponent<ButtonValueReciever>();
        }
    }

    private void Update() {
        if(MoveButton != null && BrakeButton.value == 0) {
            this._VehicleInputHandler.vertical = MoveButton.value;
        } else if(BrakeButton != null) {
            this._VehicleInputHandler.vertical = BrakeButton.value;
        }

        if(phoneSteering == PhoneSteeringType.accelerator) {
            AccelerationSteering();
        } else if(phoneSteering == PhoneSteeringType.arrows) {
            ArrowsSteering();
        } else if(phoneSteering == PhoneSteeringType.wheel) {
            WheelSteering();
        } else {
            TapsSteering();
        }

        if(ClutchButton != null) {
            this._VehicleInputHandler.clutch = ClutchButton.value;
        }

        if(GearUpButton != null && GearDownButton.value == 0) {
            this._VehicleInputHandler.gearUp = GearUpButton.value == 1 ? true : false;
        }

        if(GearDownButton != null) {
            this._VehicleInputHandler.gearDown = GearDownButton.value == 1 ? true : false;
        }

        if(GearDownButton != null && ReverseGearButton.value == 1) {
            this._VehicleInputHandler.reverseGear = true;
        }
    }

    private void AccelerationSteering() {
        Vector3 acceleration = Input.acceleration;
        this._VehicleInputHandler.horizontal = acceleration.x * (1+accelerationMultiplier);
    }

    private void ArrowsSteering() {

    }

    private void WheelSteering() {
        
    }

    private void TapsSteering() {
        
    }
}

public enum PhoneSteeringType {
    accelerator, arrows, wheel, taps
}