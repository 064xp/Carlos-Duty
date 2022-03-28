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
    private HUDManager hud;

    // Internal state
    private float nextTimeToFire = 0f;
    private int ammo;
    private int magazineAmmo;
    private bool isReloading;
    private bool wasADS;
    private float originalCamFOV;

    public Transform muzzleFlashPos;
    public bool usedByAI = false;

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
        hud = GameObject.Find("HUDManager").GetComponent<HUDManager>();
        originalCamFOV = fpsCam.fieldOfView;

        inputMethod = gunSettings.fireMode switch
        {
            Gun.FireModes.Automatic => Input.GetButton,
            Gun.FireModes.SemiAutomatic => Input.GetButtonDown,
            _ => Input.GetButton,
        };

        // On equip
        hud.SetAmmo(magazineAmmo, ammo);
    }

    // Update is called once per frame
    void Update()
    {
        if (isReloading) return;

        // Check if need to reload
        if(magazineAmmo <= 0) {
            StartReload();
            return;
        }

        // Player actions, when weapon is not being used by an AI
        CheckForUserInputs();

    }

    void CheckForUserInputs() {
        if (!usedByAI) {
            // Check for manual reload
            if(Input.GetKeyDown(KeyCode.R)) {
                StartReload();
                return;
            }

            // Fire 
            if(inputMethod("Fire1")) {
                Shoot();
            }

            // Check for ADS
            if (Input.GetButtonDown("Fire2")) {
                ADS();
            }
        }
    }

    public void Shoot() {
        if (Time.time < nextTimeToFire || isReloading) return;
        nextTimeToFire = Time.time + 1 / gunSettings.fireRate;

        RaycastHit hit;

        animator.CrossFadeInFixedTime("Shooting", 0f, 0);
        muzzleFlash.Play();
        gunSettings.shootAudioEvent.Play(audioSource);

        Transform raycastOrigin = usedByAI ?  muzzleFlashPos : fpsCam.transform;

        if(Physics.Raycast(raycastOrigin.position, raycastOrigin.forward, out hit, gunSettings.range)) {
            hit.transform.gameObject.SendMessage("TakeDamage", gunSettings.damage, SendMessageOptions.DontRequireReceiver);
            print(hit.transform.name);
        }

        // Impact effect
        GameObject impactObject = Instantiate(gunSettings.impactEffect, hit.point, Quaternion.LookRotation(hit.normal)).gameObject;
        Destroy(impactObject, 2f);

        magazineAmmo--;

        if(!usedByAI)
            hud.SetAmmo(magazineAmmo, ammo);
    }

    void StartReload() {
        if (ammo == 0 || magazineAmmo == gunSettings.clipSize) return;

        gunSettings.reloadAudioEvent.Play(audioSource);
        isReloading = true;
        animator.SetTrigger("Reload");
        wasADS = animator.GetBool("IsADS");
        animator.SetBool("IsADS", false);
    }

    public void EndReload() {
        if (wasADS) animator.SetBool("IsADS", true);

        print("end reload");
        isReloading = false;
        if(ammo < gunSettings.clipSize) {
            magazineAmmo = ammo;
            ammo = 0;
        } else {
            ammo -= gunSettings.clipSize - magazineAmmo;
            magazineAmmo = gunSettings.clipSize;
        }

        if(!usedByAI)
            hud.SetAmmo(magazineAmmo, ammo);
    }

    void ADS() {
        bool isADS = animator.GetBool("IsADS");
        float newFov = isADS ? originalCamFOV : gunSettings.ADSFov;

        crosshair.SetActive(isADS);
        StartCoroutine(LerpFOVTo(newFov));
        animator.SetBool("IsADS", !isADS);
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
