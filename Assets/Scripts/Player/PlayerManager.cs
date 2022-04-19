using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : Damagable {
    private HUDManager hudManager;
    public float gameOverSlowDownTime = 3.0f;
    [SerializeField]
    private MouseLook mouseLook;
    [SerializeField]
    private WeaponManager weaponManager;
    [HideInInspector]
    public int maxHealth { get; private set; } = 100;


    private void Start() {
        hudManager = GameObject.Find("HUDManager").GetComponent<HUDManager>();
        hudManager.SetHealth(health);
        maxHealth = health;
    }

    public override void TakeDamage(int damage) {
        base.TakeDamage(damage);
        hudManager.SetHealth(health);
    }

    public override void Die() {
        hudManager.OnGameOver("You died!");
        StartCoroutine(LerpTimeScaleTo(0.0f));
        mouseLook.enabled = false;
    }

    IEnumerator LerpTimeScaleTo(float value) {
        float elapsedTime = 0f;
        float initialTimeScale = Time.timeScale;

        while(elapsedTime <= gameOverSlowDownTime) {
            elapsedTime += Time.deltaTime;
            Time.timeScale = Mathf.Lerp(initialTimeScale, value, elapsedTime / gameOverSlowDownTime);
            yield return null;
        }

    }

    public void SetHealth(int newHealth) {
        if (newHealth > maxHealth) health = maxHealth;
        else if (newHealth < 0) health = 0;
        else health = newHealth;
        hudManager.SetHealth(health, maxHealth);
    }

    private void OnTriggerStay(Collider other) {
       if(other.gameObject.CompareTag("Equipable")) {
            weaponManager.PickupWeapon(other.gameObject);
        }
    }

}
