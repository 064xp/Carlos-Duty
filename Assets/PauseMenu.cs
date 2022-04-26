using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject settingsUI;
    public float fadeoutTime;
    public FadeTransition fadeTransition;

    public void ExitToMainMenu() {
        fadeTransition.FadeOut(fadeoutTime, "IntroScreen");
    }

    public void OpenSettings() {
        settingsUI.SetActive(true);
        this.gameObject.SetActive(false);
    }
}
