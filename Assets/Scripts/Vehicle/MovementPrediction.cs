using UnityEngine;

public class MovementPrediction : MonoBehaviour {

    private VehicleManager VehicleManager;
    public LineRenderer predictionLine;
    public Rigidbody carRigidbody;
    public float predictionTime = 5f;
    public float predictionResolution = 0.5f;

    private void Start() {
        VehicleManager = GetComponent<VehicleManager>();
        carRigidbody = VehicleManager.PhysicsCalculation.rgdbody;
        predictionLine = gameObject.AddComponent<LineRenderer>();
        // Установка параметров для LineRenderer
        predictionLine.startWidth = 0.1f;
        predictionLine.endWidth = 0.1f;
        predictionLine.material = new Material(Shader.Find("Sprites/Default")); // Пример материала для линии
    }

    private void Update() {
        DrawPredictionLine();
    }

    private void DrawPredictionLine() {
        predictionLine.positionCount = Mathf.CeilToInt(predictionTime / predictionResolution);

        Vector3 currentPosition = carRigidbody.position;
        Vector3 currentVelocity = carRigidbody.velocity;

        for (int i = 0; i < predictionLine.positionCount; i++) {
            float time = i * predictionResolution;
            Vector3 newPos = currentPosition + currentVelocity * time + 0.5f * Physics.gravity * time * time;
            newPos = new Vector3(newPos.x, 0, newPos.z); // новая позиция без изменения по оси Y

            // Устанавливаем позицию в LineRenderer
            predictionLine.SetPosition(i, newPos);
        }
    }
}