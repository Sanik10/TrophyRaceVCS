using UnityEngine;
using Cinemachine;

public class cameraSwitcher : MonoBehaviour {

    public int currentCamera = 0;
    public GameObject[] cameraObj;
    public bool cycledCam = false;

    private void Start() {
        for(int i = 0; i < cameraObj.Length; i++) {
            if(i != currentCamera) {
                cameraObj[i].SetActive(false);
            } else {
                cameraObj[i].SetActive(true);
            }
            if(cameraObj[i].GetComponent<CinemachineVirtualCamera>() != null) {
                cameraObj[i].GetComponent<CinemachineVirtualCamera>().m_Lens.FarClipPlane = 2000;
            } else if(cameraObj[i].GetComponent<CinemachineFreeLook>()) {
                cameraObj[i].GetComponent<CinemachineFreeLook>().m_Lens.FarClipPlane = 2000;
            }
        }
    }

    // private void Update() {
    //     if(Input.GetKeyDown(KeyCode.C)) {
    //         cycleCam();
    //     }
    // }

    private void cycleCam() {
        if(cycledCam) {
            if(currentCamera >= cameraObj.Length - 1 || currentCamera < 0) {
                currentCamera = 0;
            } else {
                currentCamera ++;
            }
        }
        // CameraTransition();
    }

    public void CameraTransition(int newPosition) {
        int oldPosition = currentCamera;
        if(oldPosition != newPosition) {
            cameraObj[newPosition].SetActive(true);
            cameraObj[oldPosition].SetActive(false); 
            currentCamera = newPosition;
        }
        // if(cycledCam) {
        //     if(currentCamera != 0) {
        //         cameraObj[currentCamera - 1].SetActive(false);

        //         for(int i = 0; i < cameraObj.Length; i++) {
        //             cameraObj[i - 1].SetActive(false);
        //         }

        //     } else if(currentCamera == 0) {
        //         for(int i = 0; i < cameraObj.Length; i++) {
        //             cameraObj[i].SetActive(true);
        //         }
        //     }
        // } else {
        //     int c;
        //     c = (Input.GetKey(KeyCode.Alpha1)) ? 1 : (Input.GetKey(KeyCode.Alpha2)) ? 2 : (Input.GetKey(KeyCode.Alpha3)) ? 3 :(Input.GetKey(KeyCode.Alpha4)) ? 4 : (Input.GetKey(KeyCode.Alpha5)) ? 5 : (Input.GetKey(KeyCode.Alpha6)) ? 6 : 1;
        //     if(c >= 0 || c < 6) {
        //         oldPosition = currentCamera;
        //         currentCamera = c-1;
        //     }
        //     if(oldPosition != currentCamera) {
        //         cameraObj[currentCamera].SetActive(true);
        //         cameraObj[oldPosition].SetActive(false);
        //     }
        // }
    }
}
