using System.Collections.Generic;
using UnityEngine;

public class AiInput : MonoBehaviour {

    private VehicleManager VM;
    private PhysicsCalculation PC;
    private GameObject Vehicle;

    private VehicleInputHandler _VehicleInputHandler;
    [Range(1, 10)]
    public int sensorsLength = 2;
    [SerializeField]
    private GameObject _RaysPoint;
    private bool preRaceMode = false;

    public WayPoints WP;
    public Transform currentWaypoint;
    public Transform currentWaypointNext, targetVehicle;
    public List<Transform> nodes = new List<Transform>();
    [Range(0, 1)]public float AiAcceleration = 1;
    [Range(0, 5)] public float sterrForce = 1;
    [Range(1, 10)] public int distanceOffset = 1;
    [Range(0, 10)] public int SmoothMultipilar = 5;
    public Vector3 relative;
    public float horizontalSteer;
    public float brakeDistance;
    public float targetSpeed;
    public float distance;
    public bool bigTurnAngle = false;
    public bool medTurnAngle = false;
    
    private void Start() {
        foreach(Transform i in gameObject.transform) {
            if(i.transform.name == "RaysPoint") {
                _RaysPoint = i.gameObject;  
            }
        }
        this._VehicleInputHandler = GetComponent<VehicleInputHandler>();
        VM = GetComponent<VehicleManager>();
        PC = VM.PhysicsCalculation;
        Vehicle = GameObject.FindGameObjectWithTag("Player");
        targetVehicle = Vehicle.GetComponent<Transform>();
        if(VM.aiVehicle) {
            if(GameObject.FindGameObjectWithTag("Path") != null) {
                WP = GameObject.FindGameObjectWithTag("Path").GetComponent<WayPoints>();
                nodes = WP.nodes;
            }
            calculateDistanceOfWaypoints();
        }
    }

    private void Update() {
        brakeDistance = (PC.Kph*PC.Kph - targetSpeed*targetSpeed) / (250*VM.TiresFriction.totalGroundFriction);
        relative = transform.InverseTransformPoint(currentWaypoint.transform.position);
        relative /= relative.magnitude;


        horizontalSteer = !_VehicleInputHandler.handbrake ? (relative.x / relative.magnitude) * sterrForce : (relative.x / relative.magnitude > 0) ? sterrForce : (relative.x / relative.magnitude < 0) ? -sterrForce : 0;
        // horizontal = Mathf.Lerp(horizontal, horizontal, SmoothMultipilar * Time.deltaTime);
            // if(bigTurnAngle && (PC.Kph > 60 || (VM.WheelsSettings.totalSlip > 0.5f && PC.Kph > 30)) || (medTurnAngle && PC.Kph > 70)) {
            //     this._VehicleInputHandler.vertical = -1;
            // } else if(PC.Kph > 70 && (relative.x > 0.2f || relative.x < -0.2f)) {
            //     this._VehicleInputHandler.vertical = 0;
            // } else {
            //     this._VehicleInputHandler.vertical = AiAcceleration;
            // }
        this._VehicleInputHandler.vertical = (PC.Kph < PC.recommendedSpeed - 10) ? 1 : (PC.Kph > PC.recommendedSpeed + 10) ? -1 : (PC.Kph < 10) ? 1 : 0;
        this._VehicleInputHandler.handbrake = (PC.Kph - PC.recommendedSpeed >= 50) ? true : false;

        // RaycastHit hit;
        // Vector3 pos;

        // if(Physics.Raycast(pos, transform.forward, hit, sensorsLength)) {
        //     Debug.DrawLine(pos, hit.point, Color.red);
        // }


        // if(this._VehicleInputHandler.vertical == 1 && (PC.Kph <= 5 || PC.Kph >= -5)) {
        //     this._VehicleInputHandler.reverseGear = true;
        //     horizontalSteer *= -1;
        // } else if((PC.Kph >= 25 || PC.Kph <= -25) && this._VehicleInputHandler.reverseGear == true) {
        //     this._InputHandler.gearUp = true;
        // }

        this._VehicleInputHandler.horizontal = horizontalSteer;
    }

    private void FixedUpdate() {
        if(VM.aiVehicle && !VM.chaseVehicle) {
            calculateDistanceOfWaypoints();
        } else if(VM.aiVehicle) {
            currentWaypoint = targetVehicle;
            calculateDistanceOfWaypoints();
        }
    }

    private void calculateDistanceOfWaypoints() {
        Vector3 position = gameObject.transform.position;
        if(VM.chaseVehicle){
            distance = Vector3.Distance(currentWaypoint.transform.localPosition, transform.localPosition);
        } else {
            distance = Mathf.Infinity;

            for (int i = 0; i < nodes.Count; i++) {
                Vector3 difference = nodes[i].transform.position - position;
                float currentDistance = difference.magnitude;
                if (currentDistance < distance) {
                    if ((i + distanceOffset) >= nodes.Count) {
                        i = 1;
                    }

                    currentWaypoint = nodes[i + distanceOffset];
                    if(i + 2 >= nodes.Count) {
                        currentWaypointNext = nodes[3];
                    } else {
                        currentWaypointNext = nodes[i + 2];
                    }
                    CheckAngle();
                    distance = currentDistance;
                }
            }
        }
    }

    private void CheckAngle() {
        relative = transform.InverseTransformPoint(currentWaypointNext.transform.position);
        relative /= relative.magnitude;
        if(relative.x > 0.6f || relative.x < -0.6f) {
            bigTurnAngle = true;
        } else if(relative.x > 0.4f || relative.x < -0.4f) {
            medTurnAngle = true;
        } else {
            medTurnAngle = false;
            bigTurnAngle = false;
        }
    }
}
