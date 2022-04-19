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
        Debug.LogWarning($"You need to override the GetName method for item {transform.name}");
        return "NO_NAME_SET";
    }


    virtual public void OnPickup() { }
    virtual public void OnPickup(WeaponManager weaponManager) { OnPickup(); }
    // Return the gameObject to drop. If null, will drop the one that is equipped.
    virtual public GameObject OnDrop() { return null; }
    virtual public void OnPickupEquipped(GameObject gameObject) { }
    virtual public void OnEquip() { }
    virtual public bool CanRun() { return true;  }
}
