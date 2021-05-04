using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class KillVolumeScript : MonoBehaviour
{
    // Public Variables
    public GameObject RespawnLocation;
    
    //public GameObject KillVolume;
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.transform.position = RespawnLocation.transform.position;

            Debug.Log("You have fallen");
        }
    }
}
