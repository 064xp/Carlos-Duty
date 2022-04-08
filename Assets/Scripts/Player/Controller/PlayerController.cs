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

    private void Awake() {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        Movement();    
        ManageInputs();
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
            weaponManager.SwitchToNextWeapon();
        }
    }

    private void Movement() {
        Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        bool isRunning = Input.GetButton("Run");
        float speed = settings.speed;
        bool canRun = true;

        // Running animation
        if(weaponManager.GetWeaponCount() > 0) {
            canRun = weaponManager.selectedWeapon.CanRun();
            if(canRun && isRunning)
                weaponManager.selectedWeapon.animator.SetBool("IsRunning", true);
            else 
                weaponManager.selectedWeapon.animator.SetBool("IsRunning", false);
        }

        // Run speed
        if (isRunning && canRun) {
            speed = settings.runSpeed; 
        }

        if(controller.isGrounded && moveDirection.y < 0) {
            moveDirection.y = -2f;
        }
        
        // Normalize 2D vector 
        if(input.x != 0 && input.y != 0) {
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
}
