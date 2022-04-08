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
    [SerializeField]
    private float heat = 0.0f;
    private float startCooldownAfter = 0f;

    private void OnEnable() {
        animator.CrossFadeInFixedTime("Draw", 0f, 0);
        canShoot = false;
    }

    private void OnDisable() {
        animator.SetBool("IsADS", false);
        if(crosshair != null)
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

        inputMethod = Settings.fireMode switch
        {
            WeaponSettings.FireModes.Automatic => Input.GetButton,
            WeaponSettings.FireModes.SemiAutomatic => Input.GetButtonDown,
            _ => Input.GetButton,
        };
    }

    override public void OnPickup() {
        // On pickup
        fpsCam = GameObject.Find("FPSCamera").GetComponent<Camera>();
        crosshair = GameObject.Find("Crosshair");
        hud = GameObject.Find("HUDManager").GetComponent<HUDManager>();
        originalCamFOV = fpsCam.fieldOfView;
    }

    // Update is called once per frame
    void Update()
    {
        CoolDown();
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


    void CoolDown() {
        if (Time.time < startCooldownAfter) return;
        if(heat > 0.0f) {
            heat -= Settings.cooldownRate;
            if (heat < 0f) heat = 0f;
        }
    }

    public void Shoot() {
        Transform raycastOrigin = UsedByAI ?  muzzleFlashPos : fpsCam.transform;
        Shoot(raycastOrigin);
    }

    public void Shoot(Transform raycastOrigin) {
        if (Time.time < nextTimeToFire || isReloading) return;
        nextTimeToFire = Time.time + 1 / Settings.fireRate;

        animator.SetBool("IsRunning", false);

        RaycastHit hit;

        animator.CrossFadeInFixedTime("Shooting", 0f, 0);
        muzzleFlash.Play();
        Settings.shootAudioEvent.Play(audioSource);

        Vector3 raycastDirection = ApplyInaccuracy(raycastOrigin.forward, Settings.pitchRange, Settings.yawRange, true);
        if(Physics.Raycast(raycastOrigin.position, raycastDirection, out hit, Settings.range)) {
            if (!UsedByAI && hit.transform.gameObject.CompareTag("Player")) return;

            hit.transform.gameObject.SendMessage("TakeDamage", Settings.damage, SendMessageOptions.DontRequireReceiver);
            print(hit.transform.name);

            // Impact effect
            GameObject impactObject = Instantiate(Settings.impactEffect, hit.point, Quaternion.LookRotation(hit.normal)).gameObject;
            Destroy(impactObject, 2f);
        }


        MagazineAmmo--;
        heat = Mathf.Clamp(heat + 1, 0, Settings.maxHeat);
        startCooldownAfter = Time.time + Settings.cooldownAfterShooting;
        

        if(!UsedByAI)
            hud.SetAmmo(MagazineAmmo, Ammo);
    }

    Vector3 ApplyInaccuracy(Vector3 direction, RangedFloat pitchRange, RangedFloat yawRange, bool useHeat = false) {
        float pitch = Random.Range(pitchRange.minValue, pitchRange.maxValue);
        float yaw = Random.Range(yawRange.minValue, yawRange.maxValue);
        Vector3 modVector = new Vector3(yaw, pitch, 0f);
        if (useHeat) {
            modVector *= heat / Settings.maxHeat;
        }

        return (direction + modVector).normalized;
    }

    void StartReload() {
        bool isRunning = animator.GetBool("IsRunning");
        if (Ammo == 0 || MagazineAmmo == Settings.clipSize || isRunning) return;

        Settings.reloadAudioEvent.Play(audioSource);
        isReloading = true;
        animator.SetLayerWeight(1, 0);
        animator.SetTrigger("Reload");
        wasADS = animator.GetBool("IsADS");
        if (wasADS) StartCoroutine(LerpFOVTo(originalCamFOV));
        animator.SetBool("IsADS", false);
    }

    public void EndReload() {
        if (wasADS) {
            animator.SetBool("IsADS", true);
            animator.SetLayerWeight(1, 1);
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
        bool isRunning = animator.GetBool("IsRunning");
        if (isRunning) return;

        float newFov = isADS ? originalCamFOV : Settings.ADSFov;

        if (isADS) animator.SetLayerWeight(1, 0);
        else animator.SetLayerWeight(1, 1);

        crosshair.SetActive(isADS);
        StartCoroutine(LerpFOVTo(newFov));
        animator.SetBool("IsADS", !isADS);
        animator.SetBool("IsRunning", false);
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

    override public bool CanRun() {
        bool isADS = animator.GetBool("IsADS");
        return !isADS && !isReloading;
    }
}
