using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompassController : MonoBehaviour
{
    private Transform mcGuffin;

    private void Awake()
    {
        mcGuffin = GameObject.FindGameObjectWithTag("McGuffin").GetComponent<Transform>();

        if(mcGuffin == null)
        {
            mcGuffin = gameObject.transform;
        }
    }

    private void FixedUpdate()
    {
        // set the rotation to face the gameobject
        Vector3 directionToMcGuffin = mcGuffin.position - transform.position;
        directionToMcGuffin = new Vector3(directionToMcGuffin.x, 0, directionToMcGuffin.z);
        transform.rotation = Quaternion.LookRotation(directionToMcGuffin, Vector3.up);
    }
}
