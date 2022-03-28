using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Damagable : MonoBehaviour {
    [SerializeField]
    protected int health = 100;
    public bool alive { get; private set; } = true;

    public virtual void TakeDamage(int damage) {
        health -= damage;
        if (health < 0) health = 0;


        if(health == 0) {
            alive = false;
            Die();
        }
    }

    public abstract void Die();
}
