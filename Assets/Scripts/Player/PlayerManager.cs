using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : Damagable {
    private HUDManager hudManager;
    public float gameOverSlowDownTime = 3.0f;
    [SerializeField]
    private MouseLook mouseLook;
    [SerializeField]
    private GameObject weaponHolder;
    private WeaponManager weaponManager;


    private void Start() {
        hudManager = GameObject.Find("HUDManager").GetComponent<HUDManager>();
        weaponManager = weaponHolder.GetComponent<WeaponManager>();
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

            other.transform.localPosition = Vector3.zero;
            other.transform.localRotation = Quaternion.Euler(Vector3.zero);
            other.transform.position = weaponHolder.transform.position;
            other.transform.rotation = weaponHolder.transform.rotation;

            other.transform.gameObject.SetActive(false);

            other.transform.SetParent(weaponHolder.transform);
        }
    }

}
