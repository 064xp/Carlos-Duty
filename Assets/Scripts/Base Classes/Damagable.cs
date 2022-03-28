using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damagable : MonoBehaviour {
    [SerializeField]
    private int health;
    public bool alive { get; private set; } = true;

    void TakeDamage(int damage) {
        health -= damage;
        if (health < 0) health = 0;


        if(health == 0) {
            alive = false;
            Die();
        }
    }

    public virtual void Die() {
        print($"Die {this.gameObject.name}");
    }
}
