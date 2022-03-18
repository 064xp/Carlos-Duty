using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable_Objects/Weapons/Gun")]
public class Gun : ScriptableObject {
    public enum FireModes{
        SemiAutomatic,
        Automatic
    }

    public FireModes fireMode = FireModes.Automatic;
    public float damage;
    public float range;
    public float fireRate;
    public float reloadTime;
    public float ADSTime;
    public float ADSFov = 45f;
    public int startAmmo;
    public int clipSize;
    public ParticleSystem muzzleFlash;
    public ParticleSystem impactEffect;
    public SimpleAudioEvent shootAudioEvent;
    public SimpleAudioEvent reloadAudioEvent;
}
