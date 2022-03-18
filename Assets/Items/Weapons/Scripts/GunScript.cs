using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunScript : MonoBehaviour
{
    public Gun gunSettings;

    // Object references
    private Camera fpsCam;
    private delegate bool FireInputMethod(string name) ;
    private FireInputMethod inputMethod;
    private Animator animator;
    private ParticleSystem muzzleFlash;
    private AudioSource audioSource;
    private GameObject crosshair;

    // Internal state
    private float nextTimeToFire = 0f;
    private int ammo;
    private int magazineAmmo;
    private bool isReloading;
    private bool wasADS;
    private float originalCamFOV;

    public Transform muzzleFlashPos;

    // Start is called before the first frame update
    void Start()
    {
        // Get components
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        // Initialize variables
        ammo = gunSettings.startAmmo;
        magazineAmmo = gunSettings.clipSize;
        isReloading = false;

        // Instantiate particles
        muzzleFlash = Instantiate<ParticleSystem>(gunSettings.muzzleFlash, muzzleFlashPos);

        // Initialize animator variables
        animator.SetFloat("ReloadTimeMultiplier", 1 / gunSettings.reloadTime);
        animator.SetFloat("ADSTimeMultiplier", 1 / gunSettings.ADSTime);

        // On pickup
        fpsCam = GameObject.Find("FPSCamera").GetComponent<Camera>();
        crosshair = GameObject.Find("Crosshair");
        originalCamFOV = fpsCam.fieldOfView;

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

        // Check if need to reload or manual reload
        if(magazineAmmo <= 0 || Input.GetKeyDown(KeyCode.R)) {
            StartReload();
            return;
        }

        // Fire 
        if(inputMethod("Fire1") && Time.time >= nextTimeToFire) {
            nextTimeToFire = Time.time + 1 / gunSettings.fireRate;
            Shoot();
        }

        // Check for ADS
        ADS();

    }

    void Shoot() {
        RaycastHit hit;

        animator.CrossFadeInFixedTime("Shooting", 0f, 0);
        muzzleFlash.Play();
        gunSettings.audioEvent.Play(audioSource);

        if(Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, gunSettings.range)) {
            print(hit.transform.name);
        }

        // Impact effect
        GameObject impactObject = Instantiate(gunSettings.impactEffect, hit.point, Quaternion.LookRotation(hit.normal)).gameObject;
        Destroy(impactObject, 2f);

        magazineAmmo--;
    }

    void StartReload() {
        if (ammo == 0 || magazineAmmo == gunSettings.clipSize) return;
        
        isReloading = true;
        animator.SetTrigger("Reload");
        wasADS = animator.GetBool("IsADS");
        animator.SetBool("IsADS", false);
    }

    void EndReload() {
        if (wasADS) animator.SetBool("IsADS", true);

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

    void ADS() {
        if (Input.GetButtonDown("Fire2")){
            bool isADS = animator.GetBool("IsADS");
            float newFov = isADS ? originalCamFOV : gunSettings.ADSFov;

            crosshair.SetActive(isADS);
            StartCoroutine(LerpFOVTo(newFov));
            animator.SetBool("IsADS", !isADS);
        }
    }

    IEnumerator LerpFOVTo(float value) {
        float elapsedTime = 0f;
        float initialFOV = fpsCam.fieldOfView;

        while(elapsedTime <= gunSettings.ADSTime) {
            elapsedTime += Time.deltaTime;
            fpsCam.fieldOfView = Mathf.Lerp(initialFOV, value, elapsedTime / gunSettings.ADSTime);
            yield return null;
        }

    }
}
