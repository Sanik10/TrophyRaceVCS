/* using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class AiInput : MonoBehaviour
{
    private VehicleManager _VehicleManager;
    private PhysicsCalculation _PhysicsCalculation;
    private VehicleInputHandler _VehicleInputHandler;

    [SerializeField] private float _brakeDistance;
    [SerializeField] [Range(0, 1)] private float _acceleration = 0;
    [SerializeField] [Range(1, 10)] private int distanceOffset = 1;

    [SerializeField] private GameObject _RaysPoint;
    [SerializeField] private GameObject _Vehicle;

    private Transform _targetVehicle;
    [SerializeField] private Transform _currentWaypoint;
    [SerializeField] private Transform _currentWaypointNext;
    [SerializeField] private List<Transform> _nodes = new List<Transform>();
    private WayPoints _wayPoints;
    [SerializeField] private NavMeshPath _navMeshPath;

    public float brakeDistance => this._brakeDistance;

    [Range(1, 10)] public int sensorsLength = 2;
    private bool preRaceMode = false;

    [SerializeField] private Vector3[] navMeshNodes;
    public Vector3 relative;
    public float targetSpeed;
    public float distance;
    public bool bigTurnAngle = false;
    public bool medTurnAngle = false;

    private void Start()
    {
        foreach (Transform i in gameObject.transform)
        {
            if (i.transform.name == "RaysPoint")
            {
                _RaysPoint = i.gameObject;
            }
        }

        this._VehicleManager = GetComponent<VehicleManager>();
        this._VehicleInputHandler = GetComponent<VehicleInputHandler>();
        this._PhysicsCalculation = GetComponent<PhysicsCalculation>();
        this._Vehicle = GameObject.FindGameObjectWithTag("Player");
        this._targetVehicle = this._Vehicle.GetComponent<Transform>();
        this._navMeshPath = new NavMeshPath();

        if (GameObject.FindGameObjectWithTag("Path") != null)
        {
            this._wayPoints = GameObject.FindGameObjectWithTag("Path").GetComponent<WayPoints>();
            
            // Извлекаем трансформы из чекпоинтов
            this._navMeshCheckpoints = this._wayPoints.checkpoints.Select(cp => cp.transform).ToList();
            this._nodes.AddRange(this._navMeshCheckpoints);
        }

        calculateDistanceOfWaypoints();
    }

    private void FixedUpdate()
    {
        UpdateAcceleration();   // Обновление ускорения и торможения

        if (!this._VehicleManager.chaseVehicle)
        {
            calculateDistanceOfWaypoints();
            UpdateSteering();   // Обновление рулевого воздействия
        }
        else if (this._VehicleManager.aiVehicle)
        {
            this._currentWaypoint = this._targetVehicle;
            calculateDistanceOfWaypoints();
            UpdateSteering();   // Обновление рулевого воздействия
        }
    }

    private void UpdateAcceleration()
    {
        this._brakeDistance = (this._PhysicsCalculation.Kph * this._PhysicsCalculation.Kph - targetSpeed * targetSpeed) / (250 * this._VehicleManager.TiresFriction.totalGroundFriction);

        // Логика управления ускорением и торможением
        this._VehicleInputHandler.vertical = (this._PhysicsCalculation.Kph < this._PhysicsCalculation.recommendedSpeed - 10) ? 1 : (this._PhysicsCalculation.Kph > this._PhysicsCalculation.recommendedSpeed + 10) ? -1 : (this._PhysicsCalculation.Kph < 10) ? 1 : 0;
        this._VehicleInputHandler.gearUp = (this._VehicleManager.Engine.rpm >= this._VehicleManager.Transmission.rpmUpShift);
        this._VehicleInputHandler.gearDown = (this._VehicleManager.Engine.rpm <= this._VehicleManager.Transmission.rpmDownShift);
    }

    private void UpdateSteering()
    {
        if (this._nodes.Count > 1)
        {
            Vector3 targetDirection = this._nodes[1].position - transform.position;
            float targetAngle = Vector3.SignedAngle(transform.forward, targetDirection, Vector3.up);
            float horizontalSteer = Mathf.Clamp(targetAngle, -1f, 1f);
            _VehicleInputHandler.horizontal = horizontalSteer;
        }
    }

    private void UpdateNavMeshPath()
    {
        NavMesh.CalculatePath(gameObject.transform.position, this._currentWaypoint.position, NavMesh.AllAreas, this._navMeshPath);

        // Создаем временный список для точек пути
        List<Vector3> tempNodes = new List<Vector3>();
        tempNodes.Add(gameObject.transform.position);

        // Заполняем временный список точками пути
        tempNodes.AddRange(this._navMeshPath.corners.Skip(1)); // Пропускаем первую точку, так как она уже в списке

        // Объединяем точки чекпоинтов с точками пути
        this._nodes = this._navMeshCheckpoints.Select(cp => cp.position).Concat(tempNodes).Select(pos => new GameObject().transform { position = pos }).ToList();
    }

    private void calculateDistanceOfWaypoints()
    {
        Vector3 position = gameObject.transform.position;

        if (this._VehicleManager.chaseVehicle)
        {
            distance = Vector3.Distance(this._currentWaypoint.transform.localPosition, transform.localPosition);
        }
        else
        {
            distance = Mathf.Infinity;

            for (int i = 0; i < this._nodes.Count - 1; i++)
            {
                Vector3 difference = this._nodes[i].transform.position - position;
                float currentDistance = difference.magnitude;

                if (currentDistance < distance)
                {
                    if ((i + distanceOffset) >= this._nodes.Count)
                    {
                        i = 1;
                    }

                    this._currentWaypoint = this._nodes[i + distanceOffset];

                    if (i + 2 >= this._nodes.Count)
                    {
                        this._currentWaypointNext = this._nodes[3];
                    }
                    else
                    {
                        this._currentWaypointNext = this._nodes[i + 2];
                    }

                    CheckAngle();
                    distance = currentDistance;
                }
            }
        }
    }

    private void CheckAngle()
    {
        relative = transform.InverseTransformPoint(this._currentWaypointNext.transform.position);
        relative /= relative.magnitude;

        if (relative.x > 0.6f || relative.x < -0.6f)
        {
            bigTurnAngle = true;
        }
        else if (relative.x > 0.4f || relative.x < -0.4f)
        {
            medTurnAngle = true;
        }
        else
        {
            medTurnAngle = false;
            bigTurnAngle = false;
        }
    }
} */

using System.Collections.Generic;
using UnityEngine;

public class AiInput : MonoBehaviour {

    private VehicleManager _VehicleManager;
    private PhysicsCalculation PC;
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
    private Transform _targetVehicle;
    [SerializeField]
    private Transform[] _nodes;
    [SerializeField] [Range(0, 1)]
    private float _maxAcceleration = 1;
    [SerializeField] [Range(0, 5)]
    private float _maxSteering = 1;
    [SerializeField] [Range(1, 10)]
    public int distanceOffset = 1;
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
        this._VehicleManager = GetComponent<VehicleManager>();
        PC = this._VehicleManager.PhysicsCalculation;
        if(GameObject.FindGameObjectWithTag("Path") != null) {
            this._wayPoints = GameObject.FindGameObjectWithTag("Path").GetComponent<WayPoints>();
            this._nodes = new Transform[this._wayPoints.checkpoints.Count-1];
            for(int i = 0; i < this._wayPoints.checkpoints.Count-1; i++) {
                this._nodes[i] = this._wayPoints.checkpoints[i].transform;
            }
        }
        calculateDistanceOfWaypoints();
    }

    private void Update() {
        brakeDistance = (PC.Kph*PC.Kph - targetSpeed*targetSpeed) / (250*this._VehicleManager.TiresFriction.totalGroundFriction);
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
    }

    private void calculateDistanceOfWaypoints() {
        Vector3 position = gameObject.transform.position;
        if(this._VehicleManager.chaseVehicle){
            distance = Vector3.Distance(this._currentWaypoint.transform.localPosition, transform.localPosition);
        } else {
            distance = Mathf.Infinity;

            for (int i = 0; i < this._nodes.Length-1; i++) {
                Vector3 difference = this._nodes[i].transform.position - position;
                float currentDistance = difference.magnitude;
                if(currentDistance < distance) {
                    if ((i + distanceOffset) >= this._nodes.Length) {
                        i = 1;
                    }
                    this._previousWaypoint = this._currentWaypoint;
                    this._currentWaypoint = this._nodes[i + distanceOffset];
                    if(i + 2 >= this._nodes.Length) {
                        this._nextWaypoint = this._nodes[3];
                    } else {
                        this._nextWaypoint = this._nodes[i + 2];
                    }
                    CheckAngle();
                    distance = currentDistance;
                }
            }
        }
    }

    private void CheckAngle() {
        relative = transform.InverseTransformPoint(this._nextWaypoint.transform.position);
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
