using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public Equipable EquipedItem { get; private set; }
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

        foreach(Transform item in transform) {
            if (i == selectedWeaponIndex) {
                item.gameObject.SetActive(true);
                EquipedItem = item.GetComponent<Equipable>();
                EquipedItem.OnEquip();
            } else {
                item.gameObject.SetActive(false);
            }
            i++;
        }

        if(transform.childCount == 0) {
            hudManager.SetNoWeaponAmmo();
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

    public void PickupWeapon(GameObject gameObject) {
        Weapon weapon = gameObject.GetComponent<Weapon>();
        Weapon equippedWeapon = FindEquippedWeapon(weapon);

        // If weapon is already equipped
        if (equippedWeapon != null) {
            if(equippedWeapon.Ammo < equippedWeapon.Settings.startAmmo) {
                equippedWeapon.Ammo = equippedWeapon.Ammo + weapon.Ammo;
                if (equippedWeapon.Ammo > equippedWeapon.Settings.startAmmo) equippedWeapon.Ammo = equippedWeapon.Settings.startAmmo;

                Destroy(gameObject);
            }
        } else {
            gameObject.transform.localPosition = Vector3.zero;
            gameObject.transform.localRotation = Quaternion.Euler(Vector3.zero);
            gameObject.transform.position = transform.position;
            gameObject.transform.rotation = transform.rotation;

            if (transform.childCount > 0)
                gameObject.transform.gameObject.SetActive(false);

            gameObject.transform.SetParent(transform);

            weapon.OnPickup();
        }

        SelectWeapon();
    }

    Weapon FindEquippedWeapon(Weapon weapon) {
        foreach(Transform gun in transform) {
            Weapon currentWeapon = gun.gameObject.GetComponent<Weapon>();
            if (currentWeapon.GetName() == weapon.GetName()) {
                return currentWeapon;
            }
        }
        return null;
    }

    public int GetWeaponCount() {
        return transform.childCount;
    }
}
