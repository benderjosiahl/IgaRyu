using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTestController : MonoBehaviour
{
    public static PlayerTestController Instance;

    // Character state machine
    private PlayerBaseState MovingState;
    private PlayerBaseState CurrentActionState;

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
    [SerializeField] [Range(0, 600)] private float throwForce = 20.0f;
    [SerializeField] [Range(0, 600)] private float maxThrowDis = 20.0f;
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

    private Animator anim;

    private void Awake()
    {
        /*currentHealth = initHealth;

        audioSource = GetComponent<AudioSource>();

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            throw new System.Exception("This is a singleton, having multiple instances is not allowed");
        }

        anim = GetComponentInChildren<Animator>();*/
    }

    // Start is called before the first frame update
    void Start()
    {
        //MovingState = new PlayerMoveState();
        //MovingState.EnterState(this);

        // Begin action state machine
        //TransitionToState(NoActionState);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeState(PlayerBaseState state)
    {
        CurrentActionState = state;
        CurrentActionState.EnterState(this);
    }
}
