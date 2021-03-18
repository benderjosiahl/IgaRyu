using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    PlayerControls playerControls;

    //Variables for shooting
    public GameObject player;
    public GameObject projectilePrefab;
    private GameObject thisProjectile;
    public Transform shotSpawn;
    public float delay;
    public float fireRate;

    //variables for camera and movement
    public Vector2 movementInput;
    public Vector2 cameraInput;

    public float cameraInputX;
    public float cameraInputY;

    public float verticalInput;
    public float horizontalInput;

    public List<GameObject> projectilePrefabs;
    private int currentProjectileSelection = 0;
    public float throwForce = 4.0f;

    //Variables for teleportation
    private float startFOV = 90f;
    public float t = 1.0f;


    private void OnEnable()
    {
        if (playerControls == null)
        {
            playerControls = new PlayerControls();

            playerControls.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
            playerControls.PlayerMovement.Camera.performed += i => cameraInput = i.ReadValue<Vector2>();
            playerControls.PlayerMovement.Launch.performed += i => Shoot();
            playerControls.PlayerMovement.Aim.performed += i => Aim();
        }

        playerControls.Enable();
    }

    private void Aim()
    {
        print("you are aiming");
        /*if ("The object exists or something idk")
        {
            transform.position = projectile.transform.position;
        }*/
        player.transform.position = thisProjectile.transform.position;
        //Camera.main.fieldOfView = 100f;
        //Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, startFOV, t);
    }

    private void Shoot()
    {
        print("Fired the shot");
        //ThrowableController throwable = Instantiate(projectilePrefabs[currentProjectileSelection]).GetComponent<ThrowableController>();
        //throwable.Launch(Vector3.forward, throwForce);
        thisProjectile = (GameObject)Instantiate(projectilePrefab, shotSpawn.position, shotSpawn.rotation);
        //Camera.main.fieldOfView = 60f;
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    public void HandleAllInputs()
    {
        HandleMovementInput();
        //HandleJumpingInput
        //HandleActionInput
    }

    private void HandleMovementInput()
    {
        verticalInput = movementInput.y;
        horizontalInput = movementInput.x;

        cameraInputY = cameraInput.y;
        cameraInputX = cameraInput.x;
        if (cameraInput != Vector2.zero)
        {
            print(cameraInput);
        }
    }

}