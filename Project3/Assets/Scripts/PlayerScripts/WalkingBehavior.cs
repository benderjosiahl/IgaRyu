using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingBehavior: MonoBehaviour
{

    public AudioClip collectedClip;

    private Rigidbody rb;
    private Collider collider;
    private Animator anim;
    [SerializeField] private Transform cam;

    
    [SerializeField] [Range(0, 30)] private float walkingSpeed = 6;
    [SerializeField] [Range(0, 30)] private float sneakingSpeed = 3;
    private float currentSpeed;
    private float turnSmoothVelocity;

    // Variables for player jumping
    [SerializeField] [Range(0, 300)] private float jumpForce = 200f;
    [SerializeField] [Range(0, 300)] private float dashForce = 25f;

    private bool isJumping;
    private bool canJump;
    private bool canDash;
    private bool isSneaking;

    [SerializeField] [Range(0, 1)] private float turnSmoothTime = 0.1f;

    public void Awake()
    {
        collider = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        //cam = Camera.main.transform;
        Cursor.visible = false;
        currentSpeed = walkingSpeed;
    }

    public void PerformMovement(Vector2 movementVec, float delta)
    {
        Vector3 movement = new Vector3(movementVec.x, 0.0f, movementVec.y).normalized;
        if (movement.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(movementVec.x, movementVec.y) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            Vector3 moveDirection = Quaternion.Euler(0f, angle, 0f) * Vector3.forward;

            //rb.AddForce(movement.normalized * speed );
            rb.MovePosition(rb.position + moveDirection.normalized * currentSpeed * delta);
            //Play Sound With the Movement
            PlayerController.PlaySound(collectedClip);

            
        }
        // if the player is grounded, play walk animation
        if (!isJumping)
        {
            anim.SetFloat("Speed", movement.magnitude, 0.1f, Time.deltaTime);
        } else
        {
            anim.SetFloat("VertVelocity", 1, 0.3f, Time.deltaTime);
        }

    }

    private void OnCollisionEnter (Collision other)
    {
        if (other.gameObject.CompareTag ("Ground"))
        {
            // make a noise
            // SoundDetectionBehavior.AlertAgentsToSound(other.transform.position, 1);
            canJump = true;
            canDash = true;
            isJumping = false;
            return;
        }

        RaycastHit hit;
        Debug.DrawRay(collider.bounds.center, Vector3.down, Color.red, 1f);
        if(Physics.Raycast(collider.bounds.center, Vector3.down, out hit, 1f))
        {
            canJump = true;
        }
    }

    public void PerformDash()
    {
        if (canJump == false && canDash == true)
        {
            rb.AddForce(transform.forward * dashForce, ForceMode.Impulse);// Dashing ability for the player
            canDash = false;
        }
    }

    public void PerformJump()
    {
        if (canJump)
        {
            rb.AddForce(transform.up * jumpForce);      // Used to make the player jump, requires direction and measure of force that will be applied
            canJump = false;                            // Sets the isGrounded variable back to false, preventing the player from jumping endlessly
            isJumping = true;
        }
    }

    public void CheckIsGrounded()
    {
        Debug.DrawRay(collider.bounds.center, Vector3.down, Color.red, 10f);
        if (Physics.Raycast(collider.bounds.center, Vector3.down, 1f))
        {
            canJump = true;
            isJumping = false;
        }
    }

    public void ActivateSneaking()
    {
        if (!isSneaking)
        {
            currentSpeed = sneakingSpeed;
            // trigger sneaking animation
        } else
        {
            currentSpeed = walkingSpeed;
            // trigger walking animation
        }

        isSneaking = !isSneaking;
    }
}
