using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NWH.Common.Vehicles;

/// <summary>
/// This script is needed to change the position and rotation the axle
/// </summary>
public class Axle : MonoBehaviour {

	[SerializeField] WheelUAPI LeftWheelCollider;					//Left WheelCollider ref
	[SerializeField] WheelUAPI RightWheelCollider;					//Right WheelCollider ref
	[SerializeField] Transform LeftWheelView;						//Left wheel view, to rotate the transform
	[SerializeField] Transform RightWheelView;						//Right wheel view, to rotate the transform
	[SerializeField] float AngleMiltiplier = 45;					//Rotation multiplier

	[SerializeField] Vector3 LeftWheelPosition;		//For getting position from LeftWheelCollider
	[SerializeField] Vector3 RightWheelPosition;		//For getting position from RightWheelCollider
	Transform TransformHelper;		//For save transform at the start
	[SerializeField] float Distance;					//Distance between wheels, for the angle calculation formula

	private void Awake() {
		//Saving transform on start
		TransformHelper = new GameObject("AxleTransformHelper").transform;
		TransformHelper.SetParent(transform.parent);
		TransformHelper.position = transform.position;
	}

	private void FixedUpdate () {
		//Get wheels position
		Quaternion rot;

		// LeftWheelCollider.GetWorldPose(out LeftWheelPosition, out rot);
		// RightWheelCollider.GetWorldPose(out RightWheelPosition, out rot);
		LeftWheelPosition = LeftWheelCollider.WheelPosition;
		RightWheelPosition = RightWheelCollider.WheelPosition;
		// rot = LeftWheelCollider.WheelRotation;

		LeftWheelPosition = TransformHelper.transform.InverseTransformPoint(LeftWheelPosition);
		RightWheelPosition = TransformHelper.transform.InverseTransformPoint(RightWheelPosition);

		Distance = RightWheelPosition.x - LeftWheelPosition.x;

		//Calculate axle pos, position is considered the midpoint between the wheels
		Vector3 newAxlePos = TransformHelper.localPosition;
		newAxlePos.y += (LeftWheelPosition.y + RightWheelPosition.y) * 0.5f;
		transform.localPosition = newAxlePos;

		//Calculate axle rotation, the angle is calculated from the height difference of the wheels in local space
        float angle = (LeftWheelPosition.y > RightWheelPosition.y)? -AngleMiltiplier : AngleMiltiplier;
        angle = Mathf.Abs(LeftWheelPosition.y - RightWheelPosition.y) / Distance * angle;
		rot = Quaternion.AngleAxis(angle, Vector3.forward);

		transform.localRotation = rot;

		//Wheels rotation assignment, from WheelColliders
		LeftWheelView.localRotation *= Quaternion.Euler(LeftWheelCollider.RPM * 6 * Time.deltaTime, 0f, 0f);
		RightWheelView.localRotation *= Quaternion.Euler(RightWheelCollider.RPM * 6 * Time.deltaTime, 0f, 0f);
	}
}
