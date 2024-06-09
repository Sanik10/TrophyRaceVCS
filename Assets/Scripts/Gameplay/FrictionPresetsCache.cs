using UnityEngine;
using System.Collections.Generic;

public class FrictionPresetsCache : MonoBehaviour
{
    private static FrictionPresetsCache instance;
    private Dictionary<string, FrictionPreset> frictionPresets = new Dictionary<string, FrictionPreset>();

    public static FrictionPresetsCache Instance {
        get {
            if(instance == null) {
                instance = FindObjectOfType<FrictionPresetsCache>();
                if(instance == null) {
                    GameObject obj = new GameObject();
                    obj.name = "FrictionPresetsCache";
                    instance = obj.AddComponent<FrictionPresetsCache>();
                    DontDestroyOnLoad(obj); // Сохраняем объект при загрузке новой сцены
                }
            }
            return instance;
        }
    }

    private void Awake() {
        LoadFrictionPresets();
    }

    private void LoadFrictionPresets() {
        FrictionPreset[] presets = Resources.LoadAll<FrictionPreset>("FrictionPresets");
        foreach (FrictionPreset preset in presets) {
            frictionPresets[preset.name.ToString()] = preset;
        }
    }

    public FrictionPreset GetFrictionPreset(string surfaceType) {
        if (frictionPresets.ContainsKey(surfaceType)) {
            return frictionPresets[surfaceType];
        } else {
            Debug.LogWarning("FrictionPreset for surface type " + surfaceType + " not found.");
            return null;
        }
    }
}