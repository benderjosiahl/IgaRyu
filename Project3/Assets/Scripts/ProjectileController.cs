using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
     public AudioClip collectedClip;
    private Rigidbody rb;
    public float T;

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        
    }

    public void Launch(float force)
    {
        rb.AddForce(transform.forward * force, ForceMode.Impulse);
    }

    /*private void fixedUpdate()
    {
        T = Time.deltaTime;

        print(T);

        if (T > 2)
        {
            Destroy(gameObject);
            gameObject.SetActive(false);
        }
    }*/

    private void OnCollisionEnter (Collision other)
    {
        print("projectile collided with an object");
        // activate ability
        SoundDetectionBehavior.AlertAgentsToSound(other.transform.position, 1);
        Destroy(gameObject);
        gameObject.SetActive(false);


        //Sound Code

        PlayerController audiocontroller = GetComponent<PlayerController>();
        audiocontroller.PlaySound(collectedClip);
    }
}
