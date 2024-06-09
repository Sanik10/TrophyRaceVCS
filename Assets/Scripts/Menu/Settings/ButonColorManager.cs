using UnityEngine;
using UnityEngine.UI;

public static class ButtonColorManager {

    private static ColorBlock GetNormalColorBlock(ColorBlock colorBlock) {
        colorBlock.normalColor = colorBlock.selectedColor = colorBlock.disabledColor = new Color32(28, 28, 29, 255);
        colorBlock.highlightedColor = new Color32(44, 44, 45, 255);
        colorBlock.pressedColor = new Color32(46, 99, 180, 255);
        return colorBlock;
    }

    private static ColorBlock GetSelectedColorBlock(ColorBlock colorBlock) {
        colorBlock.normalColor = colorBlock.selectedColor = colorBlock.disabledColor = new Color32(46, 90, 180, 255);
        colorBlock.highlightedColor = new Color32(51, 109, 199, 255);
        colorBlock.pressedColor = new Color32(46, 99, 180, 255);
        return colorBlock;
    }

    public static void SetNormalColor(Button button) {
        button.colors = GetNormalColorBlock(button.colors);
        }

    public static void SetSelectedColor(Button button) {
        button.colors = GetSelectedColorBlock(button.colors);
    }
}