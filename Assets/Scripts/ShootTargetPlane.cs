using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ShootTargetPlane : Damagable
{
    public UnityEvent churchDoorDestroyed;
    public HUDManager hud;

    private void Start() {
        health = 1000;
        hud.SetChurchMaxHealth(health);
        hud.SetChurchHealth(health);
    }

    public override void TakeDamage(int damage) {
        base.TakeDamage(damage);
        hud.SetChurchHealth(health);
    }

    public override void Die() {
        churchDoorDestroyed.Invoke();
    }
}
