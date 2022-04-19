using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : Equipable {
    public WeaponSettings Settings;
    public bool UsedByAI = false;
    public bool canShoot = false;
    public int Ammo;
    public int MagazineAmmo;
    protected HUDManager hud;

    public Weapon() {
        type = Types.Weapon;
    }

    public void SetCanShootTrue() {
        canShoot = true;
    }

    override public string GetName() {
        return Settings.name;
    }
    override public void OnPickup() {
        animator.enabled = true;
        GetComponent<BoxCollider>().enabled = false;
        hud = GameObject.Find("HUDManager").GetComponent<HUDManager>();
        UsedByAI = false;
    }

    override public void OnEquip() {
        hud.SetAmmo(MagazineAmmo, Ammo);
    }

    public override GameObject OnDrop() {
        UsedByAI = true;
        return null;
    }

    override public void OnPickupEquipped(GameObject gameObject) {
        Weapon weapon = gameObject.GetComponent<Weapon>();
        if(Ammo < Settings.startAmmo) {
            Ammo += weapon.Ammo;
            if (Ammo > Settings.startAmmo) Ammo = Settings.startAmmo;

            Destroy(gameObject);
        }
    }
}
