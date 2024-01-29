using UnityEngine;
using System;

namespace TrophyRace.Architecture {
    public class GameManager : MonoBehaviour {

        public static event Action StartRaceEvent;
        public static event Action SetVehiclesInPreRaceModeEvent;

        public bool startRace;
        public bool pauseResumeRace;
        public bool preRace;

        [Header("Other")]
        private VehicleSpawner _vehicleSpawner;
        public GameObject SpawnPoint;
        private float spawnTime;
        // private bool gridFlag = false;
        
        [Header("Spawn Settings")]
        [Range(0, 100)] public int opponents = 2;
        public bool spawnVehicle = false;
        [Range(0, 10)] public float horizontalDisplacement = 2;
        [Range(0, 10)] public float verticalDisplacement = 3;

        [Header("Options")]
        public bool generateStartGrid = false;
        public bool activateAi = false;
        public bool disableAi = false;
        public bool saveAll = false;

        private void Start() {
            preRace = true;
            startRace = false;
            pauseResumeRace = false;
            InitializeVehicle();
            GenerateStartGrid();
        }

        private void OnEnable() {
            AnimatedCamerasController.AnimationDoneEvent += StartRace;
        }

        private void Update() {
            HandleVehicleSpawning();

            HandleDataSaving();

            HandleRaceEvents();
            
        }

        private void InitializeVehicle() {
            if(spawnVehicle) {
                // gridFlag = true;
                spawnVehicle = false;
            }
            
            _vehicleSpawner = GameObject.Find("scripts").GetComponent<VehicleSpawner>();
            _vehicleSpawner.SpawnVehicle(PlayerPrefs.GetInt("selectedVehicleId"), SpawnMode.None);
            
            var raceTimer = GetComponent<RaceTimer>();
            raceTimer.RegisterCar(_vehicleSpawner.spawnedVehicle);
            
            Debug.Log(_vehicleSpawner.spawnedVehicle);
            spawnTime = Time.time + 0.1f;
        }

        private void GenerateStartGrid() {
            SpawnPoint = GameObject.Find("SpawnPoint");

            if(opponents == 0)
                return;
            
            Vector3 originPos = SpawnPoint.transform.localPosition;
            
            for(int i = 0; i < opponents; i++) {
                AdjustSpawnPointPosition(i);
                
                _vehicleSpawner.SpawnVehicle(PlayerPrefs.GetInt("selectedVehicleId"), SpawnMode.Bot | SpawnMode.DisableCameras | SpawnMode.PreStart);
                
                var raceTimer = GetComponent<RaceTimer>();
                raceTimer.RegisterCar(_vehicleSpawner.spawnedVehicle);
            }
            
            SpawnPoint.transform.localPosition = originPos;
            generateStartGrid = false;
        }

        private void AdjustSpawnPointPosition(int i) {
            Vector3 translation = new Vector3(
                (i % 2 == 0 ? horizontalDisplacement : -horizontalDisplacement),
                0,
                -verticalDisplacement
            );

            SpawnPoint.transform.Translate(translation, Space.Self);
        }

        private void HandleVehicleSpawning() {
            if(spawnVehicle) {
                _vehicleSpawner.SpawnVehicle(PlayerPrefs.GetInt("selectedVehicleId"), SpawnMode.None);
                spawnVehicle = false;
            }
        }

        private void HandleDataSaving() {
            if(saveAll) {
                SaveAllVehicleData();
                saveAll = false;
            }
        }

        private void SaveAllVehicleData() {
            var allVehicleData = Resources.LoadAll<VehicleData>("VehiclesConfig");
            
            foreach(var vehicleData in allVehicleData) {
                vehicleData.Save("GameManager");
            }
        }

        private void HandleRaceEvents() {
            if(startRace) {
                StartRace();
            }

            if(preRace) {
                SetVehiclesInPreRaceModeEvent?.Invoke();
            }
        }

        private void StartRace() {
            AnimatedCamerasController.AnimationDoneEvent -= StartRace;
            preRace = false;
            StartRaceEvent?.Invoke();
            startRace = false;
        }
    }
}