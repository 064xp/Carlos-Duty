using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroScreen : MonoBehaviour
{
    public float fadeDuration;
    public FadeTransition fade;
    // Start is called before the first frame update
    void Start()
    {
        fade.FadeIn(fadeDuration);
    }
    void Update()
    {
        if(Input.anyKey) {
            fade.FadeOut(fadeDuration);
            StartCoroutine(StartGame());
        }
    }

    IEnumerator StartGame() {
        yield return new WaitForSeconds(fadeDuration);
        SceneManager.LoadScene("MainScene");
    }
}
