using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TrophyRace.Architecture {
    public abstract class SceneManagerBase {

        public event Action<Scene> OnSceneLoadedEvent;

        public Scene scene {get; private set;}
        public bool isLoading {get; private set;}

        protected Dictionary<string, SceneConfig> _sceneConfigMap;

        public SceneManagerBase() {
            this._sceneConfigMap = new Dictionary<string, SceneConfig>();
        }   

        public abstract void InitScenesMap();  

        public Coroutine LoadCurrentSceneAsync() {
            if(this.isLoading) {
                throw new Exception("Scene is loading now!");
            }

            var sceneName = SceneManager.GetActiveScene().name;
            var config = this._sceneConfigMap[sceneName];
            return Coroutines.StartRoutine(this.LoadCurrentSceneRoutine(config));
        }   


        private IEnumerator LoadCurrentSceneRoutine(SceneConfig sceneConfig) {
            this.isLoading = true;

            yield return Coroutines.StartRoutine(this.InitializeSceneRoutine(sceneConfig));

            this.isLoading = false;
            this.OnSceneLoadedEvent?.Invoke(this.scene);
        }

        public Coroutine LoadNewSceneAsync(string sceneName) {
            if(this.isLoading) {
                throw new Exception("Scene is loading now!");
            }

            var config = this._sceneConfigMap[sceneName];
            return Coroutines.StartRoutine(this.LoadNewSceneRoutine(config));
        }

        private IEnumerator LoadNewSceneRoutine(SceneConfig sceneConfig) {
            this.isLoading = true;

            yield return Coroutines.StartRoutine(this.LoadNewSceneRoutine(sceneConfig));
            yield return Coroutines.StartRoutine(this.InitializeSceneRoutine(sceneConfig));

            this.isLoading = false;
            this.OnSceneLoadedEvent?.Invoke(this.scene);
        }

        private IEnumerator LoadSceneRoutine(SceneConfig sceneConfig) {
            var async = SceneManager.LoadSceneAsync(sceneConfig.sceneName);
            async.allowSceneActivation = false;
            
            while(async.progress < 0.9f) {
                yield return null;
            }

            async.allowSceneActivation = true;
        }

        private IEnumerator InitializeSceneRoutine(SceneConfig sceneConfig) {
            this.scene = new Scene(sceneConfig);

            yield return this.scene.InitialazeAsync();
        }

        public T GetRepository<T>() where T : Repository{
            return this.scene.GetRepository<T>();
        }

        public T GetInteractor<T>() where T : Interactor{
            return this.scene.GetInteractor<T>();
        }
    }
}