using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YesserOthmene
{
    public class PlayerManager : MonoBehaviour
    {
        Animator animator;
        InputManager inputManager;
        CameraManager cameraManager;
        PlayerLocomotion playerLocomotion;

        [HideInInspector]
        public bool isInteracting;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            inputManager = GetComponent<InputManager>();
            cameraManager = FindObjectOfType<CameraManager>();
            playerLocomotion = GetComponent<PlayerLocomotion>();
        }

        private void Update()
        {
            inputManager.HandleAllInputs();
        }

        private void FixedUpdate()
        {
            playerLocomotion.HandleAllMovement();
        }

        private void LateUpdate()
        {
            cameraManager.HandleAllCameraMovement();

            isInteracting = animator.GetBool("IsInteracting");
            playerLocomotion.isJumping = animator.GetBool("IsJumping");
            animator.SetBool("IsGrounded", playerLocomotion.isGrounded);
        }
    }
}
