using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunScript : Weapon
{

    // Object references
    private Camera fpsCam;
    private delegate bool FireInputMethod(string name) ;
    private FireInputMethod inputMethod;
    [SerializeField]
    private Animator animator;
    private ParticleSystem muzzleFlash;
    [SerializeField]
    private AudioSource audioSource;
    private GameObject crosshair;
    private HUDManager hud;

    // Internal state
    private float nextTimeToFire = 0f;
    private bool isReloading;
    private bool wasADS;
    private float originalCamFOV;
    public Transform muzzleFlashPos;

    private void OnEnable() {
        animator.CrossFadeInFixedTime("Draw", 0f, 0);
        canShoot = false;
    }

    private void OnDisable() {
        animator.SetBool("IsADS", false);
        crosshair.SetActive(true);
    }

    // Start is called before the first frame update
    void Start()
    {
        animator.keepAnimatorControllerStateOnDisable = true;
        // Initialize variables
        Ammo = Settings.startAmmo;
        MagazineAmmo = Settings.clipSize;
        isReloading = false;

        // Instantiate particles
        muzzleFlash = Instantiate<ParticleSystem>(Settings.muzzleFlash, muzzleFlashPos);

        // Initialize animator variables
        animator.SetFloat("ReloadTimeMultiplier", 1 / Settings.reloadTime);
        animator.SetFloat("ADSTimeMultiplier", 1 / Settings.ADSTime);
        animator.SetFloat("DrawTimeMultiplier", 1 / Settings.drawTime);

        // On pickup
        fpsCam = GameObject.Find("FPSCamera").GetComponent<Camera>();
        crosshair = GameObject.Find("Crosshair");
        hud = GameObject.Find("HUDManager").GetComponent<HUDManager>();
        originalCamFOV = fpsCam.fieldOfView;

        inputMethod = Settings.fireMode switch
        {
            WeaponSettings.FireModes.Automatic => Input.GetButton,
            WeaponSettings.FireModes.SemiAutomatic => Input.GetButtonDown,
            _ => Input.GetButton,
        };

        // On equip
        hud.SetAmmo(MagazineAmmo, Ammo);
    }

    // Update is called once per frame
    void Update()
    {
        if (isReloading || !canShoot) return;

        // Check if need to reload
        if(MagazineAmmo <= 0) {
            StartReload();
            return;
        }

        // Player actions, when weapon is not being used by an AI
        CheckForUserInputs();

    }

    void CheckForUserInputs() {
        if (!UsedByAI) {
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
        nextTimeToFire = Time.time + 1 / Settings.fireRate;

        RaycastHit hit;

        animator.CrossFadeInFixedTime("Shooting", 0f, 0);
        muzzleFlash.Play();
        Settings.shootAudioEvent.Play(audioSource);

        Transform raycastOrigin = UsedByAI ?  muzzleFlashPos : fpsCam.transform;

        if(Physics.Raycast(raycastOrigin.position, raycastOrigin.forward, out hit, Settings.range)) {
            if (!UsedByAI && hit.transform.gameObject.CompareTag("Player")) return;

            hit.transform.gameObject.SendMessage("TakeDamage", Settings.damage, SendMessageOptions.DontRequireReceiver);
            print(hit.transform.name);
        }

        // Impact effect
        GameObject impactObject = Instantiate(Settings.impactEffect, hit.point, Quaternion.LookRotation(hit.normal)).gameObject;
        Destroy(impactObject, 2f);

        MagazineAmmo--;

        if(!UsedByAI)
            hud.SetAmmo(MagazineAmmo, Ammo);
    }

    void StartReload() {
        if (Ammo == 0 || MagazineAmmo == Settings.clipSize) return;

        Settings.reloadAudioEvent.Play(audioSource);
        isReloading = true;
        animator.SetTrigger("Reload");
        wasADS = animator.GetBool("IsADS");
        if (wasADS) StartCoroutine(LerpFOVTo(originalCamFOV));
        animator.SetBool("IsADS", false);
    }

    public void EndReload() {
        if (wasADS) {
            animator.SetBool("IsADS", true);
            StartCoroutine(LerpFOVTo(Settings.ADSFov));
        }

        isReloading = false;
        if(Ammo < Settings.clipSize) {
            MagazineAmmo = Ammo;
            Ammo = 0;
        } else {
            Ammo -= Settings.clipSize - MagazineAmmo;
            MagazineAmmo = Settings.clipSize;
        }

        if(!UsedByAI)
            hud.SetAmmo(MagazineAmmo, Ammo);
    }

    void ADS() {
        bool isADS = animator.GetBool("IsADS");
        float newFov = isADS ? originalCamFOV : Settings.ADSFov;

        crosshair.SetActive(isADS);
        StartCoroutine(LerpFOVTo(newFov));
        animator.SetBool("IsADS", !isADS);
    }

    IEnumerator LerpFOVTo(float value) {
        float elapsedTime = 0f;
        float initialFOV = fpsCam.fieldOfView;

        while(elapsedTime <= Settings.ADSTime) {
            elapsedTime += Time.deltaTime;
            fpsCam.fieldOfView = Mathf.Lerp(initialFOV, value, elapsedTime / Settings.ADSTime);
            yield return null;
        }
    }

    override public string GetName(){
        return Settings.weaponName;
    }
}
