using System;
using System.Collections.Generic;

namespace TrophyRace.Architecture {
    public class StarterSceneConfig : SceneConfig {

        public const string  SCENE_NAME = "Starter";

        public override string sceneName => SCENE_NAME;

        public override Dictionary<Type, Repository> CreateAllRepositories() {
            var _repositoriesMap = new Dictionary<Type, Repository>();

            // this.CreateRepository<BankRepository>(_repositoriesMap);
            this.CreateRepository<VehicleRepository>(_repositoriesMap);

            return _repositoriesMap;
        }

        public override Dictionary<Type, Interactor> CreateAllInteractors() {
            var _interactorsMap = new Dictionary<Type, Interactor>();

            // this.CreateInteractor<BankInteractor>(_interactorsMap);
            this.CreateInteractor<VehicleInteractor>(_interactorsMap);
            this.CreateInteractor<PlayerInteractor>(_interactorsMap);

            return _interactorsMap;
        }
    }
}
