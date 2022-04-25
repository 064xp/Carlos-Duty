using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private MoveSettings settings;
    private Vector3 moveDirection;
    private CharacterController controller;
    [SerializeField]
    private WeaponManager weaponManager;
    [SerializeField]
    private float stamina;
    private float replenishStaminaAfter = 0;
    [SerializeField]
    private Animator armsAnimator;

    private void Awake() {
        controller = GetComponent<CharacterController>();
    }

    private void Start() {
        stamina = settings.stamina;
    }

    // Update is called once per frame
    void Update()
    {
        Movement();    
        ManageInputs();
        ReplenishStamina();
    }

    private void FixedUpdate() {
        controller.Move(moveDirection * Time.deltaTime);
    }

    private void ManageInputs() {
        // Scroll wheel up
        if(Input.GetAxis("Mouse ScrollWheel") > 0f) {
            weaponManager.SwitchToNextWeapon();
        }

        // Scroll wheel down
        if(Input.GetAxis("Mouse ScrollWheel") < 0f) {
            weaponManager.SwitchToPreviousWeapon();
        }

        if (Input.GetKeyDown(KeyCode.Q)) {
            weaponManager.DropWeapon();
        }
    }

    private void Movement() {
        Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        bool isRunning = Input.GetButton("Run");
        float speed = settings.speed;
        bool canRun = true;

        if (stamina <= 0f) canRun = false;

        // Running animation
        if(weaponManager.GetWeaponCount() > 0) {
            canRun = canRun && weaponManager.EquipedItem.CanRun();
            if(canRun && isRunning)
                weaponManager.EquipedItem.SetAnimatorParam("IsRunning", true);
            else 
                weaponManager.EquipedItem.SetAnimatorParam("IsRunning", false);
        }

        // Run speed
        if (isRunning && canRun) {
            speed = settings.runSpeed;
            stamina -= Time.deltaTime;
            if (stamina < 0f) stamina = 0f;
            replenishStaminaAfter = Time.time + settings.replenishStaminaTimeout;
        }

        if(controller.isGrounded && moveDirection.y < 0) {
            moveDirection.y = -2f;
        }

        if(input.magnitude > 0) 
            weaponManager?.EquipedItem?.SetAnimatorParam("IsWalking", true);
        else
            weaponManager?.EquipedItem?.SetAnimatorParam("IsWalking", false);
        
        if(input.x != 0 && input.y != 0) {
            // Normalize 2D vector 
            input *= 0.777f;
        }

        moveDirection.x = input.x * speed;
        moveDirection.z = input.y * speed;

        //moveDirection.y = -settings.antiBump - settings.gravity * Time.deltaTime;
        moveDirection.y += settings.gravity * Time.deltaTime;
        //moveDirection.y -= settings.gravity * Time.deltaTime;

        moveDirection = transform.TransformDirection(moveDirection);

        if (Input.GetKey(KeyCode.Space) && controller.isGrounded) {
            Jump();
        } 
            
    }

    private void Jump() {
        moveDirection.y =  Mathf.Sqrt(settings.jumpForce * -2f * settings.gravity);
    }

    private void ReplenishStamina() {
        if(Time.time >= replenishStaminaAfter && stamina < settings.stamina) {
            stamina += settings.staminaReplenishRate;
            if (stamina > settings.stamina) stamina = settings.stamina;
        }
    }
}
