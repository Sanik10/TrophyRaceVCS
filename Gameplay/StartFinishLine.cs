using UnityEngine;
using System;
using TrophyRace.Architecture;

public class StartFinishLine : MonoBehaviour {

    public static event Action<GameObject> startFinishLinePassedEvent;

    private void OnTriggerEnter(Collider collider) {
        GameObject mainObject = GetMainObject(collider.gameObject);

        if (mainObject != null) {
            startFinishLinePassedEvent?.Invoke(mainObject);
            Debug.Log("Line passed");
        }
    }

    private GameObject GetMainObject(GameObject obj) {
        // Получаем родительский объект коллайдера
        Transform parent = obj.transform.parent.parent;

        // Проверяем тэг у родительского объекта
        if (parent != null) {
            Debug.Log(parent.gameObject);
            return parent.gameObject;
        }

        return null;
    }
}