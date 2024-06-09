using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPoints : MonoBehaviour {

    [SerializeField] private bool _showWaypointGizmos = true;
    [SerializeField] private Color _checkpointLineColor;
    [SerializeField] private Color _routeLineColor;
    [Range(1, 20)] public float _checkpointRadius = 1;
    [Range(1, 20)] public float _nodeRadius = 1;
    [SerializeField] private float _trackDistance = 0;
    private Dictionary<GameObject, CarData> _carTimes = new Dictionary<GameObject, CarData>();
    // public Dictionary<GameObject, CarData> carTimes => _carTimes;

    [SerializeField] private List<Checkpoint> _checkpoints = new List<Checkpoint>();
    public List<Checkpoint> checkpoints => this._checkpoints;

    private void OnDrawGizmos() {
        if(this._showWaypointGizmos) {
            DrawCheckpointGizmos();
        }
    }

    private void DrawCheckpointGizmos() {
        this._trackDistance = 0;
        for(int i = 0; i < this._checkpoints.Count; i++) {
            Checkpoint currentCheckpoint = this._checkpoints[i];
            Gizmos.color = this._checkpointLineColor;
            Gizmos.DrawSphere(currentCheckpoint.transform.position, this._checkpointRadius);

            // Отрисовка линии между чекпоинтами (замыкаем линию с последним чекпоинтом)
            var nextCheckpoint = i == this._checkpoints.Count - 1 ? this._checkpoints[0] : this._checkpoints[i + 1];
            Gizmos.DrawLine(currentCheckpoint.transform.position, nextCheckpoint.transform.position);
            this._trackDistance += Vector3.Distance(currentCheckpoint.transform.localPosition, nextCheckpoint.transform.localPosition);

            for(int j = 0; j < currentCheckpoint.routes.Count; j++) {
                DrawRouteGizmos(currentCheckpoint.routes[j].node);
            }
        }
    }

    private void DrawRouteGizmos(Transform[] nodes) {
        Gizmos.color = this._routeLineColor;

        for(int i = 0; i < nodes.Length; i++) {
            var currentWaypoint = nodes[i].position;
            var previousWaypoint = i != 0 ? nodes[i - 1].position : nodes[nodes.Length - 1].position;

            Gizmos.DrawLine(previousWaypoint, currentWaypoint);
            Gizmos.DrawSphere(currentWaypoint, this._nodeRadius);
        }
    }
}

[System.Serializable]
public class Checkpoint {
    public string name;
    public Transform transform;
    public List<Route> routes = new List<Route>();
    public Transform[] nextCheckpoint;
}

[System.Serializable]
public class Route {
    public string name;
    public Transform[] node;
}