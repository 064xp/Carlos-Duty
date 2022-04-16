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


    private void Start() {
        hudManager = GameObject.Find("HUDManager").GetComponent<HUDManager>();
        hudManager.SetHealth(health);
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

    private void OnTriggerStay(Collider other) {
       if(other.gameObject.CompareTag("Weapon")) {
            weaponManager.PickupWeapon(other.gameObject);
        }
    }

}
