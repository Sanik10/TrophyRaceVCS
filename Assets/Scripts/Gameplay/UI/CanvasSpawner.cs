using UnityEngine;

public class CanvasSpawner : MonoBehaviour {

    public void SpawnCanvas(string name) {
        Instantiate(Resources.Load("UIElements/Canvas/" + name) as GameObject);
    }
}
