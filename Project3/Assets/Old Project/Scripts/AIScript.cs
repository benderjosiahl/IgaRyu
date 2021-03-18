using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class AIScript : MonoBehaviour
{
    private Animator _animator;

    private NavMeshAgent _navMeshAgent;

    public GameObject Player;

    public float FollowDistance = 20.0f;

    public Transform[] patrolPoints;

    private int currentControlPointIndex = 0;


    void Awake()
    {

        _navMeshAgent = GetComponent<NavMeshAgent>();

        MoveToNextPatrolPoint();

    }

    // Update is called once per frame
    void Update()
    {
        if (_navMeshAgent.enabled)
        {
            float dist = Vector3.Distance(Player.transform.position, this.transform.position);

            bool patrol = false;
            bool follow = (dist < FollowDistance);

            if (follow)
            {
                float random = Random.Range(0.0f, 1.0f);
                _navMeshAgent.SetDestination(Player.transform.position);
            }

            patrol = !follow && patrolPoints.Length > 0;

            if (!follow && !patrol)
                _navMeshAgent.SetDestination(transform.position);

            // Patrolling between points if there are patrol points
            if (patrol)
            {
                if (!_navMeshAgent.pathPending &&
                    _navMeshAgent.remainingDistance < 0.5f)
                    MoveToNextPatrolPoint();
            }

        }
    }


    void MoveToNextPatrolPoint()
    {
        if (patrolPoints.Length > 0)
        {
            _navMeshAgent.destination = patrolPoints[currentControlPointIndex].position;

            currentControlPointIndex++;
            currentControlPointIndex %= patrolPoints.Length;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene("Game Over");
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

}
