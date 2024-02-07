using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YesserOthmene
{
    public class CameraManager : MonoBehaviour
    {
        InputManager inputManager;

        public Transform targetTransform;   //the object the camera will follow
        public Transform cameraPivot;       //the object the camera uses to pivot (up and down)
        public Transform cameraTransform;   //transform of the actual camera object in the scene
        public LayerMask collisionLayers;
        private float defaultPosition;
        private Vector3 cameraFollowVelocity = Vector3.zero;
        private Vector3 cameraVectorPosition;

        [SerializeField] float cameraCollisionOffset = 0.2f;
        [SerializeField] float minCollisionOffset = 0.2f;
        [SerializeField] float cameraCollisionRadius = 2.0f;  //how much the camera will jump of objects its colliding with
        [SerializeField] float cameraFollowSpeed = 0.2f;
        [SerializeField] float cameraLookSpeed = 2.0f;
        [SerializeField] float cameraPivotSpeed = 2.0f;

        public float lookAngle; //look camera up and down 
        public float pivotAngle; //look camera left and right

        [SerializeField] float minPivotAngle = -35f;
        [SerializeField] float maxPivotAngle = 35f;

        private void Awake()
        {
            inputManager = FindObjectOfType<InputManager>();
            targetTransform = FindObjectOfType<PlayerManager>().transform;
            cameraTransform = Camera.main.transform;
            defaultPosition = cameraTransform.localPosition.z;
        }

        public void HandleAllCameraMovement()
        {
            FollowTarget();
            RotateCamera();
            HandleCameraCollisions();
        }

        private void FollowTarget()
        {
            Vector3 targetPosition = Vector3.SmoothDamp(transform.position, targetTransform.position, ref cameraFollowVelocity, cameraFollowSpeed);
            transform.position = targetPosition;
        }

        public void RotateCamera()
        {
            Vector3 rotation;
            Quaternion targetRotaion;

            lookAngle += (inputManager.cameraInputX * cameraLookSpeed);
            pivotAngle -= (inputManager.cameraInputY * cameraPivotSpeed);
            pivotAngle = Mathf.Clamp(pivotAngle, minPivotAngle, maxPivotAngle);

            rotation = Vector3.zero;
            rotation.y = lookAngle;
            targetRotaion = Quaternion.Euler(rotation);
            transform.rotation = targetRotaion;

            rotation = Vector3.zero;
            rotation.x = pivotAngle;
            targetRotaion = Quaternion.Euler(rotation);
            cameraPivot.localRotation = targetRotaion;
        }

        private void HandleCameraCollisions()
        {
            float targetPosition = defaultPosition;

            RaycastHit hit;
            Vector3 direction = cameraTransform.position - cameraPivot.position;
            direction.Normalize();

            if (Physics.SphereCast(cameraPivot.transform.position, cameraCollisionRadius, direction, out hit, Mathf.Abs(targetPosition), collisionLayers))
            {
                float distance = Vector3.Distance(cameraPivot.position, hit.point);
                targetPosition = -(distance - cameraCollisionOffset);
            }

            if (Mathf.Abs(targetPosition) < minCollisionOffset)
            {
                targetPosition -= minCollisionOffset;
            }

            cameraVectorPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, 0.2f);
            cameraTransform.localPosition = cameraVectorPosition;
        }
    }
}
