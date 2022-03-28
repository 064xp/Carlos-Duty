using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HUDManager : MonoBehaviour
{
    public TextMeshProUGUI ammoText;
    public GameObject gameOverPanel;
    public TextMeshProUGUI gameOverReasonText;

    public void SetAmmo(int magAmmo, int totalAmmo) {
        ammoText.SetText($"{magAmmo} / {totalAmmo}");
    }

    public void OnGameOver(string reason) {
        gameOverPanel.SetActive(true);
        gameOverReasonText.SetText(reason);
    }
}
