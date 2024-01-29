using UnityEngine;
using UnityEngine.UI;

namespace TrophyRace.Architecture {
    public class PaintButton : MonoBehaviour {
        
        private VehiclePaintCustomization script;

        void Start() {
            script = GameObject.Find("VehiclePaintCustomizationMenu").GetComponent<VehiclePaintCustomization>();
        }
        
        public void SendButtonNumber(Button btn) {
            script.GetButtonNumber(btn);
        }
    }
}