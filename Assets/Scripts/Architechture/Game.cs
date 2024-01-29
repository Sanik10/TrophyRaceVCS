using System;
using System.Collections;

namespace TrophyRace.Architecture {
    public static class Game {

        public static event Action OnGameInitializedEvent;
        public static SceneManagerBase sceneManager {get; private set;}

        public static void Run() {
            sceneManager = new MainMenuSceneManager();
            Coroutines.StartRoutine(InitialazeGameRoutine());
        }

        private static IEnumerator InitialazeGameRoutine() {
            sceneManager.InitScenesMap();
            yield return sceneManager.LoadCurrentSceneAsync();
            OnGameInitializedEvent?.Invoke();
        }

        public static T GetInteractor<T>() where T : Interactor {
            return sceneManager.GetInteractor<T>();
        }

        public static T GetRepository<T>() where T : Repository {
            return sceneManager.GetRepository<T>();
        }
    }
}