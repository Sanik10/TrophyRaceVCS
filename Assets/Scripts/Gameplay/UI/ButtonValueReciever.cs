using UnityEngine;
using UnityEngine.UI;

public class ButtonValueReciever : MonoBehaviour {
    
    public float value;

    public void ValueReciever(float btnValue) {
        value = btnValue;
    }
}
