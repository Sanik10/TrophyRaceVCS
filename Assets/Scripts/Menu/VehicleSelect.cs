using UnityEngine;

public class VehicleSelect : MonoBehaviour {
    public string objectName;
    public int id;

    public void saveInt() {
        PlayerPrefs.SetInt(objectName, id);
    }
}
