using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SwitchToggle : MonoBehaviour {
   [SerializeField]
   private RectTransform uiHandleRectTransform;
   [SerializeField] 
   private Color backgroundActiveColor;
   [SerializeField]
   private Color handleActiveColor;

   private Image backgroundImage, handleImage;
   private Color backgroundDefaultColor, handleDefaultColor;
   private Toggle toggle;
   private Vector2 handlePosition;

   void Awake() {
      toggle = GetComponent <Toggle>();

      handlePosition = uiHandleRectTransform.anchoredPosition;

      backgroundImage = uiHandleRectTransform.parent.GetComponent<Image>();
      handleImage = uiHandleRectTransform.GetComponent<Image>();

      backgroundDefaultColor = backgroundImage.color;
      handleDefaultColor = handleImage.color;

      toggle.onValueChanged.AddListener(OnSwitch);

      if (toggle.isOn)
         OnSwitch(true);
   }

   void OnSwitch(bool on) {
      uiHandleRectTransform.DOAnchorPos(on ? handlePosition * -1 : handlePosition, .4f).SetEase (Ease.InOutBack);

      backgroundImage.DOColor(on ? backgroundActiveColor : backgroundDefaultColor, .6f);

      handleImage.DOColor(on ? handleActiveColor : handleDefaultColor, .4f);
   }

   void OnDestroy() {
      toggle.onValueChanged.RemoveListener(OnSwitch);
   }
}
