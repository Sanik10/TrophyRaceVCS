namespace TrophyRace.Architecture {
    public sealed class MainMenuSceneManager : SceneManagerBase {
        public override void InitScenesMap() {
            this._sceneConfigMap[MainMenuSceneConfig.SCENE_NAME] = new MainMenuSceneConfig();
        }
    }
}