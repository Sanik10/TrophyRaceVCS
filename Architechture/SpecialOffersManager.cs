using UnityEngine;

public class SpecialOffersManager : MonoBehaviour {

    private static SpecialOffersManager _instance;
    public static SpecialOffersManager Instance => _instance;

    // Добавьте дополнительные переменные или методы, чтобы отслеживать акцию

    private void Awake() {
        if (_instance == null) {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }
}