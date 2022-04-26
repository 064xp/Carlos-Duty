using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverPanel : MonoBehaviour
{
    public FadeTransition fadeTransition;
    public float fadeDuration;

    public void RestarGame() {
        fadeTransition.FadeOut(fadeDuration, "MainScene");
    }
}
