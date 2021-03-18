using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMover : MonoBehaviour
{
    public float speed = 2;

    public Rigidbody rb;

    private Vector3 direction;

    public LayerMask collisionCheck;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * speed;


        Debug.Log("The projectile is now moving");
    }

    private void OnTriggerEnter(Collider other)
    {
        gameObject.SetActive(false);
    }

}
