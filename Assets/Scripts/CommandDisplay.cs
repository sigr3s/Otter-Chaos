using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CommandDisplay : MonoBehaviour {
    public Image image;
    public Text text;

    public void Display(Sprite sprite){
        image.sprite = sprite;

        image.DOFade(1, 0.25f);
        text.DOFade(0, 0.25f);
    }

    public void Display(string content){
        image.DOFade(0, 0.25f);
        text.DOFade(1, 0.25f);

        text.text = content;
    }
}