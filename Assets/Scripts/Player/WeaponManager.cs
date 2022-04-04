using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public Weapon selectedWeapon { get; private set; }
    private int selectedWeaponIndex = 0;
    [SerializeField]
    private HUDManager hudManager;
    [SerializeField]
    private Camera fpsCamera;
    float initialCamFOV;

    private void Start() {
        initialCamFOV = fpsCamera.fieldOfView;
        SelectWeapon();
    }

    private void SelectWeapon() {
        int i = 0;

        // Reset cam FOV when weapon switches
        fpsCamera.fieldOfView = initialCamFOV;

        foreach(Transform weapon in transform) {
            if (i == selectedWeaponIndex) {
                weapon.gameObject.SetActive(true);
                selectedWeapon = weapon.GetComponent<Weapon>();
                selectedWeapon.UsedByAI = false;

                //// ===== on pickup
                // Enable animator
                weapon.gameObject.GetComponent<Animator>().enabled = true;
                // disable collider
                weapon.gameObject.GetComponent<BoxCollider>().enabled = false;
                // call pickup method on weapon
                hudManager.SetAmmo(selectedWeapon.MagazineAmmo, selectedWeapon.Ammo);
            } else {
                weapon.gameObject.SetActive(false);
            }
            i++;
        }
    }

    public void SwitchWeapon(int weaponIndex) {
        if (weaponIndex > transform.childCount - 1) return;

        selectedWeaponIndex = weaponIndex;
        SelectWeapon();
    }

    public void SwitchToNextWeapon() {
        if (transform.childCount < 1) return;

        if (selectedWeaponIndex >= transform.childCount - 1)
            selectedWeaponIndex = 0;
        else
            selectedWeaponIndex++;

        SelectWeapon();
    }

    public void SwitchToPreviousWeapon() {
        if (transform.childCount < 1) return;

        if (selectedWeaponIndex <= 0)
            selectedWeaponIndex = transform.childCount - 1;
        else
            selectedWeaponIndex--;

        SelectWeapon();
    }

}
