namespace TrophyRace.Architecture {
    public sealed class StarterSceneManager : SceneManagerBase {
        public override void InitScenesMap() {
            this._sceneConfigMap[StarterSceneConfig.SCENE_NAME] = new StarterSceneConfig();
        }
    }
}