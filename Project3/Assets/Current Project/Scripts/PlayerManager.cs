using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    InputManager inputManager;
    PlayerLocomotion playerLocomotion;
    CameraManager cameraManager;

    public GameObject cameraObject;

    public void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Awake()
    {
        inputManager = GetComponent<InputManager>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
        cameraManager = cameraObject.GetComponent<CameraManager>();
    }

    private void Update()
    {
        inputManager.HandleAllInputs();
        playerLocomotion.HandleAllMovement();
        cameraManager.HandleAllCameraMovement();
    }

    private void FixedUpdate()
    {

    }
}