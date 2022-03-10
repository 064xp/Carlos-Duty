using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunScript : MonoBehaviour
{
    public float damage = 10f;
    public float range = 100f;
    public ParticleSystem muzzleFlash;
    public ParticleSystem impactEffect;
    public float fireRate = 10f;
    public FireModes fireMode;

    private Camera fpsCam;
    private float nextTimeToFire = 0f;
    private delegate bool FireInputMethod(string name) ;
    FireInputMethod inputMethod;

    // Start is called before the first frame update
    void Start()
    {
        fpsCam = GameObject.Find("FPSCamera").GetComponent<Camera>();

        inputMethod = fireMode switch
        {
            FireModes.Automatic => Input.GetButton,
            FireModes.SemiAutomatic => Input.GetButtonDown,
            _ => Input.GetButton,
        };
    }

    // Update is called once per frame
    void Update()
    {
        if(inputMethod("Fire1") && Time.time >= nextTimeToFire) {
            nextTimeToFire = Time.time + 1 / fireRate;
            Shoot();
        }
    }

    void Shoot() {
        RaycastHit hit;
        muzzleFlash.Play();
        if(Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range)) {
            print(hit.transform.name);
        }

        // Impact effect
        GameObject impactObject = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal)).gameObject;
        Destroy(impactObject, 2f);
    }

    public enum FireModes{
        SemiAutomatic,
        Automatic
    }
}
