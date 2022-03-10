using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunScript : MonoBehaviour
{
    public float damage = 10f;
    public float range = 100f;
    public ParticleSystem muzzleFlash;
    public ParticleSystem impactEffect;

    private Camera fpsCam;
    // Start is called before the first frame update
    void Start()
    {
        fpsCam = GameObject.Find("FPSCamera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Fire1")) {
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
        Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
    }
}
