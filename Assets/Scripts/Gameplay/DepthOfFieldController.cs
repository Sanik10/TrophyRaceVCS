using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class DepthOfFieldController : MonoBehaviour {

    private Ray raycast;
    private RaycastHit hit;
    private bool isHit;
    float hitDistance;

    [SerializeField] [Range(1,10)]
    private float focusSpeed = 8;
    [SerializeField]
    private float maxFocusDistance = 100;

    [SerializeField]
    private PostProcessVolume volume;
    private DepthOfField depthOfField;

    void Start() {
        volume.profile.TryGetSettings(out depthOfField);
    }

    // Update is called once per frame
    private void Update() {
        raycast = new Ray(transform.position, transform.forward * maxFocusDistance);

        isHit = false;

        if(Physics.Raycast(raycast, out hit, maxFocusDistance)) {
            isHit = true;
            hitDistance = Vector3.Distance(transform.position, hit.point);
        } else if(hitDistance < maxFocusDistance) {
            hitDistance++;
        }

        SetFocus();
    }

    private void SetFocus() {
        depthOfField.focusDistance.value = Mathf.Lerp(depthOfField.focusDistance.value, hitDistance, Time.deltaTime * focusSpeed);
    }
}
