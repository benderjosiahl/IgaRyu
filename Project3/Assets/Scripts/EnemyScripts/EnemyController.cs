using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyController: MonoBehaviour
{
    private EnemyState state;
    public EnemyState CurrentState
    {
        get { return state; }
        set { 
            // if the audio source is not playing (when it's coming out of IDLE), play the audio source
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }

            switch (value)
            {
                case EnemyState.IDLE:
                    audioSource.Pause(); 
                    idleTimer = timeToIdle;
                    anim.SetBool("isWalking", false);
                    break;
                case EnemyState.CHASING:
                    target = player.transform;
                    agent.SetDestination(player.transform.position);
                    anim.SetBool("isWalking", true);
                    break;
                case EnemyState.PATROLLING:
                    anim.SetBool("isWalking", true);
                    break;
                case EnemyState.ATTACKING:
                    attackTimer = timeBetweenAttacks;
                    agent.SetDestination(transform.position);
                    anim.SetBool("isAttacking", true);
                    anim.SetBool("isWalking", false);
                    break;
                case EnemyState.CURIOUS:
                    // location of the navagent cannot be controlled here, be sure to set the destination of the nav agent where you change it to curious
                    break;
            }
            print("Robot is now " + value);
            state = value;
        }
    }

    public enum EnemyState
    {
        IDLE,
        CHASING,
        PATROLLING,
        CURIOUS,
        ATTACKING
    }

    private Transform target;
    [SerializeField]
    private static GameObject player;
    private NavMeshAgent agent;
    [SerializeField] private Animator anim;
    [SerializeField] private GameObject laserPrefab;
    [SerializeField] private Transform laserSpawn;

    // List for waypoints
    public List<Transform> waypoints;
    private int waypointIndex = 0;

    // variables for idle state
    private float idleTimer;
    [SerializeField] [Range(0, 10)] private float timeToIdle = 4f;
    private bool permaIdle = false;

    // variables for curious state
    private float curiousTimer;
    [SerializeField] [Range(0, 10)] private float timeToCurious = 4f;

    //variables for attacking state
    private float attackTimer;
    [SerializeField] [Range(0, 10)] private float timeBetweenAttacks = 4f;
    [SerializeField] [Range(0, 10)] private float attackRange = 4f;

    //Sound Code   
    AudioSource audioSource;
    public AudioClip walkSound;
    bool m_Play;


    
    void Start()
    {
        audioSource= GetComponent<AudioSource>();

        // Setting the initial waypoint for the AI, if there are no points, then permanently idle
        if (waypoints.Count > 0)
        {
            target = waypoints[waypointIndex];
        } else
        {
            permaIdle = true;
        }

        // Creating a variable that manipulates the NavMesh Agent component
        agent = GetComponent<NavMeshAgent>();

        // Manipulating the NavMesh Agent component to set it's destination to the initial waypoint
        if (target != null)
        {
            agent.SetDestination(target.position);
        }

        player = PlayerController.instance.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case EnemyState.IDLE:
                // if we are idling, then subtract from the timer until it runs out
                idleTimer -= Time.deltaTime;
            
                //print(idleTimer);
                if(idleTimer <= 0 && !permaIdle)
                {
                    print("The timer ended");
                    CurrentState = EnemyState.PATROLLING;
                    // Calls for the function that generates the next waypoint
                    SetNextWaypoint();
                }
                break;
            case EnemyState.ATTACKING:
                // if we are attacking, subtract from the timer until we can attack again
                // if the timer has run out...
                if (attackTimer <= 0)
                {
                    //... attack the player, then if the player is not within range
                    attack(player);
                    if (Vector3.Distance(target.position, transform.position) > attackRange)
                    {
                        CurrentState = EnemyState.CHASING;
                        return;
                    }
                    attackTimer = timeBetweenAttacks;
                } else
                {
                    // charge for the attack
                    attackTimer -= Time.deltaTime;
                    // check if the target is still close enough
                }
                break;
            case EnemyState.CURIOUS:
                // if we are close enough to our destination, idle
                if (Vector3.Distance(agent.destination, transform.position) <= 2f)
                {
                    // if they have, make it idle next update call
                    CurrentState = EnemyState.IDLE;
                }
                break;
            case EnemyState.CHASING:
                if (Vector3.Distance(target.position, transform.position) <= attackRange)
                {
                    CurrentState = EnemyState.ATTACKING;
                }
                break;
            case EnemyState.PATROLLING:
                if (Vector3.Distance(target.position, transform.position) <= 2f)
                {
                    // if they have, make it idle next update call
                    CurrentState = EnemyState.IDLE;
                }
                break;
        }
    }

    private void attack(GameObject player)
    {
        print("We attacked the player");
        Vector3 vecToPlayer = player.transform.position - laserSpawn.position;
        Instantiate(laserPrefab, laserSpawn.position, Quaternion.LookRotation(vecToPlayer, Vector3.up)) ;
    }

    // Function for facing the player when the AI is withing stopping distance of the player
    void FaceTarget()
    {
        // Vector3 variable that stores the direction that the target is relative to the AI
        Vector3 direction = (target.position - transform.position).normalized;

        // Quaternion variable that stores the rotation that the AI needs to face the player
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

        // Set's the AI's rotation to be relative to where the player is
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    // Function called to set the next waypoint for the AI to go to
    private void SetNextWaypoint()
    {
        print("Got the next index");
        // Statement that triggers when the waypointIndex variable is larger than 1 minues the lenght of the waypoin list
        if (waypointIndex >= waypoints.Count - 1)
        {
            // set's the AI's target to be the original waypoin in the list
            target = waypoints[0];
            // reset's the waypoint index variable to it's original value
            waypointIndex = 0;
        }
        // Statement for when the original condition is not met
        else
        {
            // Raises the value of the waypointIndex variable by 1
            waypointIndex++;
            // Set's the AI's target to next waypoint in the list
            target = waypoints[waypointIndex];  // Set's the AI's target to the waypoint list value based on the waypointIndex value
        }
        agent.SetDestination(target.position);
    }

    public void AlertEnemyToPlayer()
    {
        if (state != EnemyState.ATTACKING || state != EnemyState.CHASING)
           CurrentState = EnemyState.CHASING;
    }
}
