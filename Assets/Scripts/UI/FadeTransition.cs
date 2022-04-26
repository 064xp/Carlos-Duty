using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FadeTransition : MonoBehaviour {
    public Image image;

    public void FadeIn(float duration) {
        image.CrossFadeAlpha(1f, 0f, true);
        image.CrossFadeAlpha(0f, duration, true);
    }

    public void FadeOut(float duration) {
        image.CrossFadeAlpha(0f, 0f, true);
        image.CrossFadeAlpha(1f, duration, true);
    }
    public void FadeOut(float duration, string scene) {
        image.CrossFadeAlpha(0f, 0f, true);
        image.CrossFadeAlpha(1f, duration, true);
        StartCoroutine(LoadSceneAfterTime(scene, duration));
    }

    private IEnumerator LoadSceneAfterTime(string scene, float time) {
        float startTime = Time.realtimeSinceStartup;

        while(Time.realtimeSinceStartup - startTime < time) {
            yield return null;
        }

        SceneManager.LoadScene(scene);
    }
}
