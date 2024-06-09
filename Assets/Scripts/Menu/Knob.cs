using UnityEngine;
using UnityEngine.UI;

public class Knob : MonoBehaviour {

    public SwipeLayout script;

    void Start() {
        // script = GameObject.Find("MapsButtonsContainer").GetComponent<SwipeLayout>();
        // script = transform.parent.GetComponent<SwipeLayout>();
    }
    
    public void OnKnobClicked(Button btn) {
        script.OnKnobClicked(btn);
    }
}