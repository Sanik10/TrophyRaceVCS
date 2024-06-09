using UnityEngine;
using UnityEngine.UI;

public class InteractablePanelSwitcher : MonoBehaviour {

    public GameObject Panel;

    private void Start() {
        CheckInteract();
    }

    public void CheckInteract() {
        Panel.SetActive((GetComponent<Button>().interactable == false) ? true : false);
    }
}
