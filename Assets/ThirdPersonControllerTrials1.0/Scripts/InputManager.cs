using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YesserOthmene{
    public class InputManager : MonoBehaviour
    {
        PlayerInput playerControls;
        PlayerLocomotion playerLocomotion;
        AnimatorHandler animatorManager;

        public Vector2 movementInput;
        public Vector2 cameraInput;

        public float cameraInputX;
        public float cameraInputY;

        public float moveAmount;
        public float verticalInput;
        public float horizontalInput;

        public bool b_input; //sprinting
        public bool jump_Input;

        private void Awake()
        {
            animatorManager = GetComponent<AnimatorHandler>();
            playerLocomotion = GetComponent<PlayerLocomotion>();
        }

        private void OnEnable()
        {
            if (playerControls == null)
            {
                playerControls = new PlayerInput();

                //when we hit the keys WASD we are going to record the movement to movementInput
                playerControls.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
                //read values when mouse moves or right joystick moves
                playerControls.PlayerMovement.Camera.performed += i => cameraInput = i.ReadValue<Vector2>();
                //read if we clicked the sprinting button "B"
                playerControls.PlayerActions.B.performed += i => b_input = true;
                playerControls.PlayerActions.B.canceled += i => b_input = false;
                //read if we clicked space for the jump
                playerControls.PlayerActions.Jump.performed += i => jump_Input = true;
            }

            playerControls.Enable();
        }

        private void OnDisable()
        {
            playerControls.Disable();
        }

        public void HandleAllInputs()
        {
            HandleMovementInput();
            HandleSprintingInput();
            HandleJumpingInput();
            //handle action input
        }

        private void HandleMovementInput()
        {
            verticalInput = movementInput.y;
            horizontalInput = movementInput.x;

            cameraInputX = cameraInput.x;
            cameraInputY = cameraInput.y;

            moveAmount = Mathf.Clamp01(Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput));  //we do Mathf.abs because we use 0 to 1 values on the blend tree for walking forward
            animatorManager.UpdateAnimatorValues(0, moveAmount, playerLocomotion.isSprinting);
        }

        private void HandleSprintingInput()
        {
            if (b_input && moveAmount > 0.5f)
            {
                playerLocomotion.isSprinting = true;
            }
            else
            {
                playerLocomotion.isSprinting = false;
            }
        }

        private void HandleJumpingInput()
        {
            if (jump_Input)
            {
                jump_Input = false;
                playerLocomotion.HandleJumping();
            }
        }
    }
}