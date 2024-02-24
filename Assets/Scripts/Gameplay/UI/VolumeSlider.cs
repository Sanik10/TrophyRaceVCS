using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class VolumeSlider : MonoBehaviour {

    [SerializeField]
    private string volumeMixer = "GlobalVolume";
    [SerializeField]
    private AudioMixer audioMixer;
    [SerializeField]
    public Slider slider;

    [SerializeField]
    private float _volume;
    [SerializeField]
    private const float _multiplier = 20f;

    private void Start() {
        slider.onValueChanged.AddListener(ChangeVolume);
        _volume = PlayerPrefs.GetFloat(volumeMixer, Mathf.Log10(slider.value) * _multiplier);
        Debug.Log(_volume);
        slider.value = Mathf.Pow(10f, _volume / _multiplier);
    }

    private void ChangeVolume(float value) {
        _volume = Mathf.Log10(value) * _multiplier;
        Debug.Log(_volume);
        audioMixer.SetFloat(volumeMixer, _volume);
    }

    private void OnDisable() {
        PlayerPrefs.SetFloat(volumeMixer, _volume);
    }
}