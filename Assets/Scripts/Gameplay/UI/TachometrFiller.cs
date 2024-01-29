using UnityEngine;
using UnityEngine.UI;

namespace TrophyRace.Architecture {
    public class TachometrFiller : MonoBehaviour {

        private VehicleManager _VehicleManager;
        private Engine _Engine;
        private Image _TachometrImage;
        private int _maxRpm;
        [SerializeField]
        private float _maxFillAmount = 1f;


        void Start() {
            _TachometrImage = GetComponent<Image>();
        }

        void Update() {
            if(_VehicleManager == null) {
                _VehicleManager = GameObject.Find("scripts").GetComponent<VehicleSpawner>().playerVehicle.GetComponent<VehicleManager>();
                _Engine = _VehicleManager.Engine;
            }
            _TachometrImage.fillAmount = (_Engine.rpm / _Engine.maxRpm) * _maxFillAmount;
            TachometrColor();
        }

        private void TachometrColor() {
            /*if(_Engine.rpm > _Engine.maxRpm - 250) {
                _TachometrImage.color = Color.Lerp(Color.white, Color.red, ((_Engine.rpm % 20 == 0) ? 0 : 1));
            } else */if(_Engine.rpm >= (_Engine.maxRpm + _Engine.medRpm) / 2) {
                _TachometrImage.color = Color.Lerp(Color.yellow, Color.red, ((_Engine.rpm - (_Engine.maxRpm + _Engine.medRpm) / 2) / (_Engine.maxRpm / 10)));
            } else if(_Engine.rpm < (_Engine.maxRpm + _Engine.medRpm) / 2 && _Engine.rpm >= _Engine.medRpm) { //_Engine.rpm <= _Engine.medRpm + _Engine.medRpm / 2
                _TachometrImage.color = Color.Lerp(Color.green, Color.yellow, ((_Engine.rpm - _Engine.medRpm) / ((_Engine.maxRpm - _Engine.medRpm) / 2)));
            } else {
                _TachometrImage.color = Color.green;
            }
            // float m = (_Engine.rpm < _Engine.medRpm) ? 0 : (_Engine.rpm - _Engine.medRpm) / (_Engine.maxRpm - _Engine.medRpm);
            // _TachometrImage.color = Color.Lerp(Color.green, Color.red, m);
        }
    }
}