using System;
using System.Collections.Generic;

namespace TrophyRace.Architecture {
    public class InteractorsBase {

        private Dictionary<Type, Interactor> _interactorsMap;
        private SceneConfig _sceneConfig;

        public InteractorsBase(SceneConfig sceneConfig) {
            this._sceneConfig = sceneConfig;
        }

        public void CreateAllInteractors() {
            this._interactorsMap = this._sceneConfig.CreateAllInteractors();
        }

        public void SendOnCreateToAllInteractors() {
            var allInteractors = this._interactorsMap.Values;
            foreach (var interactor in allInteractors) {
                interactor.OnCreate();
            }
        }

        public void InitializeAllInteractors() {
            var allInteractors = this._interactorsMap.Values;
            foreach (var interactor in allInteractors) {
                interactor.Initialize();
            }
        }

        public void SendOnStartToAllInteractors() {
            var allInteractors = this._interactorsMap.Values;
            foreach (var interactor in allInteractors) {
                interactor.OnStart();
            }
        }

        public T GetInteractor<T>() where T : Interactor {
            var type = typeof(T);
            return (T) this._interactorsMap[type];
        }
    }
}