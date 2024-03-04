using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NWH.WheelController3D;
using NWH.Common.Vehicles;

/// <summary>
/// This script is needed to move the wheel in an arc
/// </summary>
public class ArcMoveFromWheelCollider : MonoBehaviour {

	[SerializeField] private WheelUAPI WheelCollider;					//WheelCollider ref
	[SerializeField] private Vector3 OffsetPosition;					//Offset position, calculate from local space of WheelCollider
	[SerializeField] private float ArmLength;							//Arm length, radius
	[SerializeField, Range(0f, 1f)] private float MaxLengthOnDistance;  //The position of the wheel in which the arm is horizontal
	[SerializeField] private bool Invertion;							//Invertion direction

	private Vector3 _CenterArcPoint;									//Point the center of the circle
	private Vector3 CenterArcPoint {
		get {
			if (_CenterArcPoint != Vector3.zero) {
				return _CenterArcPoint;
			} else {
				return (OffsetPosition + (Vector3.left * (Invertion ? ArmLength : -ArmLength)));
			}
		}
	}

	Vector3 Position;				//For getting position from WheelCollider
	Quaternion Rotation;			//For getting rotation from WheelCollider

	private void Awake () {
		//Saving center position on start
		_CenterArcPoint = CenterArcPoint ;
	}

	private void FixedUpdate () {
		// Get position and rotation feom WheelCollider
		// WheelCollider.GetWorldPose(out Position, out Rotation);
		Position = WheelCollider.WheelPosition;
		Rotation = WheelCollider.WheelRotation;

		//Position translation to local space
		// Position = WheelCollider.transform.InverseTransformPoint(Position) + OffsetPosition;
		Position = WheelCollider.transform.InverseTransformPoint(Position);
		Position -= CenterArcPoint;

		//Find Y of wheel position
		var offsetY = Position.y - MaxLengthOnDistance * WheelCollider.SpringMaxLength;

		if (offsetY < ArmLength && offsetY > -ArmLength) {
			//Calcelate X, According to the formula X*X + Y*Y = R*R
			Position.x = OffsetPosition.x + Mathf.Sqrt((ArmLength * ArmLength) - (offsetY * offsetY));
		}
		if (Invertion) Position.x = -Position.x;
		transform.localPosition = Position + OffsetPosition;
	}

	List<Vector3> GizmoPoints = new List<Vector3>();
	float GizmoArmLength;
	float GizmoSuspensionDistance;
	float GizmoMaxLengthOnDistance;

	void CalculateGizmoPoints () {
		GizmoPoints.Clear();
		var centerArcWheelColliderPoint = Vector3.left * (Invertion? -ArmLength: ArmLength);
		Vector3 point;
		for (int i = 0; i < (int)(WheelCollider.SpringMaxLength * 100); i++) {
			point = new Vector3(0, ((float)i / 100f) - (WheelCollider.SpringMaxLength / 2), 0);
			var offsetY = point.y - MaxLengthOnDistance * (WheelCollider.SpringMaxLength) + (WheelCollider.SpringMaxLength / 2);
			point -= centerArcWheelColliderPoint;
			if (offsetY < ArmLength && offsetY > -ArmLength) {
				point.x = Mathf.Sqrt((ArmLength * ArmLength) - (offsetY * offsetY));
			}
			if (Invertion) point.x = -point.x;
			GizmoPoints.Add(point);
		}
		GizmoArmLength = ArmLength;
		GizmoSuspensionDistance = WheelCollider.SpringMaxLength;
		GizmoMaxLengthOnDistance = MaxLengthOnDistance;
	}

	private void OnDrawGizmosSelected () {

		if (WheelCollider == null) return;

		var centerArcWheelColliderPoint = Vector3.left * (Invertion? -ArmLength: ArmLength);

		if (GizmoPoints.Count == 0 ||
		!Mathf.Approximately(GizmoArmLength, ArmLength)||
		!Mathf.Approximately(GizmoSuspensionDistance, WheelCollider.SpringMaxLength) ||
		!Mathf.Approximately(GizmoMaxLengthOnDistance, MaxLengthOnDistance)) {
			CalculateGizmoPoints ();
		}

		Gizmos.color = Color.green;
		Vector3 prevPoint = GizmoPoints[0];
		for (int i = 1; i < GizmoPoints.Count; i++) {
			Gizmos.DrawLine(WheelCollider.transform.TransformPoint(centerArcWheelColliderPoint + OffsetPosition + prevPoint), WheelCollider.transform.TransformPoint(centerArcWheelColliderPoint + OffsetPosition + GizmoPoints[i]));
			prevPoint = GizmoPoints[i];
		}
		Gizmos.DrawLine(WheelCollider.transform.TransformPoint(centerArcWheelColliderPoint + OffsetPosition), WheelCollider.transform.TransformPoint(centerArcWheelColliderPoint + OffsetPosition + (GizmoPoints[0])));
		Gizmos.DrawLine(WheelCollider.transform.TransformPoint(centerArcWheelColliderPoint + OffsetPosition), WheelCollider.transform.TransformPoint(centerArcWheelColliderPoint + OffsetPosition + GizmoPoints[GizmoPoints.Count - 1]));

		Gizmos.color = Color.yellow;

		Gizmos.DrawWireSphere(WheelCollider.transform.TransformPoint(centerArcWheelColliderPoint), 0.02f);

		Gizmos.color = Color.white;
	}
}
