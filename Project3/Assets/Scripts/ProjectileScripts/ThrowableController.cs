using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableController : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] [Range(0, 500)] private float throwVelocity; // represents units per second this is travelling
    [SerializeField] [Range(0, 500)] private float timeActive = 100;
    private float timer;
    [SerializeField] private bool isSticky = false;
    [SerializeField] private bool playerThrown = false;
    [SerializeField] private float damage = 5;
    [SerializeField] private float loudness = 10;

    [SerializeField] private LayerMask mask;


    //Particle Objects
    public GameObject kunaiParticle;


    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        timer = 0;
        rb.velocity = rb.transform.forward * throwVelocity;
    }

    private void FixedUpdate()
    {
        timer += Time.deltaTime;

        if (timer >= timeActive)
        {
            Destroy(this);
        }

        if (rb.velocity == Vector3.zero)
        {
            return;
        }

        // based on where we are, and the forces applied, where are we going?
        // throwVelocity
        Vector3 _moveDir = rb.transform.forward * throwVelocity * Time.fixedDeltaTime;

        //check if there is a body in the way via raycast to the destination
        RaycastHit hit;
        if (Physics.Raycast(rb.position, _moveDir, out hit, throwVelocity * Time.fixedDeltaTime, mask))
        {
            if (hit.collider.isTrigger)
            {
                return;
            }

            if (isSticky)
            {
                rb.velocity = Vector3.zero;
                rb.position = hit.point;
                kunaiContact();
                // TODO: trigger effect for the projectile hit
            }

            if (playerThrown)
            {
                SoundDetectionBehavior.AlertAgentsToSound(hit.point, loudness);
            }

            PlayerController _playerController = hit.collider.gameObject.GetComponent<PlayerController>();
            if (_playerController != null)
            {
                _playerController.ChangeHealth(-damage);
            }
        }
    }

    //Particle System function
    void kunaiContact()
    {
         Instantiate(kunaiParticle, rb.position + Vector3.up * 0.5f, Quaternion.identity);
    }

}
