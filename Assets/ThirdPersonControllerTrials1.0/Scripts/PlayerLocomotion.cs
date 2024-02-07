using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YesserOthmene
{
    public class PlayerLocomotion : MonoBehaviour
    {
        PlayerManager playerManager;
        AnimatorHandler animatorManager;
        InputManager inputManager;

        Vector3 moveDirection;
        Transform cameraObject;//which direction the camera facing
        Rigidbody rb;

        [Header("Falling")]
        public float inAirTimer;
        public float leapingVelocity;
        public float fallingVelocity;
        public float rayCastOffset = 0.5f;
        public LayerMask groundLayer;

        [Header("Movement Flags")]
        public bool isSprinting;
        public bool isGrounded;
        public bool isJumping;

        [Header("Movement Speeds")]
        public float walkingSpeed = 1.5f;
        public float runningSpeed = 5f;
        public float sprintingSpeed = 7f;
        public float rotationSpeed = 15f;

        [Header("Jump Speeds")]
        public float jumpHeight = 3f;
        public float gravityIntensity = -15f;

        private void Awake()
        {
            playerManager = GetComponent<PlayerManager>();
            animatorManager = GetComponent<AnimatorHandler>();
            inputManager = GetComponent<InputManager>();
            rb = GetComponent<Rigidbody>();
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
            moveDirection += cameraObject.right * inputManager.horizontalInput;
            moveDirection.Normalize();
            moveDirection.y = 0;

            //if we are sprinting, select the sprinting speed
            //if we are running select the running speed
            //if we are walking select the walking speed

            if (isSprinting)
            {
                moveDirection *= sprintingSpeed;
            }
            else
            {
                if (inputManager.moveAmount >= 0.5f)
                {
                    moveDirection *= runningSpeed;
                }
                else
                {
                    moveDirection *= walkingSpeed;
                }
            }

            Vector3 movementVelocity = moveDirection;
            rb.velocity = movementVelocity;
        }

        private void HandleRotation()
        {
            if (isJumping)
                return;

            Vector3 targetDirection = Vector3.zero;

            targetDirection = cameraObject.forward * inputManager.verticalInput;
            targetDirection += cameraObject.right * inputManager.horizontalInput;
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
            Vector3 rayCasOrigin = transform.position;
            Vector3 targetPosition;
            rayCasOrigin.y += rayCastOffset;
            targetPosition = transform.position;

            if (!isGrounded && !isJumping)
            {
                if (!playerManager.isInteracting)
                {
                    animatorManager.PlayTargetAnimation("Falling", true);
                }
                inAirTimer += Time.deltaTime;
                rb.AddForce(transform.forward * leapingVelocity);
                rb.AddForce(-Vector3.up * fallingVelocity * inAirTimer);
            }
            if (Physics.SphereCast(rayCasOrigin, 0.2f, -Vector3.up, out hit, groundLayer))
            {
                if (!isGrounded && !playerManager.isInteracting)
                {
                    animatorManager.PlayTargetAnimation("Land", false);
                }
                Vector3 rayCastHitPoint = hit.point;
                targetPosition.y = rayCastHitPoint.y;
                inAirTimer = 0;
                isGrounded = true;
            }
            else
            {
                isGrounded = false;
            }

            if (isGrounded && !isJumping)
            {
                if (playerManager.isInteracting || inputManager.moveAmount > 0)
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
                animatorManager.animator.SetBool("IsJumping", true);
                animatorManager.PlayTargetAnimation("Jump", true);

                float jumpingVelocity = Mathf.Sqrt(-2 * gravityIntensity * jumpHeight);
                Vector3 playerVelocity = moveDirection;
                playerVelocity.y = jumpingVelocity;
                rb.velocity = playerVelocity;
            }
        }
    }
}
