namespace TrophyRace.Architecture {
    public sealed class DesertSceneManager : SceneManagerBase {
        
        public override void InitScenesMap() {
            this._sceneConfigMap[DesertSceneConfig.SCENE_NAME] = new DesertSceneConfig();
        }
    }
}