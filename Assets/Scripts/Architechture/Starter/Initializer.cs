using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.Localization.Settings;

namespace TrophyRace.Architecture {
    public class Initializer : MonoBehaviour {

        public string mainSceneName = "MainMenu"; // Название основной сцены
        public AudioMixer audioMixer;
        private bool settingsApplied = false;
        private bool vehicleListInitialized = false;

        private static bool isVehicleListInitialized = false; // Флаг для проверки инициализации

        private void Start() {
            // Game.Run();
            // Game.OnGameInitializedEvent += OnGameInitialized;
            // Применение настроек из PlayerPrefs
            ApplyPlayerPrefsSettings();

            ShowLoadingScreen();

            // Начало инициализации VehicleList
            StartCoroutine(InitializeVehicleList());
        }

        // private void OnGameInitialized() {
        //     Game.OnGameInitializedEvent -= OnGameInitialized;
        //     var playerInteractor = Game.GetInteractor<PlayerInteractor>();
        //     var player = playerInteractor.player;
        // }

        private void ApplyPlayerPrefsSettings() {
            if(PlayerPrefs.HasKey("LanguagePrefs")) {
                Debug.Log("ID языка: " + PlayerPrefs.GetInt("LanguagePrefs"));
                LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[PlayerPrefs.GetInt("LanguagePrefs")];
            }

            if(PlayerPrefs.HasKey("FrameRatePrefs")) {
                int targetFps = PlayerPrefs.GetInt("FrameRatePrefs");
                Debug.Log("Установлоенный fps: " + targetFps);
                Application.targetFrameRate = targetFps;
            }

            if(PlayerPrefs.HasKey("QualityPrefs")) {
                int qualityPrefs = PlayerPrefs.GetInt("QualityPrefs");
                Debug.Log("устанровленный уровень графики: " + qualityPrefs);
                QualitySettings.SetQualityLevel(qualityPrefs, true);
            }

            if(PlayerPrefs.HasKey("GlovalVolume")) {
                string volumeMixer = "GlovalVolume";
                audioMixer.SetFloat(volumeMixer, PlayerPrefs.GetFloat(volumeMixer));
            }

            if(PlayerPrefs.HasKey("VehicleVolume")) {
                string volumeMixer = "VehicleVolume";
                audioMixer.SetFloat(volumeMixer, PlayerPrefs.GetFloat(volumeMixer));
            }

            if(PlayerPrefs.HasKey("MusicVolume")) {
                string volumeMixer = "MusicVolume";
                audioMixer.SetFloat(volumeMixer, PlayerPrefs.GetFloat(volumeMixer));
            }

            settingsApplied = true; // Помечаем, что настройки применены
        }

        private IEnumerator InitializeVehicleList() {
            // Ждем пока настройки будут применены
            while(!settingsApplied) {
                yield return null;
            }

            // Проверяем, инициализирован ли уже VehicleList
            if(!isVehicleListInitialized) {
                // Создание объекта VehicleList и перемещение его в DontDestroyOnLoad
                GameObject vehicleListObject = new GameObject("VehicleList");
                VehicleList vehicleList = vehicleListObject.AddComponent<VehicleList>();
                DontDestroyOnLoad(vehicleListObject);

                // Показать загрузочный экран (если требуется)

                // Подписка на событие инициализации VehicleList
                VehicleList.vehicleListInitializedEvent += OnVehicleListInitialized;
                vehicleList.Initialize(); // Вызов инициализации вручную, если это требуется

                // Ждем пока VehicleList не будет инициализирован
                while (!vehicleListInitialized) {
                    yield return null;
                }

                isVehicleListInitialized = true; // Устанавливаем флаг после инициализации
            }

            // Переход на основную сцену после инициализации VehicleList
            Debug.Log("Открывается Меню");
            SceneManager.LoadScene(mainSceneName);
        }

        private void ShowLoadingScreen() {
            // Логика для отображения загрузочного экрана (если требуется)
            Debug.Log("Showing loading screen...");
        }

        private void OnVehicleListInitialized() {
            vehicleListInitialized = true; // Помечаем, что VehicleList инициализирован
        }
    }
}