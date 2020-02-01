using UnityEngine;
using UnityEngine.UI;

public class CommandDisplay : MonoBehaviour {
    public Image image;


    public void Display(Sprite sprite){
        image.sprite = sprite;
    }
}