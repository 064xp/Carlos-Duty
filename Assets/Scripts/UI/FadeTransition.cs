using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeTransition : MonoBehaviour {
    Image image;
    // Start is called before the first frame update
    void Start() {
        image = GetComponent<Image>();
    }

    public void FadeIn(float duration) {
        image.CrossFadeAlpha(1f, 0f, true);
        image.CrossFadeAlpha(0f, duration, true);
    }
    public void FadeOut(float duration) {
        image.CrossFadeAlpha(0f, 0f, true);
        image.CrossFadeAlpha(1f, duration, true);
    }
}
