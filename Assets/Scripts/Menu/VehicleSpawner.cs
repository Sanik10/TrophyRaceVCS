using System;
using System.Collections.Generic;
using UnityEngine;

namespace TrophyRace.Architecture {
    public class VehicleSpawner : MonoBehaviour {
        
        public event Action vehicleSpawned;

        private Vector3 _spawnPosition;
        private Quaternion _spawnRotation;
        private GameObject SpawnPoint;
        private List<GameObject> _opponents = new List<GameObject>();
        [SerializeField]
        private GameObject _playerVehicle;

        public GameObject playerVehicle => this._playerVehicle;
        public List<GameObject> opponents => this._opponents;

        [SerializeField]
        private GameObject _spawnedVehicle;
        public GameObject spawnedVehicle => this._spawnedVehicle;

        private void Start() {
            SpawnPoint = GameObject.Find("SpawnPoint");
        }

        public void SpawnVehicle(int vehicleID, SpawnMode spawnMode) {
            if(SpawnPoint == null) {
                SpawnPoint = GameObject.Find("SpawnPoint");
                _spawnPosition = GetComponent<Transform>().position;
                _spawnRotation = GetComponent<Transform>().rotation;
            } else {
                _spawnPosition = SpawnPoint.GetComponent<Transform>().position;
                _spawnRotation = SpawnPoint.GetComponent<Transform>().rotation;
            }
            Quaternion spwanrtshn = Quaternion.Euler(0, 0, 0);

            VehicleData vehicleData = GetVehicleDataByID(vehicleID);
            vehicleData.Load();

            if((spawnMode & SpawnMode.Bot) == 0) {
                _playerVehicle = Instantiate(vehicleData.Prefab, _spawnPosition, spwanrtshn);
            } else {
                GameObject aiVehicle = Instantiate(vehicleData.Prefab, _spawnPosition, spwanrtshn);
                aiVehicle.GetComponent<VehicleManager>().aiVehicle = true;
                _opponents.Add(aiVehicle);
            }

            GameObject vehicle = _spawnedVehicle = (spawnMode & SpawnMode.Bot) != 0 ? _opponents[_opponents.Count-1] : _playerVehicle;

            vehicle.GetComponent<Transform>().rotation = _spawnRotation;
            vehicle.GetComponent<VehicleManager>().vehicleData = vehicleData;

            if((spawnMode & SpawnMode.Freeze) != 0) {
                vehicle.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationY;
                vehicle.GetComponent<VehicleVFX>().hasTrails = _playerVehicle.GetComponent<VehicleVFX>().hasSmokes = false;
            }
            if((spawnMode & SpawnMode.DisableCameras) != 0) {
                if(vehicle.GetComponentInChildren<Transform>().Find("Cameras") != null) {
                    vehicle.GetComponentInChildren<Transform>().Find("Cameras").gameObject.SetActive(false);

                }
            }
            if((spawnMode & SpawnMode.DisableReflectionProbes) != 0) {
                if(vehicle.GetComponentInChildren<Transform>().Find("Reflection Probe") != null) {
                    vehicle.GetComponentInChildren<Transform>().Find("Reflection Probe").gameObject.SetActive(false);
                }
            }
            // if((spawnMode & SpawnMode.PreStart) != 0) {
            //     vehicle.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationY;
            // }

            vehicleSpawned?.Invoke();
        }

        private VehicleData GetVehicleDataByID(int vehicleID) {
            VehicleList vehicleList = GetComponent<VehicleList>();
            if(vehicleList != null && vehicleList.allVehiclesInGame != null && vehicleList.allVehiclesInGame.Count > 0) {
                Debug.Log(vehicleID + " Переданный, Сохраненный: " + PlayerPrefs.GetInt("selectedVehicleId"));
                return vehicleList.allVehiclesInGame.Find(data => data.id == vehicleID);
            } else {
                VehicleData[] allVehicleData = Resources.LoadAll<VehicleData>("VehiclesConfig");
                return Array.Find(allVehicleData, data => data.id == vehicleID);
            }
        }
    }
}

[System.Serializable] [Flags]
public enum SpawnMode {
    None = 0,
    Freeze = 1,
    Bot = 2,
    DisableCameras = 4,
    DisableReflectionProbes = 8,
    PreStart = 16,
    // Добавьте дополнительные режимы при необходимости
}