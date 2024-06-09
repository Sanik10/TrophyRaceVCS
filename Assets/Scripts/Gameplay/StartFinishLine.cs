using UnityEngine;
using System;
using TrophyRace.Architecture;

public class StartFinishLine : MonoBehaviour {

    public static event Action<GameObject> startFinishLinePassedEvent;

    private void OnTriggerEnter(Collider collider) {
        GameObject mainObject = GetMainObject(collider.gameObject);

        if (mainObject != null && mainObject.GetComponent<VehicleManager>() != null) {
            startFinishLinePassedEvent?.Invoke(mainObject);
            Debug.Log("Line passed by " + mainObject.name);
        }
    }

    private GameObject GetMainObject(GameObject obj) {
        // Получаем родительский объект коллайдера
        Transform parent = obj.transform.parent;

        // Ищем объект, на котором есть скрипт VehicleManager
        while (parent != null) {
            if (parent.GetComponent<VehicleManager>() != null) {
                Debug.Log(parent.gameObject.name);
                return parent.gameObject;
            }
            parent = parent.parent;
        }

        return null;
    }
}