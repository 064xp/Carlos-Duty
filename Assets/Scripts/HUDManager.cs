using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDManager : MonoBehaviour
{
    public TextMeshProUGUI ammoText;
    public GameObject gameOverPanel;
    public TextMeshProUGUI gameOverReasonText;
    public TextMeshProUGUI healthText;
    public Image healthIconFill;
    public Slider churchHealthBar;

    public void SetAmmo(int magAmmo, int totalAmmo) {
        ammoText.SetText($"{magAmmo} / {totalAmmo}");
    }

    public void SetNoWeaponAmmo() {
        ammoText.SetText("");
    }

    public void SetEquipableAmount(int amount) {
        ammoText.SetText($"{amount}");
    }

    public void OnGameOver(string reason) {
        gameOverPanel.SetActive(true);
        gameOverReasonText.SetText(reason);
    }

    public void SetHealth(int health, int maxHealth = 100) {
        healthText.SetText(health.ToString());
        healthIconFill.fillAmount = (float) NumberUtils.Map(health, 0, maxHealth, 0, 1);
    }

    public void SetChurchHealth(int health) {
        //StartCoroutine(UpdateChurchHealth(health));
        churchHealthBar.value = health;
    }

    IEnumerator UpdateChurchHealth(int newValue) {
        float speed = 2f;
        while(churchHealthBar.value != newValue) {
            churchHealthBar.value = Mathf.MoveTowards(churchHealthBar.value, newValue, Time.deltaTime * speed);
            yield return null;
        }
    }

    public void SetChurchMaxHealth(int value) {
        churchHealthBar.maxValue = (float) value;
    }
}
