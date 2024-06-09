using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CareerSelection : MonoBehaviour {

    [SerializeField]
    private GameObject SeasonsScrollView;

    [SerializeField]
    private GameObject[] MapsScrollView;

    private void Start() {
        foreach(Transform i in gameObject.transform) {
            if(i.transform.name == "SeasonsScrollView") {
                SeasonsScrollView = i.gameObject; 
            }
            if(i.transform.name == "MapsScrollView") {
                MapsScrollView = new GameObject[i.transform.childCount];
                for(int q = 0; q < i.transform.childCount; q++) {
                    MapsScrollView[q] = i.GetChild(q).gameObject;
                }
            }
        }
        SeasonsScrollView.SetActive(true);
        for(int i = 0; i < MapsScrollView.Length; i++) {
            MapsScrollView[i].SetActive(false);
        }
    }

    public void OpenMapsScrollView(int index) {
        for(int i = 0; i < MapsScrollView.Length; i++) {
            MapsScrollView[i].SetActive(i == index ? true : false);
        }
        SeasonsScrollView.SetActive(false);
    }

    public void OpenSeasonsScrollView() {
        for(int i = 0; i < MapsScrollView.Length; i++) {
            MapsScrollView[i].SetActive(false);
        }
        SeasonsScrollView.SetActive(true);
    }
}