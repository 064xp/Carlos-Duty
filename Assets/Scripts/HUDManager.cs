using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HUDManager : MonoBehaviour
{
    public TextMeshProUGUI ammoText;

    public void SetAmmo(int magAmmo, int totalAmmo) {
        ammoText.SetText($"{magAmmo} / {totalAmmo}");
    }
}
