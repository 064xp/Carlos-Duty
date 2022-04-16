using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable_Objects/Movement/Settings")]
public class MoveSettings : ScriptableObject {
    public float speed = 5.0f;
    public float runSpeed = 18.0f;
    public float jumpForce = 13.0f;
    public float antiBump = 4.5f;
    public float gravity = -30.0f;
    public float stamina = 10f;
    public float replenishStaminaTimeout = 1.5f;
    public float staminaReplenishRate = 0.3f;
}