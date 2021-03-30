using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //Sound Code
    AudioSource audioSource;
   public AudioClip throwSound;
    public AudioClip hitSound;
    public AudioClip dashSound;
    public AudioClip jumpSound;

    public static PlayerController instance;

    public enum State
    {
        WALKING,
        JUMPING,
        CLIMBING
    }

    // locomotion bahaviors
    private ClimbingBehavior climbingBehavior;
    private WalkingBehavior walkingBehavior;

    private State currentState;
    public static State CurrentState
    {
        get
        {
            return instance.currentState;
        }
        set
        {
            switch (value)
            {
                case State.CLIMBING:
                    instance.anim.SetBool("isClimbing", true);
                    break;
                case State.JUMPING:
                    instance.anim.SetBool("isJumping", true);
                    break;
                case State.WALKING:
                    if (instance.anim.GetBool("isClimbing") || instance.anim.GetBool("isJumping"))
                    {
                        instance.anim.SetBool("isJumping", false);
                        instance.anim.SetBool("isClimbing", false);
                    }
                    instance.walkingBehavior.checkIsGrounded();
                    break;
            }

            print("you changed your state to " + value);
            instance.currentState = value;
        }
    }

    private Vector2 movementVec;

    //Variables for projectile
    [Header("Projectile attributes")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform projectileSpawnObject;
    [SerializeField] [Range (0, 600)] private float throwForce = 600.0f;
    [SerializeField] private float fireRatePerSecond;
    private GameObject projectileInstance;

    [SerializeField] private Animator anim;

    private void Awake()
    {
         audioSource= GetComponent<AudioSource>();

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            throw new System.Exception("This is a singleton, having multiple instances is not allowed");
        }

        climbingBehavior = GetComponent<ClimbingBehavior>();
        walkingBehavior = GetComponent<WalkingBehavior>();
    }

    private void Update()
    {
        if (CurrentState != State.CLIMBING)
        {
            if (climbingBehavior.ShouldStartWallClimb())
            {
                CurrentState = State.CLIMBING;
            }
        }
        else
        {
            climbingBehavior.StateUpdate(movementVec, Time.deltaTime);
        }
    }

    private void FixedUpdate()
    {
        switch (CurrentState)
        {
            case State.CLIMBING:
                climbingBehavior.PerformMovement(Time.deltaTime);
                break;
            case State.WALKING:
                walkingBehavior.PerformMovement(movementVec, Time.deltaTime);
                break;
            case State.JUMPING:
                walkingBehavior.PerformMovement(movementVec, Time.deltaTime);
                break;
        }
    }

    private void OnMove(InputValue value)
    {
        movementVec = value.Get<Vector2>();
    }

    private void OnFire()
    {
        print("Fired the shot");
        projectileInstance = Instantiate(projectilePrefab, projectileSpawnObject.position, projectileSpawnObject.rotation);
        projectileInstance.GetComponent<ProjectileController>().Launch(throwForce);
        PlaySound(throwSound);
    }

    private void OnDash()
    {
        if (currentState == State.JUMPING) 
        {
            walkingBehavior.PerformDash();  // method here only allows dash if you are jumping
            PlaySound(dashSound);
        }
    }

    private void OnJump()
    {
        if (currentState == State.WALKING || currentState == State.JUMPING)
        {
            walkingBehavior.PerformJump();
        }
        else if (currentState == State.CLIMBING)
        {
            climbingBehavior.performJump(movementVec);
            CurrentState = State.JUMPING;
             PlaySound(jumpSound);
        }
    }

    private void OnTeleport()
    {
        if (projectileInstance != null)
        {
            transform.position = projectileInstance.transform.position;
            Destroy(projectileInstance);
            PlaySound(hitSound);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("McGuffin"))
        {
            GameManager.WinGame();
        }

        if (collision.gameObject.CompareTag("Ground") && CurrentState == State.JUMPING)
        {
            CurrentState = State.WALKING;
        }
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    
}
