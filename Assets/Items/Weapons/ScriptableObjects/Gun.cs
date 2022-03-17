using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable_Objects/Weapons/Gun")]
public class Gun : ScriptableObject {
    public enum FireModes{
        SemiAutomatic,
        Automatic
    }
    //public FireModes fireMode { get { return _speed; } private set { _speed = value; } }
    //[SerializeField] private float _speed = 5.0f;
    public FireModes fireMode = FireModes.Automatic;
    public float damage;
    public float range;
    public float fireRate;
    public float reloadTime;
    public float ADSTime;
    public int startAmmo;
    public int clipSize;
    public ParticleSystem muzzleFlash;
    public ParticleSystem impactEffect;
}
