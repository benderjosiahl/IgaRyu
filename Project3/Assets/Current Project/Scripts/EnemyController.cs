using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    //variable that determines how far away an enemy can see
    public float lookRadius = 10f;

    //speed that the AI follows the path
    //public float agent.(speed);

    //funny dialogue
    /*public Text warningText;
    public Light flashLight;
    public float time;*/

    private Transform target;
    [SerializeField]
    private GameObject player;
    private NavMeshAgent agent;
    private int wavepointIndex = 0;

    /*//for freezing mechanic
    private bool isFrozen;*/

    // Start is called before the first frame update
    void Start()
    {
        target = Waypoints.points[0];

        agent = GetComponent<NavMeshAgent>();

        agent.SetDestination(target.position);

        //warningText.text = "";

        //isFrozen = false;
    }

    // Update is called once per frame
    void Update()
    {

        float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);

        if (distanceToPlayer <= lookRadius)
        {
            //warningText.text = "Take the Ankh and  your soul is forfeit";

            agent.SetDestination(player.transform.position);
        }
        else
        {
            target = Waypoints.points[wavepointIndex];
            //warningText.text = "";
            float distanceToWaypoint = Vector3.Distance(target.position, transform.position);
            //FollowPath();

            if (distanceToWaypoint <= 2f)
            {
                GetNextWaypoint();
            }

            agent.SetDestination(target.position);
        }

        /*if (isFrozen == true)
        {
            agent.speed = 0f;

            time -= Time.deltaTime;

            if (time <= 0)
            {
                isFrozen = false;
            }
            Debug.Log(time);
        }
        if (isFrozen == false)
        {
            agent.speed = 4f;
            time = 3;
        }*/

    }

    //function for facing the player when the AI is withing stopping distance of the player
    void FaceTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;

        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    //draws a red sphere acting as the detection radius for the AI
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }

    /*void FollowPath()
    {
        target = Waypoints.points[0];

        //Vector3 dir = target.position - transform.position;
        //transform.Translate(dir.normalized * speed * Time.deltaTime, Space.World);

        agent.SetDestination(target.position);

        if (Vector3.Distance(transform.position, target.position) <= 0.2f)
        {
            //FaceTarget();
            GetNextWaypoint();
        }
    }*/

    /* void FollowPlayer()
    {
        target = PlayerManager.instance.player.transform;

        float distance = Vector3.Distance(target.position, transform.position);
        agent.SetDestination(target.position);

        //warningText.text = "Take the Ankh and your soul mine";

        if (distance <= agent.stoppingDistance)
        {
            //Face the Target
            FaceTarget();
        }
    }*/

    void GetNextWaypoint()
    {
        if (wavepointIndex >= Waypoints.points.Length - 1)
        {
            target = Waypoints.points[0];
            wavepointIndex = 0;
        }
        else
        {
            wavepointIndex++;
            target = Waypoints.points[wavepointIndex];
        }
    }

    /*void OnTriggerEnter (Collider other)
    {

        if (other.gameObject.CompareTag("Light") && flashLight.enabled == true)
        {
            warningText.text = "IT BURNSSSSSSS";

            //agent.speed = 0f; 

            isFrozen = true;
        }
    }*/
    /*void OnTriggerEnter(Collider other)
    {
        float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);

        if (other.gameObject.CompareTag("Light") && flashLight.enabled == true)
        {
            warningText.text = "IT BURNSSSS!";

            agent.stoppingDistance = distanceToPlayer;

        }
    }*/
}
