using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NWH.Common.Vehicles;

/// <summary>
/// This script is needed to change position and rotatin from WheelCollider
/// </summary>
public class WorldPosFromWheelCollider : MonoBehaviour {

	[SerializeField] WheelUAPI WheelController;
	[SerializeField] bool SetPosition;
	[SerializeField] bool SetRotation;
	[SerializeField] Vector3 OffsetPosition;

	Vector3 Position;
	Quaternion Rotation;

	private void LateUpdate () {
		// WheelController.GetWorldPose(out Position, out Rotation);
		Position = WheelController.WheelPosition;
		Rotation = WheelController.WheelRotation;

		if (SetPosition) {
			transform.position = Position;
			transform.localPosition += OffsetPosition;
		}

		if (SetRotation) {
			transform.rotation = Rotation;
		}
	}
}
