using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunScript : MonoBehaviour
{
    public Gun gunSettings;

    private Camera fpsCam;
    private delegate bool FireInputMethod(string name) ;
    private FireInputMethod inputMethod;
    private Animator animator;
    private float nextTimeToFire = 0f;

    public int ammo;
    public int magazineAmmo;
    public bool isReloading;

    // Start is called before the first frame update
    void Start()
    {
        // Get components
        animator = GetComponent<Animator>();

        // Initialize variables
        ammo = gunSettings.startAmmo;
        magazineAmmo = gunSettings.clipSize;
        isReloading = false;

        animator.SetFloat("ReloadTimeMultiplier", 1 / gunSettings.reloadTime);
        animator.SetFloat("ShootTimeMultiplier", gunSettings.fireRate);

        // On pickup
        fpsCam = GameObject.Find("FPSCamera").GetComponent<Camera>();

        inputMethod = gunSettings.fireMode switch
        {
            Gun.FireModes.Automatic => Input.GetButton,
            Gun.FireModes.SemiAutomatic => Input.GetButtonDown,
            _ => Input.GetButton,
        };
    }

    // Update is called once per frame
    void Update()
    {
        if (isReloading) return;

        if(magazineAmmo <= 0) {
            StartReload();
            return;
        }

        if(inputMethod("Fire1") && Time.time >= nextTimeToFire) {
            nextTimeToFire = Time.time + 1 / gunSettings.fireRate;
            Shoot();
        }   
    }


    void Shoot() {
        RaycastHit hit;

        animator.CrossFadeInFixedTime("Shooting", 0.1f, 0);

        gunSettings.muzzleFlash.Play();
        if(Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, gunSettings.range)) {
            print(hit.transform.name);
        }

        // Impact effect
        GameObject impactObject = Instantiate(gunSettings.impactEffect, hit.point, Quaternion.LookRotation(hit.normal)).gameObject;
        Destroy(impactObject, 2f);

        magazineAmmo--;
    }

    void StartReload() {
        isReloading = true;
        if (ammo == 0) return;
        animator.SetTrigger("Reload");
    }

    void EndReload() {
        if(ammo < gunSettings.clipSize) {
            magazineAmmo = ammo;
            ammo = 0;
            isReloading = false;
        } else {
            magazineAmmo = gunSettings.clipSize;
            ammo -= gunSettings.clipSize;
            isReloading = false;
        }
    }

}
