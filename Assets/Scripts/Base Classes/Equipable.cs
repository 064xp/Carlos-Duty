using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipable : MonoBehaviour
{
    public enum Types {
        Weapon,
        Powerup
    };

    public Animator animator;
    public Types type;

    virtual public string GetName() {
        return "NO_NAME_SET";
    }

    virtual public void OnPickup() { }

    virtual public void OnEquip() {}

    virtual public bool CanRun() { return true;  }
}
