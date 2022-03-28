using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : Damagable {
    public HUDManager hudManager;
    public float slowDownTime = 3.0f;

    private void Start() {
        hudManager = GameObject.Find("HUDManager").GetComponent<HUDManager>();
    }

    public override void TakeDamage(int damage) {
        base.TakeDamage(damage);
        hudManager.SetHealth(health);
    }

    public override void Die() {
        hudManager.OnGameOver("You died!");
        StartCoroutine(LerpTimeScaleTo(0.0f));
    }

    IEnumerator LerpTimeScaleTo(float value) {
        float elapsedTime = 0f;
        float initialTimeScale = Time.timeScale;

        while(elapsedTime <= slowDownTime) {
            elapsedTime += Time.deltaTime;
            Time.timeScale = Mathf.Lerp(initialTimeScale, value, elapsedTime / slowDownTime);
            yield return null;
        }

    }

}
