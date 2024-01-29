using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace RaindropFX {
    public class WindController : MonoBehaviour {

        public float maxWindSpeed = 10f; // Максимальная скорость ветра
        public float windDirectionSensitivity = 10f; // Чувствительность к изменению направления

        private PostProcessVolume postProcessVolume;
        private RaindropFX_PPV raindropFX;

        private Vector3 lastForward;

        void Start() {
            postProcessVolume = GetComponent<PostProcessVolume>();

            if (postProcessVolume != null) {
                postProcessVolume.profile.TryGetSettings(out raindropFX);
            } else {
                Debug.LogError("PostProcessingVolume or RaindropFX_PPV not found!");
            }

            if (Camera.main != null) {
                lastForward = Camera.main.transform.forward;
            } else {
                Debug.LogError("Main Camera not found!");
            }
        }

        void Update() {
            if (raindropFX == null)
                return;

            if (Camera.main != null) {
                // Получаем текущее направление взгляда камеры
                Vector3 currentForward = Camera.main.transform.forward;

                // Получаем угол между текущим и предыдущим направлением
                float angleChange = Vector3.SignedAngle(lastForward, currentForward, Vector3.up);

                // Преобразуем изменение угла в скорость ветра
                float windSpeed = Mathf.Clamp(-angleChange * windDirectionSensitivity, -maxWindSpeed, maxWindSpeed);

                // Устанавливаем значения ветра в компонент RaindropFX_PPV
                raindropFX.wind.value = new Vector2(windSpeed, 0f);

                // Обновляем предыдущее направление
                lastForward = currentForward;
            } else {
                Debug.LogError("Main Camera not found!");
            }
        }
    }
}