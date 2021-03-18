using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonMovement : MonoBehaviour
{
    [SerializeField]
    private CharacterController controller;
    public PlayerControls controls;

    public float speed = 6;

    void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();
        
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
