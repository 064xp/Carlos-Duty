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
    public Animator armsAnimator;
    public bool UsedByAI = true;
    public Types type;
    public Vector3 playerCustomPosition = Vector3.zero;
    public Vector3 armsCustomPosition = Vector3.zero;

    virtual public string GetName() {
        Debug.LogWarning($"You need to override the GetName method for item {transform.name}");
        return "NO_NAME_SET";
    }

    virtual public void OnPickup() {
        armsAnimator = GameObject.Find("/Player/FPSCamera/FPSArmsContainer/FPSArms").GetComponent<Animator>();
        UsedByAI = false;
    }
    virtual public void OnPickup(WeaponManager weaponManager) { OnPickup(); }
    // Return the gameObject to drop. If null, will drop the one that is equipped.
    virtual public GameObject OnDrop() {
        UsedByAI = true;
        return null; 
    }
    virtual public void OnPickupEquipped(GameObject gameObject) { }
    virtual public void OnEquip() { }
    virtual public bool CanRun() { return true;  }

    public void SetAnimatorParam(string name, bool value) {
        if(armsAnimator != null)
            armsAnimator.SetBool(name, value);
        animator.SetBool(name, value);
    }
    public void SetAnimatorParam(string name, float value) {
        if(armsAnimator != null)
            armsAnimator.SetFloat(name, value);
        animator.SetFloat(name, value);
    }

    public void SetAnimatorTrigger(string name) {
        if(armsAnimator != null)
            armsAnimator.SetTrigger(name);
        animator.SetTrigger(name);
    }
    
    public void ToAnimatorState(string state) {
        animator.CrossFadeInFixedTime(state, 0f, 0);
        if(armsAnimator != null) 
            armsAnimator.CrossFadeInFixedTime(state, 0f, 0);
    }
    public void ToAnimatorState(string weaponState, string armsState) {
        animator.CrossFadeInFixedTime(weaponState, 0f, 0);
        if(armsAnimator != null) {
            armsAnimator.CrossFadeInFixedTime(armsState, 0f, 0);
        }
    }

}
