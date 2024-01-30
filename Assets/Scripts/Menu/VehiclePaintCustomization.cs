using UnityEngine;
using UnityEngine.UI;

namespace TrophyRace.Architecture {
    public class VehiclePaintCustomization : MonoBehaviour {

        private VehicleSpawner _VehicleSpawner;
        private VehicleBodyPainter _VehicleBodyPainter;

        private void Start() {
            _VehicleSpawner = GameObject.Find("scripts").GetComponent<VehicleSpawner>();
            _VehicleBodyPainter = GameObject.Find("VehicleCustomizationMenu").GetComponent<VehicleBodyPainter>();
        }

        public void GetButtonNumber(Button btn) {
            VehicleManager _VehicleManager = _VehicleSpawner.playerVehicle.GetComponent<VehicleManager>();
            Transform parent = btn.transform.parent.transform;
            Transform pressedButton = btn.transform;
            int i = 0;
            foreach(Transform child in parent) {
                if(child == pressedButton) {
                    _VehicleManager.VehicleVFX.ChangeColor(_VehicleBodyPainter.currentCustomizationWindow, i);
                    // CallFunction(i);
                    break;
                }
                i++;
            }
            var VehicleData = _VehicleManager.vehicleData;
            if(_VehicleBodyPainter.currentCustomizationWindow == 0) {
                VehicleData.bodyMaterailId = i;
            } else if(_VehicleBodyPainter.currentCustomizationWindow == 1) {
                VehicleData.diskMaterailId = i;
            }
            VehicleData.Save("VehiclePaintCustomization");
        }
    }
}