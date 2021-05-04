using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisionDetectionBehavior : MonoBehaviour
{
    private SphereCollider collider;
    private EnemyController controller;

    [SerializeField] [Range(5, 30)] private float playerDetectionRadius = 20f;
    [SerializeField] [Range(45, 180)] private float agentFieldOfView = 60f;

    // Start is called before the first frame update
    void Awake()
    {
        collider = GetComponent<SphereCollider>();
        collider.radius = playerDetectionRadius;

        controller = GetComponentInParent<EnemyController>();
    }

    private void OnTriggerStay(Collider other)
    {

        if (other.gameObject.CompareTag("Player"))
        {
            Vector3 vecToPlayer = other.transform.position - transform.position;

            if (Vector3.Angle(transform.forward, vecToPlayer) > agentFieldOfView)
            {
                return;
            }

            RaycastHit hit;
            if (Physics.Raycast(transform.position, vecToPlayer, out hit, vecToPlayer.magnitude))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    print("robot has seen the player");
                    // alert the robot
                    controller.AlertEnemyToPlayer();
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, playerDetectionRadius);
    }
}
