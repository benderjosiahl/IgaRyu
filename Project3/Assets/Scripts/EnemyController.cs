using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyController: MonoBehaviour
{
    //Sound Code   
    AudioSource audioSource;
   public AudioClip walkSound;
     bool m_Play;



    
    // variable that determines how far away an enemy can see
    public float lookRadius = 10f;

    //speed that the AI follows the path
    //public float agent.(speed);

    private Transform target;
    [SerializeField]
    private GameObject player;
    private NavMeshAgent agent;

    // List for waypoints
    public List<Transform> waypoints;
    private int waypointIndex = 0;

    private float idleTimer;
    private bool isIdling = false;
    [SerializeField] [Range(0, 10)] private float timeToIdle = 4f;

    [SerializeField] private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
         audioSource= GetComponent<AudioSource>();

        // Setting the initial waypoint for the AI
        target = waypoints[waypointIndex];

        // Creating a variable that manipulates the NavMesh Agent component
        agent = GetComponent<NavMeshAgent>();

        // Manipulating the NavMesh Agent component to set it's destination to the initial waypoint
        agent.SetDestination(target.position);

    }

    // Update is called once per frame
    void Update()
    {
        // if we are idling, then subtract from the timer until it runs out
        if (isIdling)
        {
            idleTimer -= Time.deltaTime;
            audioSource.Pause();

            //print(idleTimer);
            if(idleTimer < 0)
            {
                print("The timer ended");
                isIdling = false;
                // Calls for the function that generates the next waypoint
                SetNextWaypoint();
                audioSource.Play();
                anim.SetBool("isWalking", true);
            }
            return;
        }

        //Creates a float value that measure's the AI's distance from the player
        float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);

        // Statement that triggers when the player is within the AI's look radius
        if (distanceToPlayer <= lookRadius)
        {
            // When the conditions for the statement are met, the AI's target become's the player's location
            agent.SetDestination(player.transform.position);
            // if we are close enough to the player, we don't want to check the distance to the player for idle
            return;
        }
        // If the agent is not detecting the player, check if they have reached their desination
        else if (Vector3.Distance(target.position, transform.position) <= 2f)
        {
            // if they have, make it idle next update call
            isIdling = true;
            idleTimer = timeToIdle;
            anim.SetBool("isWalking", false);
        }
    }

    // Function for facing the player when the AI is withing stopping distance of the player
    void FaceTarget()
    {
        // Vector3 variable that store's the direction that the target is relative to the AI
        Vector3 direction = (target.position - transform.position).normalized;

        // Quaternion variable that store's the rotation that the AI need's to face the player
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

        // Set's the AI's rotation to be relative to where the player is
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    // Draws a red sphere acting as the detection radius for the AI
    void OnDrawGizmosSelected()
    {
        // Sets the color of the sphere
        Gizmos.color = Color.red;

        // Draws the sphere (using the position of the object that this script is applied to as well as a variable that determines the radius)
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }

    // Function called to set the next waypoint for the AI to go to
    private void SetNextWaypoint()
    {
        print("Got the nest index");
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
}
