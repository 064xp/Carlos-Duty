using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {
    public WeaponSettings Settings;
    public bool UsedByAI = false;
    public bool canShoot = false;
    public int Ammo;
    public int MagazineAmmo;

    public void SetCanShootTrue() {
        canShoot = true;
    }

    virtual public string GetName() {
        return "NO_NAME_SET";
    }

}
