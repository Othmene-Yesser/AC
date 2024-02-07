using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace YesserOthmene
{
    public class EnemyLocomotionManager : MonoBehaviour
    {
        EnemyManager enemyManager;
        EnemyAnimatorManager enemyAnimatorManager;
        NavMeshAgent navMeshAgent;
        public Rigidbody enemyRB;

        public CharacterStats currentTarget;
        CharacterStats _currentTarget;

        [SerializeField] LayerMask detectionLayer;

        public float distanceFromTarget;
        public float stoppingDistance = 0.5f;

        public float rotationSpeed = 15;

        public float maxDistance = 10;


        private void Awake()
        {
            enemyRB = GetComponent<Rigidbody>();
            enemyManager= GetComponent<EnemyManager>();
            enemyAnimatorManager= GetComponent<EnemyAnimatorManager>();
            navMeshAgent = GetComponentInChildren<NavMeshAgent>();
        }

        private void Start()
        {
            navMeshAgent.enabled = false;
            enemyRB.isKinematic = false;
        }

        //private void OnDrawGizmos()
        //{
        //    Gizmos.color = Color.cyan;
        //    Gizmos.DrawSphere(transform.position, enemyManager.detectionRadius);
        //}

        //checks if there is a player in front of the enemey Duuh
        public void HandleDetection()
        {
            //To do change the OverlapSphere to something more optimized "kinda done"
            Ray ray = new Ray(transform.position - (transform.forward* enemyManager.detectionRadius), transform.forward);

            if (Physics.SphereCast(ray, enemyManager.detectionRadius, out RaycastHit hit, maxDistance, detectionLayer))
            {
                _currentTarget = hit.collider.GetComponent<CharacterStats>();

                float viewableAngle = Vector3.Angle(_currentTarget.transform.position - transform.position, transform.forward);
                Debug.Log("hit " + hit.collider.name + " / Is in FOV  = "+ (viewableAngle > enemyManager.minimumDetectionAngle && viewableAngle < enemyManager.maximumDetectionAngle));

                if (viewableAngle > enemyManager.minimumDetectionAngle && viewableAngle < enemyManager.maximumDetectionAngle)
                {
                    currentTarget = _currentTarget;
                }
            }
            else
            {
                Debug.Log("No player in range");
            }

        }

        public void HandleMoveToTarget()
        {
            Vector3 targetDirection = currentTarget.transform.position - transform.position;
            distanceFromTarget = Vector3.Distance(currentTarget.transform.position, transform.position);
            float viewableAngle = Vector3.Angle(targetDirection, transform.forward);

            //If the enemy is preforming an action, stop the movement !
            if (enemyManager.isPreformingAction)
            {
                enemyAnimatorManager.animator.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);
                navMeshAgent.enabled = false;
            }
            else
            {
                if (distanceFromTarget > stoppingDistance)
                {
                    enemyAnimatorManager.animator.SetFloat("Vertical", 1, 0.1f, Time.deltaTime);
                }
                else if (distanceFromTarget <= stoppingDistance)
                {
                    enemyAnimatorManager.animator.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);
                    //TO DO! Hit Player
                }
            }

            HandleRotateTowardsTarget();
            navMeshAgent.transform.localPosition = Vector3.zero;
            navMeshAgent.transform.localRotation = Quaternion.identity;
        }

        private void HandleRotateTowardsTarget()
        {
            //Rotate manually
            if (enemyManager.isPreformingAction)
            {
                Vector3 direction = currentTarget.transform.position - transform.position;
                direction.y = 0;
                direction.Normalize();

                if (direction == Vector3.zero)
                {
                    direction = transform.forward;
                }

                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed / Time.deltaTime);

            }
            //Rotate with pathfinding(NavMeshAgent)
            else
            {
                Vector3 relativeDirection = transform.InverseTransformDirection(navMeshAgent.desiredVelocity);
                Vector3 targetVelocity = enemyRB.velocity;

                navMeshAgent.enabled = true;
                navMeshAgent.SetDestination(currentTarget.transform.position);
                enemyRB.velocity = targetVelocity;
                transform.rotation = Quaternion.Slerp(transform.rotation, navMeshAgent.transform.rotation, rotationSpeed / Time.deltaTime);
            }
        }
    }
}
