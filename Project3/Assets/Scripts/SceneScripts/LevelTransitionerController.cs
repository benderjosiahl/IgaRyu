using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTransitionerController : MonoBehaviour
{
    [SerializeField] private GameManager.levels destinationLevel;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            GameManager.TransitionToLevel(destinationLevel);
        }
    }
}
