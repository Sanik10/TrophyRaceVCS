using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class VolumeSlider : MonoBehaviour {

    public string volumeMixer = "GlobalVolume";
    public AudioMixer audioMixer;
    public Slider slider;

    private float _volume;
    private static float _multiplier = 20;

    private void Awake() {
        slider.onValueChanged.AddListener(ChangeVolume);
    }

    private void Start() {
        _volume = PlayerPrefs.GetFloat(volumeMixer, Mathf.Log10(slider.value) * _multiplier);
        slider.value = Mathf.Pow(10f, _volume / _multiplier);
    }

    private void ChangeVolume(float value) {
        _volume = Mathf.Log10(value) * _multiplier;
        audioMixer.SetFloat(volumeMixer, _volume);
    }

    private void OnDisable() {
        PlayerPrefs.SetFloat(volumeMixer, _volume);
    }
}
