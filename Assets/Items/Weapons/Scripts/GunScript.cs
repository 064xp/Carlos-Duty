using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunScript : MonoBehaviour
{
    public Gun gunSettings;

    private Camera fpsCam;
    private float nextTimeToFire = 0f;
    private delegate bool FireInputMethod(string name) ;
    private FireInputMethod inputMethod;

    private int ammo;

    // Start is called before the first frame update
    void Start()
    {

        ammo = gunSettings.maxAmmo;

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
        if(inputMethod("Fire1") && Time.time >= nextTimeToFire) {
            nextTimeToFire = Time.time + 1 / gunSettings.fireRate;
            Shoot();
        }
    }

    void Shoot() {
        RaycastHit hit;
        gunSettings.muzzleFlash.Play();
        if(Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, gunSettings.range)) {
            print(hit.transform.name);
        }

        // Impact effect
        GameObject impactObject = Instantiate(gunSettings.impactEffect, hit.point, Quaternion.LookRotation(hit.normal)).gameObject;
        Destroy(impactObject, 2f);
    }

}
