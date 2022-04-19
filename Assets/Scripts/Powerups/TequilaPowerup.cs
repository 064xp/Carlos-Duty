using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TequilaPowerup : Powerup
{
    public int healthRegen = 50;
    bool isDrinking = false;
    private void OnEnable() {
        animator.CrossFadeInFixedTime("Draw", 0f, 0);
        canUse = false;
        isDrinking = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        //animator.keepAnimatorControllerStateOnDisable = true;
    }

    // Update is called once per frame
    void Update()
    {
       if (!canUse || isDrinking) return;

       if(Input.GetButtonDown("Fire1") 
            //&& player.health < player.maxHealth
            ) {
            StartDrink();
       }
    }

    private void StartDrink() {
        animator.SetTrigger("Drink");
        isDrinking = true;
    }

    private void EndDrink() {
        player.SetHealth(player.health + healthRegen);
        isDrinking = false;

        amount--;
        if(amount <= 0) {
            //weaponManager.DropWeapon();
            transform.SetParent(null);
            Destroy(this.gameObject);
            weaponManager.SelectWeapon();
            return;
        }

        hud.SetEquipableAmount(amount);
    }

    override public string GetName() {
        return "Tequila";
    }

    public override bool CanRun() {
        return !isDrinking;
    }
}
