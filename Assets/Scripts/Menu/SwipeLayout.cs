using UnityEngine;
using UnityEngine.UI;

public class SwipeLayout : MonoBehaviour {
    [Range(1, 20)]
    public int transitionSpeed = 5;
    [Range(0, 1)]
    public float neighbourReductionPercentage = 0.5f;
    [Range(0, 90)]
    public int neighbourRotation = 30;
    public bool scrollWhenReleased = true;
    [Range(0.0001f, 1)]
    public float scrollStopSpeed = 1;
    [SerializeField] Sprite[] knobs;
    [SerializeField] GameObject knob;
    public Transform knobContainer;
    public Scrollbar scrollbar;

    private Scrollbar _Scrollbar;
    private Transform _KnobContainer;
    private Vector2 _neighbourScale;
    private Quaternion _neighbourRotation;
    private Vector2 _mainScale;
    private Quaternion _mainRotation;
    private int _childCount = 0;
    private float[] _attractionPoints;
    private float _scrollbarValue = 0;
    private float _subdivisionDistance = 0;
    private float _attractionPoint = 0;
    private bool _knobClicked = false;

    private void Start() {
        this._attractionPoints = new float[transform.childCount];
        this._childCount = this._attractionPoints.Length;
        this._subdivisionDistance = 1f / (this._childCount - 1f);
        for(int i = 0; i < this._childCount; i++) {
            this._attractionPoints[i] = this._subdivisionDistance * i;
            knob.GetComponent<Knob>().script = this;
            Instantiate(knob, this.knobContainer);
        }
        foreach(Transform child in transform) {
            child.localScale = new Vector2(neighbourReductionPercentage, neighbourReductionPercentage);
            child.localRotation = Quaternion.Euler(0, neighbourRotation, 0);
        }
        if(this._childCount > 0) {
            this.knobContainer.GetChild(0).GetComponent<Image>().sprite = knobs[0];
            transform.GetChild(0).localScale = Vector2.one;
            transform.GetChild(0).localRotation = Quaternion.Euler(0, 0, 0);
        }
    }

    private void Update() {
        if(!this._knobClicked && (Input.GetMouseButton(0) || (scrollWhenReleased && GetScrollSpeed() > scrollStopSpeed))) {
            this._scrollbarValue = this.scrollbar.value;
            FindAttractionPoint();
            UpdateUI();
        } else if(IsBeingScaled()) {
            this.scrollbar.value = Mathf.Lerp(this.scrollbar.value, this._attractionPoint, transitionSpeed * Time.deltaTime);
            UpdateUI();
        } else {
            this._knobClicked = false;
        }
    }

    private void FindAttractionPoint() {
        if(this._scrollbarValue < 0) {
            this._attractionPoint = 0;
        } else {
            for(int i = 0; i < this._childCount; i++) {
                if(this._scrollbarValue < this._attractionPoints[i] + this._subdivisionDistance / 2 && this._scrollbarValue > this._attractionPoints[i] - this._subdivisionDistance / 2) {
                    this._attractionPoint = this._attractionPoints[i];
                    break;
                }
                if(i == this._childCount - 1) {
                    this._attractionPoint = this._attractionPoints[i];
                }
            }
        }
    }

    private void UpdateUI() {
        for(int i = 0; i < this._attractionPoints.Length; i++) {
            if(this._attractionPoints[i] == this._attractionPoint) {
                this.knobContainer.GetChild(i).GetComponent<Image>().sprite = knobs[0];
                this._mainScale = Vector2.Lerp(transform.GetChild(i).localScale, Vector2.one, transitionSpeed * Time.deltaTime); // 1-20 (4)
                this._mainRotation = Quaternion.Euler(0, 0, 0);
                transform.GetChild(i).localScale = this._mainScale;
                transform.GetChild(i).localRotation = Quaternion.Slerp(transform.GetChild(i).localRotation, _mainRotation, transitionSpeed * Time.deltaTime);
            } else {
                this.knobContainer.GetChild(i).GetComponent<Image>().sprite = knobs[1];
                this._neighbourScale = Vector2.Lerp(transform.GetChild(i).localScale, new Vector2(neighbourReductionPercentage, neighbourReductionPercentage), transitionSpeed * Time.deltaTime); // 1-20 (4)
                this._neighbourRotation = Quaternion.Euler(0, neighbourRotation, 0);
                transform.GetChild(i).localScale = this._neighbourScale;
                transform.GetChild(i).localRotation = Quaternion.Slerp(transform.GetChild(i).localRotation, _neighbourRotation, transitionSpeed * Time.deltaTime);
            }
        }
    }

    public void OnKnobClicked(Button btn) {
        this._knobClicked = true;
        Transform parent = btn.transform.parent.transform;
        Transform pressedButton = btn.transform;
        int i = 0;
        foreach(Transform child in parent) {
            if(child == pressedButton) {
                this._attractionPoint = this._attractionPoints[i];
                break;
            }
            i++;
        }
    }

    float GetScrollSpeed() {
        return Mathf.Abs(this._scrollbarValue - this.scrollbar.value) / Time.deltaTime;
    }

    bool IsBeingScaled() {
        return Mathf.Abs(this.scrollbar.value - this._attractionPoint) > 0.01f || this._mainScale.x < 0.99f || this._neighbourScale.x > neighbourReductionPercentage + 0.01f;
    }
}