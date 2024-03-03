using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NWH.Common.Vehicles;

/// <summary>
/// This script is needed to move the object based on the angle of angle of the WheelCollider
/// </summary>
public class MoveBySteerAngle : MonoBehaviour {

	[SerializeField] WheelUAPI WheelController;		//WheelCollider ref
	[SerializeField] Direction Direction;				//Move direction
	[SerializeField] float Mulriplier;					//Move multiplier
	
	Vector3 StartLocalPosition;

	private void Awake () {
		StartLocalPosition = transform.localPosition;
	}

	void FixedUpdate () {
		transform.localPosition = StartLocalPosition + (Direction.vector3 * WheelController.SteerAngle * Mulriplier);
	}
}
