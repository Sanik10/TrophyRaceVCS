using UnityEngine;

public class VehicleCamera : MonoBehaviour {

    private cameraSwitcher _CS;
    private int currentCamera = 1;

    private void Start() {
        _CS = GetComponent<cameraSwitcher>();
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.C)) {
            _CS.CameraTransition(currentCamera);
            if(currentCamera < _CS.cameraObj.Length-1) {
                currentCamera++;
            } else {
                currentCamera = 0;
            }
        }
    }
}
