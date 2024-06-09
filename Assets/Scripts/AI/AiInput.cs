using System.Collections.Generic;
using UnityEngine;

public class AiInput : MonoBehaviour {

    private VehicleManager _VehicleManager;
    private PhysicsCalculation _PhysicsCalculation;
    private GameObject Vehicle;
    private VehicleInputHandler _VehicleInputHandler;
    private WayPoints _wayPoints;

    [Range(1, 10)]
    public int sensorsLength = 2;
    [SerializeField]
    private GameObject _RaysPoint;
    [SerializeField]
    private Transform _nextWaypoint;
    [SerializeField]
    private Transform _currentWaypoint;
    [SerializeField]
    private Transform _previousWaypoint;
    [SerializeField]
    private int _currentWaypointNumber = 0;
    [SerializeField]
    private Transform _targetVehicle;
    [SerializeField]
    private Transform[] _nodes;
    [SerializeField] [Range(0, 1)]
    private float _maxAcceleration = 1;
    [SerializeField] [Range(1, 10)]
    private int _distanceOffset = 1;
    [SerializeField]
    private float _turningAngle = 0;
    private Vector3 _relative;
    [SerializeField]
    private bool _bigTurnAngle = false;
    [SerializeField]
    private bool _medTurnAngle = false;
    [SerializeField]
    private bool _smallTurnAngle = false;
    [SerializeField]
    private float distance;
    
    private void Start() {
        foreach(Transform i in gameObject.transform) {
            if(i.transform.name == "RaysPoint") {
                _RaysPoint = i.gameObject;  
            }
        }
        this._VehicleInputHandler = GetComponent<VehicleInputHandler>();
        this._VehicleManager = GetComponent<VehicleManager>();
        this._PhysicsCalculation = GetComponent<PhysicsCalculation>();
        if(GameObject.FindGameObjectWithTag("Path") != null) {
            this._wayPoints = GameObject.FindGameObjectWithTag("Path").GetComponent<WayPoints>();
            this._nodes = new Transform[this._wayPoints.checkpoints.Count];
            for(int i = 0; i < this._wayPoints.checkpoints.Count; i++) {
                this._nodes[i] = this._wayPoints.checkpoints[i].transform;
            }
        }
        calculateDistanceOfWaypoints();
    }

    private void FixedUpdate() {
        if(!this._VehicleManager.chaseVehicle) {
            calculateDistanceOfWaypoints();
        } else {
            if(this._targetVehicle == null) {
                if(GameObject.FindGameObjectWithTag("Player") != null) {
                    this._targetVehicle = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
                } else {
                    Debug.LogWarning("Автомобиля с тегом Player не найдено");
                }
            }
            this._currentWaypoint = this._targetVehicle;
            calculateDistanceOfWaypoints();
        }

        SteeringInput();
    }

    private void calculateDistanceOfWaypoints() {
    Vector3 position = gameObject.transform.position;

    if (this._VehicleManager.chaseVehicle) {
        distance = Vector3.Distance(this._currentWaypoint.transform.localPosition, transform.localPosition);
    } else {
        distance = Mathf.Infinity;

        for (int i = 0; i < this._nodes.Length; i++) {
            Vector3 difference = this._nodes[i].transform.position - position;
            float currentDistance = difference.magnitude;

            // Переключение на следующую точку при минимальном расстоянии или при последнем чекпоинте
            if ((currentDistance < distance && (i > this._currentWaypointNumber - 4 && i < this._currentWaypointNumber + 4)) || (i == this._nodes.Length - 1 && currentDistance < distance)) {
                // если зашли в этот if - переключение на след. точку
                this._previousWaypoint = this._currentWaypoint;
                this._nextWaypoint = this._nodes[(i + this._distanceOffset) % this._nodes.Length];
                this._currentWaypoint = this._nodes[i];
                this._currentWaypointNumber = i;
                distance = currentDistance;
            }
        }

        // Дополнительная проверка для переключения на 0
        if (this._currentWaypointNumber == this._nodes.Length - 1 && Vector3.Distance(this._nodes[0].position, position) < distance) {
            this._previousWaypoint = this._currentWaypoint;
            this._nextWaypoint = this._nodes[this._distanceOffset % this._nodes.Length];
            this._currentWaypoint = this._nodes[0];
            this._currentWaypointNumber = 0;
        }
    }

    CheckAngle();
}

    private void CheckAngle() {
        this._relative = transform.InverseTransformPoint(this._nextWaypoint.transform.position);
        this._relative /= this._relative.magnitude;
        this._turningAngle = Mathf.Atan2(this._relative.x, this._relative.z) * Mathf.Rad2Deg;


        this._bigTurnAngle = Mathf.Abs(this._turningAngle) > 54;
        this._medTurnAngle = Mathf.Abs(this._turningAngle) > 36;
        this._smallTurnAngle = Mathf.Abs(this._turningAngle) > 18;
    }

    private void SteeringInput() {
        this._VehicleInputHandler.horizontal = this._bigTurnAngle ? this._relative.x * 1.5f : this._relative.x;
    }
}