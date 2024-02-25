namespace TrophyRace.Architecture {
    public sealed class DesertTestSceneManager : SceneManagerBase {
        
        public override void InitScenesMap() {
            this._sceneConfigMap[DesertTestSceneConfig.SCENE_NAME] = new DesertTestSceneConfig();
        }
    }
}