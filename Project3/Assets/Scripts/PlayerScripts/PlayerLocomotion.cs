using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLocomotion : MonoBehaviour
{

    
    //public PlayerControls controls;
    public float speed = 0;
    public float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;

    private Rigidbody rb;

    [SerializeField] Transform cam;

    private float movementX;
    private float movementY;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnMove(InputValue movementValue)
    {
        //print("You moved something");
        Vector2 movementVector = movementValue.Get<Vector2>();

        movementX = movementVector.x;
        movementY = movementVector.y;


        
    }

    private void FixedUpdate()
    {
        Vector3 movement = new Vector3(movementX, 0.0f, movementY).normalized;

        if (movement.magnitude >= 0.1f)
        {

            //print(movement);

            
            float targetAngle = Mathf.Atan2(movementX, movementY) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDirection = Quaternion.Euler(0f, angle, 0f) * Vector3.forward;
            

            //rb.AddForce(movement.normalized * speed );
            rb.MovePosition(rb.position + moveDirection.normalized * speed * Time.deltaTime);
        }
    }

}
