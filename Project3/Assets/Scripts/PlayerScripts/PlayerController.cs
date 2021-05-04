using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    private LayerMask projectileLayermask;

    //Compass Variables
    private int compassDuration = 8;

    public enum LocomotionState
    {
        WALKING,
        JUMPING,
        CLIMBING
    }

    // locomotion bahaviors
    private ClimbingBehavior climbingBehavior;
    private WalkingBehavior walkingBehavior;

    private LocomotionState currentState;
    public static LocomotionState CurrentState
    {
        get
        {
            return instance.currentState;
        }
        set
        {
            switch (value)
            {
                case LocomotionState.CLIMBING:
                    instance.anim.SetBool("isClimbing", true);
                    instance.anim.SetBool("isJumping", false);
                    break;
                case LocomotionState.JUMPING:
                    print("YOU ARE JUMPING");
                    instance.anim.SetBool("isJumping", true);
                    break;
                case LocomotionState.WALKING:
                    instance.anim.SetBool("isJumping", false);
                    instance.anim.SetBool("isClimbing", false);
                    instance.walkingBehavior.CheckIsGrounded();
                    break;
            }

            print("you changed your state to " + value);
            instance.currentState = value;
        }
    }

    // state bools
    private bool isInvincible = false;


    private Vector2 movementVec;

    [Header("Health variables")]
    private float currentHealth;
    public float Health { get { return currentHealth; } }
    [SerializeField] float maxHealth = 10;
    [SerializeField] float initHealth = 5;
    [SerializeField] float timeInvincible;
    private float invincibleTimer;


    //Variables for projectile
    [Header("Projectile attributes")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform projectileSpawnObject;
    [SerializeField] [Range (0, 600)] private float throwForce = 20.0f;
    [SerializeField] [Range (0, 600)] private float maxThrowDis = 20.0f;
    [SerializeField] [Range (0, 100)] private float teleportationDistance = 10.0f;
    [SerializeField] private float fireRatePerSecond;
    private GameObject projectileInstance;

    //Teleportation Particle Variable
    public GameObject teleportationParticle;

    
    //Sound Code
    private AudioSource audioSource;
    [Header("Sounds")]
    [SerializeField] private AudioClip throwSound;
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private AudioClip dashSound;
    [SerializeField] private AudioClip jumpSound;

    [SerializeField] private GameObject compassPrefab;

    private Animator anim;
    private void Awake()
    {
        currentHealth = initHealth;

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
        anim = GetComponentInChildren<Animator>();

        projectileLayermask =~ LayerMask.GetMask("Player"); //<---------- this specifies all layers EXCEPT the one that comes back
    }

    private void Update()
    {
        if (CurrentState != LocomotionState.CLIMBING)
        {
            if (climbingBehavior.ShouldStartWallClimb())
            {
                CurrentState = LocomotionState.CLIMBING;
            }
        }
        else
        {
            climbingBehavior.StateUpdate(movementVec, Time.deltaTime);
        }

        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (timeInvincible <= 0)
            {
                isInvincible = false;
            }
        }
    }

    private void FixedUpdate()
    {
        switch (CurrentState)
        {
            case LocomotionState.CLIMBING:
                climbingBehavior.PerformMovement(Time.deltaTime);
                break;
            case LocomotionState.WALKING:
                walkingBehavior.PerformMovement(movementVec, Time.deltaTime);
                break;
            case LocomotionState.JUMPING:
                walkingBehavior.PerformMovement(movementVec, Time.deltaTime);
                break;
        }
    }

    public void ChangeHealth(float amount)
    {

        GameManager.LoseGame();

        if (amount < 0)
        {
            if (isInvincible)
            {
                return;
            }

            isInvincible = true;
            invincibleTimer = timeInvincible;
        }

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        print(currentHealth + "/" + maxHealth);
    }

    private void OnMove(InputValue value)
    {
        movementVec = value.Get<Vector2>();
    }

    private void OnFire()
    {
        print("threw a kunai");
        // raycast from the camera, what we hit is our target
        //      if we don't hit anything, make the target XX units out
        Transform originTransform = Camera.main.transform;
        RaycastHit hit;
        //Debug.DrawRay(originTransform.position, originTransform.forward, Color.red, 10f) ;
        if (Physics.Raycast(originTransform.position, originTransform.forward, out hit, maxThrowDis, projectileLayermask)){
            Vector3 relativeTargetPos = hit.point - projectileSpawnObject.position;
            // spawn the prefab, then let the prefab take over?
            projectileInstance = Instantiate(projectilePrefab, projectileSpawnObject.position, Quaternion.LookRotation(relativeTargetPos));
        } else
        {
            projectileInstance = Instantiate(projectilePrefab, projectileSpawnObject.position, Quaternion.LookRotation(originTransform.forward * maxThrowDis));
        }
        instance.anim.SetBool("isThrowing", true);

        instance.anim.SetTrigger("Throw");/*
        instance.anim.ResetTrigger("Throw");*/
        PlaySound(throwSound);
    }

    private void OnDash()
    {
        if (CurrentState == LocomotionState.JUMPING) 
        {
            walkingBehavior.PerformDash();  // method here only allows dash if you are jumping
            PlaySound(dashSound);
        }
    }

    private void OnJump()
    {
        if (CurrentState == LocomotionState.WALKING || CurrentState == LocomotionState.JUMPING)
        {
            walkingBehavior.PerformJump();
        }
        else if (CurrentState == LocomotionState.CLIMBING)
        {
            climbingBehavior.performJump(movementVec);
            
        }
        CurrentState = LocomotionState.JUMPING;
        PlaySound(jumpSound);
    }

    private void OnSneak()
    {
        if (CurrentState == LocomotionState.WALKING)
        {
            walkingBehavior.ActivateSneaking();
        }
    }

    private void OnTeleport()
    {
        if (projectileInstance != null)
        {
            if (Vector3.Distance(projectileInstance.transform.position, transform.position) > teleportationDistance)
            {
                return;
            }
            transform.position = projectileInstance.transform.position;
            teleportVfx();
            Destroy(projectileInstance);
            PlaySound(hitSound);
        }
    }


    private void OnCompass()
    {
        StartCoroutine(CompassCountdown());
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("McGuffin"))
        {
            GameManager.WinGame();
        }

        if (collision.gameObject.CompareTag("Ground") && CurrentState == LocomotionState.JUMPING)
        {
            CurrentState = LocomotionState.WALKING;
        }
    }

    public static void PlaySound(AudioClip clip)
    {
        instance.audioSource.PlayOneShot(clip);
    }


//Teleportation Particles
    void teleportVfx()
    {
        Instantiate(teleportationParticle, transform.position + Vector3.up * 0.5f, Quaternion.identity);
    }

    //Coroutine for instantiating the compass under the player. ONLY NEED TO MAKE INSTANTIATE CODE BUT INSTANTIATE AS CHILD OF PLAYER PARENT OBJECT!
    IEnumerator CompassCountdown()
    {
        Instantiate(compassPrefab, gameObject.transform);

        while(compassDuration > 0)
        {
            print("Compass Has spawned");
            yield return new WaitForSeconds(1f);
            compassDuration --;
        }

        yield return new WaitForSeconds(1f);

        print("Compass Has Faded");
    }
}
