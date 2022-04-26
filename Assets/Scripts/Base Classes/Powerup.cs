using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : Equipable
{
    protected PlayerManager player;
    [SerializeField]
    protected int amount = 1;
    [SerializeField]
    protected bool canUse = false;
    protected HUDManager hud;
    protected WeaponManager weaponManager;
    public int weaponID = 0;

    public Powerup() {
        type = Types.Powerup;
    }

    public void SetCanUseTrue() {
        canUse = true;
    }

    override public void OnPickup(WeaponManager weaponManager) {
        base.OnPickup();
        animator.enabled = true;
        GetComponent<BoxCollider>().enabled = false;
        hud = GameObject.Find("HUDManager").GetComponent<HUDManager>();
        player = GameObject.Find("Player").GetComponent<PlayerManager>();
        this.weaponManager = weaponManager;
        canUse = true;
    }

    override public GameObject OnDrop() {
        amount--;

        // If dropping last item
        if (amount == 0) {
            amount = 1;
            canUse = false;
            UsedByAI = true;
            return this.gameObject;
        }

        hud.SetEquipableAmount(amount);

        GameObject clone = Instantiate(this.gameObject);
        Powerup powerup = clone.GetComponent<Powerup>();
        powerup.canUse = false;
        powerup.amount = 1;
        powerup.animator.enabled = false;
        powerup.UsedByAI = true;

        return clone;
    }


    public override void OnPickupEquipped(GameObject gameObject) {
        Destroy(gameObject);
        amount++;
    }

    public override void OnEquip() {
        armsAnimator.SetInteger("WeaponID", weaponID);
        hud.SetEquipableAmount(amount);
    }
}
