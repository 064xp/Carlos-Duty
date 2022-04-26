using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WeaponManager : MonoBehaviour
{
    
    public Equipable EquipedItem { get; private set; }
    [SerializeField]
    private int selectedItemIndex = 0;
    [SerializeField]
    private HUDManager hudManager;
    [SerializeField]
    private Camera fpsCamera;
    float initialCamFOV;
    public UnityEvent OnWeaponChange;
    [SerializeField]
    Animator armsAnimator;
    private AudioSource audioSource;

    [Header("Sounds")]
    public SimpleAudioEvent pickupAudio;

    private void Start() {
        initialCamFOV = fpsCamera.fieldOfView;
        audioSource = GetComponent<AudioSource>();
        SelectWeapon();
    }

    public void SelectWeapon() {
        int i = 0;

        if(transform.childCount == 0) {
            hudManager.SetNoWeaponAmmo();
            EquipedItem = null;
            armsAnimator.SetInteger("WeaponID", 0);
            return;
        }

        if (selectedItemIndex >= transform.childCount) selectedItemIndex = transform.childCount - 1;

        // Reset cam FOV when weapon switches
        fpsCamera.fieldOfView = initialCamFOV;

        foreach(Transform item in transform) {
            if (i == selectedItemIndex) {
                item.gameObject.SetActive(true);
                EquipedItem = item.GetComponent<Equipable>();
                EquipedItem.OnEquip();
            } else {
                item.gameObject.SetActive(false);
            }
            i++;
        }

        OnWeaponChange.Invoke();
    }

    public void SwitchWeapon(int weaponIndex) {
        if (weaponIndex > transform.childCount - 1) return;

        selectedItemIndex = weaponIndex;
        SelectWeapon();
    }

    public void SwitchToNextWeapon() {
        if (transform.childCount < 2) 
            return;

        if (selectedItemIndex == transform.childCount - 1)
            selectedItemIndex = 0;
        else
            selectedItemIndex++;

        SelectWeapon();
    }

    public void SwitchToPreviousWeapon() {
        if (transform.childCount < 2) 
            return;

        if (selectedItemIndex <= 0)
            selectedItemIndex = transform.childCount - 1;
        else
            selectedItemIndex--;

        SelectWeapon();
    }

    public void PickupWeapon(GameObject gameObject) {
        Equipable equipable = gameObject.GetComponent<Equipable>();
        Equipable equippedItem = FindEquipped(equipable);

        // If weapon is already equipped
        if (equippedItem != null) {
            equippedItem.OnPickupEquipped(gameObject);
        } else {
            //gameObject.transform.localPosition = Vector3.zero;
            gameObject.transform.localRotation = Quaternion.Euler(Vector3.zero);
            gameObject.transform.rotation = transform.rotation;

            if (transform.childCount > 0)
                gameObject.transform.gameObject.SetActive(false);

            gameObject.transform.SetParent(transform);
            gameObject.transform.localPosition = equipable.playerCustomPosition;

            equipable.OnPickup(this);
        }

        pickupAudio.Play(audioSource);

        SelectWeapon();
    }

    public void OnDrawGizmos() {
        float angle = Vector3.Angle(Vector3.down, transform.forward) - 65f;
        Vector3 rayDir = Quaternion.AngleAxis(angle, transform.right) * transform.forward;
        Vector3 origin = transform.position;
        origin.y += 1;

        Debug.DrawRay(origin, rayDir * 10, Color.red);
    }

    public void DropWeapon() {
        if (transform.childCount == 0) return;

        // If it returns a GameObject, drop that, otherwise,
        // drop the currently selected item
        GameObject gameObject = EquipedItem.OnDrop();
        Transform selectedItem;

        if (gameObject == null)
            selectedItem = transform.GetChild(selectedItemIndex);
        else
            selectedItem = gameObject.transform;

        selectedItem.SetParent(null);
        RaycastHit hit;

        float angle = Vector3.Angle(Vector3.down, transform.forward) - 65f;
        Vector3 rayDir = Quaternion.AngleAxis(angle, transform.right) * transform.forward;
        Vector3 origin = transform.position;
        origin.y += 1;

        if(Physics.Raycast(origin, rayDir, out hit)) {
            BoxCollider itemCollider = selectedItem.gameObject.GetComponent<BoxCollider>();
            Vector3 newPos = hit.point;
            newPos.y = itemCollider.bounds.size.y + 0.4f;

            selectedItem.position = newPos;
            selectedItem.rotation = Quaternion.Euler(transform.forward);
            itemCollider.enabled = true;
            selectedItem.GetComponent<Equipable>().animator.enabled = false;
        }

        SelectWeapon();
    }

    public void DestroyItem(GameObject gameObject) {
        
    }

    Equipable FindEquipped(Equipable equipable) {
        foreach(Transform item in transform) {
            Equipable currentItem = item.gameObject.GetComponent<Equipable>();
            if (currentItem.GetName() == equipable.GetName()) {
                return currentItem;
            }
        }
        return null;
    }

    public int GetWeaponCount() {
        return transform.childCount;
    }
}
