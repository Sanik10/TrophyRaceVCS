using System;
using System.Collections.Generic;

namespace TrophyRace.Architecture {
    public class RepositoriesBase {

        private Dictionary<Type, Repository> _repositoriesMap;
        private SceneConfig _sceneConfig;

        public RepositoriesBase(SceneConfig sceneConfig) {
            this._sceneConfig = sceneConfig;
        }

        public void CreateAllRepositories() {
            this._repositoriesMap = this._sceneConfig.CreateAllRepositories();
        }

        // private void CreateRepository<T>() where T : Repository, new() {
        //     var repository = new T();
        //     var type = typeof(T);
        //     this._repositoriesMap[type] = repository;
        // }

        public void SendOnCreateToAllRepositories() {
            var allRepositories = this._repositoriesMap.Values;
            foreach (var repository in allRepositories) {
                repository.OnCreate();
            }
        }

        public void InitializeAllRepositories() {
            var allRepositories = this._repositoriesMap.Values;
            foreach (var repository in allRepositories) {
                repository.Initialize();
            }
        }

        public void SendOnStartToAllRepositories() {
            var allRepositories = this._repositoriesMap.Values;
            foreach (var repository in allRepositories) {
                repository.OnStart();
            }
        }

        public T GetRepository<T>() where T : Repository {
            var type = typeof(T);
            return (T) this._repositoriesMap[type];
        }
    }
}