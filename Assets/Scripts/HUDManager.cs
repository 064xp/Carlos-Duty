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

    public void SetAmmo(int magAmmo, int totalAmmo) {
        ammoText.SetText($"{magAmmo} / {totalAmmo}");
    }

    public void SetNoWeaponAmmo() {
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
}
