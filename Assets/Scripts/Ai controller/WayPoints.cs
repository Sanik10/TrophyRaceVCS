using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPoints : MonoBehaviour {

    [SerializeField] private bool showWaypointGizmos = true;
    [SerializeField] private Color checkpointLineColor;
    [SerializeField] private Color routeLineColor;
    [Range(1, 20)] public float checkpointRadius = 1;
    [Range(1, 20)] public float nodeRadius = 1;

    [SerializeField] private List<Checkpoint> _checkpoints = new List<Checkpoint>();
    public List<Checkpoint> checkpoints => this._checkpoints;

    private void OnDrawGizmos() {
        if (showWaypointGizmos) {
            DrawCheckpointGizmos();
        }
    }

    private void DrawCheckpointGizmos() {
        for (int i = 0; i < this._checkpoints.Count; i++) {
            var currentCheckpoint = this._checkpoints[i];
            Gizmos.color = checkpointLineColor;
            Gizmos.DrawSphere(currentCheckpoint.transform.position, checkpointRadius);

            // Отрисовка линии между чекпоинтами (замыкаем линию с последним чекпоинтом)
            var nextCheckpoint = i == this._checkpoints.Count - 1 ? this._checkpoints[0] : this._checkpoints[i + 1];
            Gizmos.DrawLine(currentCheckpoint.transform.position, nextCheckpoint.transform.position);

            for (int j = 0; j < currentCheckpoint.routes.Count; j++) {
                DrawRouteGizmos(currentCheckpoint.routes[j].node);
            }
        }
    }

    private void DrawRouteGizmos(Transform[] nodes) {
        Gizmos.color = routeLineColor;

        for (int i = 0; i < nodes.Length; i++) {
            var currentWaypoint = nodes[i].position;
            var previousWaypoint = i != 0 ? nodes[i - 1].position : nodes[nodes.Length - 1].position;

            Gizmos.DrawLine(previousWaypoint, currentWaypoint);
            Gizmos.DrawSphere(currentWaypoint, nodeRadius);
        }
    }
}

[System.Serializable]
public class Checkpoint {
    public string name;
    public Transform transform;
    public List<Route> routes = new List<Route>();
}

[System.Serializable]
public class Route {
    public string name;
    public Transform[] node;
}