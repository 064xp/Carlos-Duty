using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : Equipable {
    public WeaponSettings Settings;
    public bool canShoot = false;
    public int Ammo;
    public int MagazineAmmo;
    [SerializeField]
    protected HUDManager hud;

    public Weapon() {
        type = Types.Weapon;
    }

    override public string GetName() {
        return Settings.name;
    }

    override public void OnPickup() {
        base.OnPickup();
        animator.enabled = true;
        GetComponent<BoxCollider>().enabled = false;
        hud = GameObject.Find("HUDManager").GetComponent<HUDManager>();
        UsedByAI = false;
    }

    override public void OnEquip() {
        hud.SetAmmo(MagazineAmmo, Ammo);
        armsAnimator.SetInteger("WeaponID", Settings.weaponID);
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
