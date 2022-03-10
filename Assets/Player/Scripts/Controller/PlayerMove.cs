using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField]
    private MoveSettings settings;
    private Vector3 moveDirection;
    private CharacterController controller;

    private void Awake() {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        Movement();    
    }

    private void FixedUpdate() {
        controller.Move(moveDirection * Time.deltaTime);
    }

    private void Movement() {
        Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        if(controller.isGrounded && moveDirection.y < 0) {
            moveDirection.y = -2f;
        }
        
        // Normalize 2D vector 
        if(input.x != 0 && input.y != 0) {
            input *= 0.777f;
        }

        moveDirection.x = input.x * settings.speed;
        moveDirection.z = input.y * settings.speed;

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