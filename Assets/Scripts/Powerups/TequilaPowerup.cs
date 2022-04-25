using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TequilaPowerup : Powerup
{
    public int healthRegen = 50;
    public float drunkEffectAmount = 0.5f;
    private bool isDrinking = false;
    [SerializeField]
    private DrunkEffect drunkEffect;

    private void OnEnable() {
        animator.CrossFadeInFixedTime("Draw", 0f, 0);
        canUse = false;
        isDrinking = false;
    }

    public override void OnPickup() {
        base.OnPickup();
        //drunkEffect = GameObject.Find("DrunkEffect").GetComponent<DrunkEffect>();
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
            && player.health < player.maxHealth
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
        drunkEffect.ApplyEffect(drunkEffectAmount);
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
