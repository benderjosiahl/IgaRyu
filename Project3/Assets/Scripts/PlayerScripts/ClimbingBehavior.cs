using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ClimbingBehavior : MonoBehaviour
{
    // components
    private Rigidbody rb;

    // transition variables
    private bool isInTransition;
    private float transitionTimer;
    private Transform helper;
    private Vector3 startPos;
    private Vector3 targetPos;
    private Quaternion startRot;
    private Quaternion targetRot;

    private float posOffset;

    [SerializeField] private float offsetFromWall = 0.3f;
    [SerializeField] private float objectHeight = 1.5f;
    [SerializeField] [Range(0,5  )] private float climbSpeed = 1;
    [SerializeField] [Range(0,5  )] private float rotateSpeed = 5;
    [SerializeField] [Range(0,300)] private float jumpForce = 200f;

    private bool testFlag = true;

    [SerializeField] private LayerMask mask;

    private void Awake()
    {
        isInTransition = false;

        helper = new GameObject().transform;
        helper.name = "Climb helper";

        rb = gameObject.GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Update function called to handle input
    /// </summary>
    /// <param name="inputVec"></param>
    /// <param name="delta"></param>
    public void StateUpdate(Vector2 inputVec, float delta)
    {
        

        // if you are currently moving, don't accept input
        // if the input is nothing, there's no change
        if (isInTransition || inputVec == Vector2.zero)
        {
            return;
        }

        // can we move there?

        // where are we moving?
        Vector3 horMovement = helper.right * inputVec.x;                // hor movement becomes a Vec3  pointing to the right at the length of the 
        Vector3 vertMovement = helper.up * inputVec.y;
        Vector3 moveDir = (horMovement + vertMovement).normalized;

        
        // can we move that way, or not?
        bool canMove = CanMove(moveDir);    // this function sets the position of our helper as well
        if (!canMove)
        {
            print("you can't move there");
            // if we cannot move there, stay
            return;
        }

        // otherwise, climb there
        transitionTimer = 0;
        isInTransition = true;
        startPos = transform.position;
        helper.position = new Vector3(helper.position.x, helper.position.y - objectHeight, helper.position.z);
        targetPos = helper.position;
        // now when you call climbing movement, it will move the player
    }

    /// <summary>
    /// update function called to change the game object's position (can put this in either Update or Fixed, depending on the setup)
    /// </summary>
    public void PerformMovement(float delta)
    {
        // if we are not supposed to be moving, return
        if (!isInTransition)
        {
            return;
        }

        // update your transition timer
        transitionTimer += Time.deltaTime * climbSpeed;
        //if your transition timer reaches 1, you should be there
        if (transitionTimer > 1)
        {
            transitionTimer = 1;
            isInTransition = false;
        }

        Vector3 cp = Vector3.Lerp(startPos, targetPos, transitionTimer);
        transform.position = cp;
        transform.rotation = Quaternion.Slerp(transform.rotation, helper.rotation, Time.deltaTime * rotateSpeed);
    }

    /// <summary>
    /// checks if there is a climbable surface (any surface) in front of the game object
    /// </summary>
    /// <returns></returns>
    public bool ShouldStartWallClimb()
    {
        // perform raycast from chest area of the player
        Vector3 origin = transform.position;
        origin.y += objectHeight;
        Vector3 dir = transform.forward;
        RaycastHit hit;
        Debug.DrawRay(origin, dir.normalized * offsetFromWall);
        // if we dit a climbable surface in front of us, init climb
        if (Physics.Raycast(origin, dir, out hit, offsetFromWall))
        {
            print("you hit " + hit.collider.name);
            helper.transform.position = PosWithOffset(origin, hit.point);
            InitClimb(hit);
            return true;
        }
        return false;
    }

    private void InitClimb(RaycastHit hit)
    {
        // sets the rotation of the helper to be opposite to the normal of the surface
        helper.transform.rotation = Quaternion.LookRotation(-hit.normal);
        startPos = transform.position;
        // target position is distance off of the wall from the point we hit with the raycast
        targetPos = hit.point + (hit.normal * offsetFromWall);
        transitionTimer = 0;
        isInTransition = true;

        if (rb != null)
        {
            print("you turned off the grav");
            rb.useGravity = false;
            rb.velocity = Vector3.zero;
        }
        // control your animator here to change anim to climbing
    }

    private bool CanMove(Vector3 moveDirection)
    {
        // location of the player object, off the wall
        Vector3 origin = transform.position;
        origin.y += objectHeight;
        // the distace we want to move our player over
        float moveDis = 1f;          // TODO: make the value here it's own variable and test out different values
        // the direction we want to move
        Vector3 dir = moveDirection;

        // draw a ray from the origin along the direciton we want to move
        Debug.DrawRay(origin, dir * moveDis, Color.red, 0.5f);
        RaycastHit hit;
        // if we hit something in the direction we want to move, we've hit a inner corner
        if(Physics.Raycast(origin, dir, out hit, moveDis, mask.value))
        {
            // TODO: move the player around that inner corner
            // note: maybe use initClimb to transition to other wall?
            if (hit.collider.gameObject.CompareTag("Ground"))  //<-------------------------------------------------custom code
            {
                PlayerController.CurrentState = PlayerController.State.WALKING;
                rb.useGravity = true; 
                transform.rotation = Quaternion.LookRotation(-transform.forward);   // Sets the rb to face away from the wall
            }
            return false;
        }

        // if we didn't hit something, we want to check if there is a surface in front of that point we want to move to
        origin += moveDirection * moveDis;  // the new origin is where we want to check from
        dir = helper.forward;               // the new direction is forward
        float dis = moveDis * 1.5f;         // this is the distance of the raycast forward, the larger the dist, the more "curved" a surface can be
                                            // TODO: make this value a variable and test how it works
        Debug.DrawRay(origin, dir * dis);
        if(Physics.Raycast(origin, dir, out hit, dis, mask.value))
        {
            if (hit.collider.gameObject.CompareTag("Ground"))  //<-------------------------------------------------custom code
            {
                PlayerController.CurrentState = PlayerController.State.WALKING;
                rb.useGravity = true;
                transform.rotation = Quaternion.LookRotation(-transform.forward);   // Sets the object to face away from the wall
                return false;
            }

            // if we hit the wall, set the position and rotation of the helper
            helper.position = PosWithOffset(origin, hit.point);
            helper.rotation = Quaternion.LookRotation(-hit.normal);
            return true;
        }

        // if we didn't hit something, we want to check if there is a surface around the corner we can transition to
        origin += dir * dis;
        dir = -moveDirection;

        Debug.DrawRay(origin, dir * dis);
        if(Physics.Raycast(origin, dir, out hit, dis, mask.value))
        {
            float angle = Vector3.Angle(helper.up, hit.normal);
            if (angle < 10) //<-------------------------------------------------custom code
            {
                rb.useGravity = true;
                transform.position = hit.point + new Vector3(0f, offsetFromWall, 0f);
                rb.position = hit.point + new Vector3(0f, offsetFromWall, 0f);
                PlayerController.CurrentState = PlayerController.State.WALKING;
                return false;
            }
            helper.position = PosWithOffset(origin, hit.point);
            helper.rotation = Quaternion.LookRotation(-hit.normal);
            return true;
        }

        return false;
    }

    public void performJump(Vector2 inputVec)
    {
        // get direction we are aiming
        Vector3 _inputDir = new Vector3(inputVec.x, 0, inputVec.y).normalized;

        // make vector for jumping 
        // the -1.5 float is to turn the vec around, and make the length longer than the dir vector so that the player always jumps away from the wall
        Vector3 _jumpVec = ((transform.forward * -1.5f) + _inputDir + (transform.up * 1.5f)).normalized;
        rb.useGravity = true;
        rb.AddForce(_jumpVec * jumpForce);                                  // Used to make the player jump, requires direction and measure of force that will be applied
        transform.rotation = Quaternion.LookRotation(-transform.forward);   // Sets the rb to face away from the wall
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground") && PlayerController.CurrentState == PlayerController.State.CLIMBING)
        {
            PlayerController.CurrentState = PlayerController.State.WALKING;
            rb.useGravity = true;
            transform.rotation = Quaternion.LookRotation(-transform.forward);   // Sets the object to face away from the wall
        }
    }

    private Vector3 PosWithOffset(Vector3 origin, Vector3 target)
    {
        Vector3 direction = (origin - target);
        direction.Normalize();
        Vector3 offset = direction * offsetFromWall;
        return target + offset;
    }
}
