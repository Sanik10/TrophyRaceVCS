using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NWH.Common.Vehicles;

/// <summary>
/// This script is needed to rotate object from WheelCollider.steerAngle
/// </summary>
public class SetRotationBySteerAngle : MonoBehaviour {

	[SerializeField] WheelUAPI WheelController;		//WheelCollider ref
	[SerializeField] float SteerMultiplier = 1;			//Rotate multiplier
	[SerializeField] Axis AxisRotate;					//Axis of rotation

	Quaternion StartRotation;							//For saving rotation object at the start

	private void Awake () {
		if (WheelController == null) {
			Debug.LogError("wheelCollider is null");
			enabled = false;
		}
		//save rotation object
		StartRotation = transform.localRotation;
	}

	private void FixedUpdate () {
		//Calculate angle
		float steer = WheelController.SteerAngle * SteerMultiplier;

		//Raotate object
		switch (AxisRotate) {
			case Axis.X: transform.localRotation = StartRotation * Quaternion.AngleAxis(steer, Vector3.left); break;
			case Axis.Y: transform.localRotation = StartRotation * Quaternion.AngleAxis(steer, Vector3.up); break;
			case Axis.Z: transform.localRotation = StartRotation * Quaternion.AngleAxis(steer, Vector3.forward); break;
		}
	}
}
