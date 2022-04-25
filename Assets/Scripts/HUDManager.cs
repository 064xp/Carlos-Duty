using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDManager : MonoBehaviour
{
    public enum AmmoTypes {
        Bullets,
        Bottle
    }

    [Header("Ammo")]
    public TextMeshProUGUI magAmmoText;
    public TextMeshProUGUI ammoText;
    public Transform ammoTypeGraphics;
    [Header("Ammo Type Graphics")]
    public GameObject bulletsGraphic;
    public GameObject bottleGraphic;
    [Header("GameOver")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI gameOverReasonText;
    [Header("Player Health")]
    public TextMeshProUGUI healthText;
    public Image healthIconFill;
    [Header("Church Health")]
    public Slider churchHealthBar;

    
    public void SetAmmo(int magAmmo, int totalAmmo) {
        magAmmoText.SetText($"{magAmmo}");
        ammoText.SetText($"/{totalAmmo}");
    }

    public void SetAmmoType(AmmoTypes type) {
        foreach(Transform child in ammoTypeGraphics) {
            child.gameObject.SetActive(false);
        }

        switch (type) {
            case AmmoTypes.Bullets:
                bulletsGraphic.SetActive(true);
                break;
        }
    }

    public void SetNoWeaponAmmo() {
        foreach(Transform child in ammoTypeGraphics) {
            child.gameObject.SetActive(false);
        }
        ammoText.SetText("");
        magAmmoText.SetText("");
    }

    public void SetEquipableAmount(int amount) {
        magAmmoText.SetText($"{amount}");
        ammoText.SetText("");
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
        churchHealthBar.value = health;
    }

    public void SetChurchMaxHealth(int value) {
        churchHealthBar.maxValue = (float) value;
    }
}
