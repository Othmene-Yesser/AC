using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{
    PlayerManager playerManager;
    AnimatorManager animatorManager;
    InputManager inputManager;

    Vector3 moveDirection;
    Transform cameraObject;
    public Rigidbody playerRB;

    [Header ("Falling")]
    public float inAirTimer;
    [SerializeField] float leapingVelocity = 3;
    [SerializeField] float fallingVelocity = 33;
    [SerializeField] float rayCastHeightOffSet = 0.5f;
    [SerializeField] LayerMask groundLayer;

    [Header ("Movement Flags")]
    public bool isSprinting;
    public bool isGrounded;
    public bool isJumping;

    [Header ("Movement Speeds")]
    [SerializeField] float walkingSpeed = 2;
    [SerializeField] float runningSpeed = 5;
    [SerializeField] float sprintingSpeed = 8;
    [SerializeField] float rotationSpeed = 15;

    [Header ("Jump Speeds")]
    [SerializeField] float jumpHeight =1.5f;
    [SerializeField] float gravityIntensity = -15;

    private void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
        animatorManager = GetComponent <AnimatorManager>();
        inputManager = GetComponent<InputManager>();
        playerRB = GetComponent <Rigidbody>();
        cameraObject = Camera.main.transform;
    }

    public void HandleAllMovement()
        {
            HandleFallingAndLanding();

            if (playerManager.isInteracting)
                return;

            HandleMovement();
            HandleRotation();
        }

    private void HandleMovement()
    {
        if (isJumping)
            return;
        
        moveDirection = cameraObject.forward * inputManager.verticalInput;
        moveDirection = moveDirection + cameraObject.right * inputManager.horizontalInput;
        moveDirection.Normalize();
        moveDirection.y = 0;
        
        if(isSprinting)
        {
            moveDirection *=sprintingSpeed;
        }
        else
        {
            if(inputManager.moveAmount >= 0.5f)
            {
                moveDirection *= runningSpeed;
            }
            else
            {
                moveDirection = moveDirection * walkingSpeed;
            }
        }

         if (inputManager.horizontalInput == 0 && inputManager.verticalInput == 0)
        {
            playerRB.velocity = Vector3.zero;
        }
        else
        {
            Vector3 movementVelocity = moveDirection;
            playerRB.velocity = movementVelocity;
        }
    }
    
    private void HandleRotation()
    {
        if (isJumping)
            return;

        Vector3 targetDirection = Vector3.zero;

        targetDirection = cameraObject.forward * inputManager.verticalInput;
        targetDirection = targetDirection + cameraObject.right * inputManager.horizontalInput;
        targetDirection.Normalize();
        targetDirection.y = 0;

        if (targetDirection == Vector3.zero)
        {
            targetDirection = transform.forward;
        }
            
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        transform.rotation = playerRotation;

    }

    private void HandleFallingAndLanding()
    {
        RaycastHit hit;
        Vector3 rayCastOrigin = transform.position;
        Vector3 targetPosition;
        rayCastOrigin.y += rayCastHeightOffSet;
        targetPosition = transform.position;

        if (!isGrounded && !isJumping)
        {
            if(!playerManager.isInteracting)
            {
                animatorManager.PlayTargetAnimation("Falling",true);
            }

            animatorManager.animator.SetBool("isUsingRootMotion",false);
            inAirTimer+= Time.deltaTime;
            playerRB.AddForce(transform.forward * leapingVelocity);
            playerRB.AddForce(-Vector3.up * fallingVelocity * inAirTimer);
        }

        if (Physics.SphereCast(rayCastOrigin, 0.3f, -Vector3.up, out hit, groundLayer))
        {
            if (!isGrounded && !playerManager.isInteracting)
            {
                animatorManager.PlayTargetAnimation("Land",true);
            }

            Vector3 rayCastHitPoint = hit.point;
            targetPosition.y = rayCastHitPoint.y;
            inAirTimer = 0;
            isGrounded = true;
        }
        else
            isGrounded = false;

        if (isGrounded && !isJumping)
        {
            if(playerManager.isInteracting || inputManager.moveAmount >0)
            {
                transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime / 0.1f);
            }
            else
            {
                transform.position = targetPosition;
            }
        }
    }

    public void HandleJumping()
    {
        if (isGrounded)
        {
            animatorManager.animator.SetBool("isJumping",true);
            animatorManager.PlayTargetAnimation("Jump",false);

            float jumpingVelocity = Mathf.Sqrt(-2 * gravityIntensity * jumpHeight);
            Vector3 playerVelocity = moveDirection;
            playerVelocity.y= jumpingVelocity;
            playerRB.velocity = playerVelocity;
        }
    }

    public void HandleDodge()
    {
        if (playerManager.isInteracting)
            return;

        animatorManager.PlayTargetAnimation("Dodge",true, true);
    }
}