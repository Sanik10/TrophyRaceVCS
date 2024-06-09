using UnityEngine;
using UnityEngine.UI;

public class MapSelector : MonoBehaviour {
    
    public Scrollbar _scrollbar;
    public float scrollPos = 0;
    public float[] pos;
    public float distance;

    private void Start() {
        _scrollbar = GameObject.Find("scrollbarHorizontal").GetComponent<Scrollbar>();
        pos = new float[transform.childCount];
        distance = 1f / (pos.Length - 1f);
        for(int i = 0; i < pos.Length; i++) {
            pos[i] = distance * i;
        }
    }

    private void Update() {
        if(Input.GetMouseButton(0)) {
            scrollPos = _scrollbar.value;
        } else {
            for (int i = 0; i < pos.Length; i++) {
                if(scrollPos < pos[i] + (distance / 2) && scrollPos > pos[i] - (distance / 2)) {
                    _scrollbar.value = Mathf.Lerp(_scrollbar.value, pos[i], Time.deltaTime);
                }
            }
        }

    }
}
